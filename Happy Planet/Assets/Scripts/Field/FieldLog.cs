using UnityEngine;
using EHTool;
using EHTool.DBKit;
using System.Collections.Generic;

public static class FieldLogUtils {
    public static string CostumVector3ToString(Vector3 v3)
    {
        return string.Format("{0}/{1}/{2}", v3.x, v3.y, v3.z);
    }

    public static Vector3 CostumStringToVector3(string x, string y, string z)
    {
        return new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
    }

    public static LogEvent GetEventFromString(string str)
    {

        string[] tocken = str.Split('/');

        switch (tocken[0])
        {
            case "01":
                return new CreateEvent(tocken);
            case "02":
                return new RemoveEvent(tocken);
            default:
                return new LevelUpEvent();

        }
    }

}

public interface LogEvent {
    public void Action(IField target, float time, int id, bool isAction = false);
    public void Undo(IField target, float time, int id);
}

class CreateEvent : LogEvent {
    Vector3 Position;
    Vector3 Dir;
    string UnitCode;

    internal CreateEvent(IUnit data)
    {
        Position = data.Pos;
        Dir = data.Dir;
        UnitCode = data.GetInfor().UnitCode;
    }
    internal CreateEvent(string[] code)
    {
        UnitCode = code[1];

        Position = FieldLogUtils.CostumStringToVector3(code[2], code[3], code[4]);
        Dir = FieldLogUtils.CostumStringToVector3(code[5], code[6], code[7]);
    }

    public override string ToString()
    {

        return string.Format("01/{0}/{1}/{2}", UnitCode, FieldLogUtils.CostumVector3ToString(Position), FieldLogUtils.CostumVector3ToString(Dir));

    }

    public void Action(IField target, float time, int id, bool isAction = false)
    {
        Unit newUnit = AssetOpener.ImportGameObject("Prefabs/unit").GetComponent<Unit>();

        newUnit.transform.position = Position;
        newUnit.transform.up = Dir;

        newUnit.SetInfor(UnitDataManager.Instance.GetUnitData(UnitCode), time, id, 0, !isAction);

        target.RegisterUnit(id, newUnit);

    }
    public void Undo(IField target, float time, int id)
    {
        target.GetUnit(id)?.Remove(time);
        target.UnregisterUnit(id);

    }
}

class RemoveEvent : LogEvent {

    Vector3 Position;
    Vector3 Dir;

    string UnitCode;
    int Level;
    float InitialTime;

    internal RemoveEvent(IUnit data)
    {
        Position = data.Pos;
        Dir = data.Dir;
        UnitCode = data.GetInfor().UnitCode;
        Level = data.NowLevel;
        InitialTime = data.InstantiateTime;
    }
    internal RemoveEvent(string[] code)
    {
        UnitCode = code[1];

        Position = FieldLogUtils.CostumStringToVector3(code[2], code[3], code[4]);
        Dir = FieldLogUtils.CostumStringToVector3(code[5], code[6], code[7]);

        Level = int.Parse(code[8]);
        InitialTime = float.Parse(code[9]);
    }
    public override string ToString()
    {
        return string.Format("02/{0}/{1}/{2}/{3}/{4}",
            UnitCode, FieldLogUtils.CostumVector3ToString(Position), FieldLogUtils.CostumVector3ToString(Dir), Level, InitialTime);

    }
    public void Action(IField target, float time, int id, bool isAction = false)
    {
        target.GetUnit(id)?.Remove(time, isAction);
        target.UnregisterUnit(id);

    }

    public void Undo(IField target, float time, int id)
    {
        Unit newUnit = AssetOpener.ImportGameObject("Prefabs/unit").GetComponent<Unit>();

        newUnit.transform.position = Position;
        newUnit.transform.up = Dir;

        newUnit.SetInfor(UnitDataManager.Instance.GetUnitData(UnitCode), InitialTime, id, Level);

        target.RegisterUnit(id, newUnit);

    }
}

class LevelUpEvent : LogEvent {
    public override string ToString()
    {
        return "03/";
    }
    public void Action(IField target, float time, int id, bool isAction = false)
    {
        target.GetUnit(id)?.LevelUp(time, isAction);
    }
    public void Undo(IField target, float time, int id)
    {
        target.GetUnit(id)?.LevelDown(time);
    }
}

[System.Serializable]
public struct Log : IDictionaryable<Log> {
    public float OccurrenceTime;
    public int TargetId;
    public int Cost;
    public string EventStr;

    internal Log(float time, int id, int cost, LogEvent even)
    {
        OccurrenceTime = Mathf.Max(0, time);
        TargetId = id;
        Cost = cost;
        EventStr = even.ToString();
    }

    internal Log(float time, int id, int cost, string eventStr)
    {

        OccurrenceTime = Mathf.Max(0, time);
        TargetId = id;
        Cost = cost;
        EventStr = eventStr;
    }

    public void Action(IField target)
    {
        GetEvent().Action(target, OccurrenceTime, TargetId, true);
    }

    public void Undo(IField target)
    {
        GetEvent().Undo(target, OccurrenceTime, TargetId);
        target.AddMoney(Cost);
    }

    public void Redo(IField target)
    {
        GetEvent().Action(target, OccurrenceTime, TargetId);
        target.AddMoney(-Cost);

    }

    public LogEvent GetEvent()
    {
        return FieldLogUtils.GetEventFromString(EventStr);
    }

    public IDictionary<string, object> ToDictionary()
    {
        IDictionary<string, object> retval = new Dictionary<string, object>();

        retval["OccurrenceTime"] = OccurrenceTime;
        retval["TargetId"] = TargetId;
        retval["Cost"] = Cost;
        retval["EventStr"] = EventStr;

        return retval;
    }

    public override string ToString()
    {
        return string.Format("{0}, {1}, {2}, {3}", OccurrenceTime, TargetId, Cost, EventStr);
    }

    public void SetValueFromDictionary(IDictionary<string, object> value)
    {
        OccurrenceTime = float.Parse(value["OccurrenceTime"].ToString());
        TargetId = int.Parse(value["TargetId"].ToString());
        Cost = int.Parse(value["Cost"].ToString());
        EventStr = value["EventStr"].ToString();
    }
}