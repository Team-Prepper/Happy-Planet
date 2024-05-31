using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DataManager : MonoSingleton<DataManager>
{
    static string CostumVector3ToString(Vector3 v3)
    {
        return string.Format("{0}/{1}/{2}", v3.x, v3.y, v3.z);
    }

    static Vector3 CostumStringToVector3(string x, string y, string z) {
        return new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
    }

    static private LogEvent GetEventFromString(string str) {

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

    public interface LogEvent {
        public void Action(float time, int id);
        public void Undo(float time, int id);
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

            Position = CostumStringToVector3(code[2], code[3], code[4]);
            Dir = CostumStringToVector3(code[5], code[6], code[7]);
        }

        public override string ToString()
        {
            
            return string.Format("01/{0}/{1}/{2}", UnitCode, CostumVector3ToString(Position), CostumVector3ToString(Dir));

        }

        public void Action(float time, int id)
        {
            Unit newUnit = AssetOpener.ImportGameObject("Prefabs/unit").GetComponent<Unit>();

            newUnit.transform.position = Position;
            newUnit.transform.up = Dir;

            newUnit.SetInfor(UnitDataManager.Instance.GetUnitData(UnitCode), time, id);

            if (id < Instance._units.Count)
            {
                Instance._units[id] = newUnit;
                return;
            }

            Instance._units.Add(newUnit);

        }
        public void Undo(float time, int id)
        {
            Instance._units[id].Remove();
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

            Position = CostumStringToVector3(code[2], code[3], code[4]);
            Dir = CostumStringToVector3(code[5], code[6], code[7]);

            Level = int.Parse(code[8]);
            InitialTime = float.Parse(code[9]);
        }
        public override string ToString()
        {
            return string.Format("02/{0}/{1}/{2}/{3}/{4}",
                UnitCode, CostumVector3ToString(Position), CostumVector3ToString(Dir), Level, InitialTime);

        }
        public void Action(float time, int id)
        {
            Instance._units[id].Remove();

        }

        public void Undo(float time, int id)
        {
            Unit newUnit = AssetOpener.ImportGameObject("Prefabs/unit").GetComponent<Unit>();

            newUnit.transform.position = Position;
            newUnit.transform.up = Dir;

            newUnit.SetInfor(UnitDataManager.Instance.GetUnitData(UnitCode), InitialTime, id);
            newUnit.SetLevel(Level);

            Instance._units[id] = newUnit;

        }
    }

    class LevelUpEvent : LogEvent {
        public override string ToString()
        {
            return "03/";
        }
        public void Action(float time, int id)
        {
            Instance._units[id].LevelUp();
        }
        public void Undo(float time, int id)
        {
            Instance._units[id].LevelDown();
        }
    }

    [System.Serializable]
    public struct Log {
        public float OccurrenceTime;
        public int TargetId;
        public int Cost;
        public string EventStr;

        internal Log(float time, int id, int cost, LogEvent even)
        {
            OccurrenceTime = time;
            TargetId = id;
            Cost = cost;
            EventStr = even.ToString();
        }

        internal Log(float time, int id, int cost, string eventStr) {

            OccurrenceTime = time;
            TargetId = id;
            Cost = cost;
            EventStr = eventStr;
        }

        public void Action() {
            GetEvent().Action(OccurrenceTime, TargetId);
        }

        public void Undo()
        {
            GetEvent().Undo(OccurrenceTime, TargetId);
            GameManager.Instance.AddMoney(Cost);
        }

        public void Redo() {
            Action();
            GameManager.Instance.AddMoney(-Cost);

        }

        public LogEvent GetEvent() {
            return GetEventFromString(EventStr);
        }

    }
}
