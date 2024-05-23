using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DataManager : MonoSingleton<DataManager>
{
    static string CostumVector3ToString(Vector3 v3)
    {
        return v3.x.ToString() + "/" + v3.y.ToString() + "/" + v3.z.ToString();
    }

    static private LogEvent GetEventFromString(string str) {

        string[] tocken = str.Split('/');

        switch (tocken[0])
        {
            case "01":
                return new CreateEvent(tocken);
            case "02":
                return new LevelUpEvent();
            default:
                return new RemoveEvent(tocken);

        }
    }

    public interface LogEvent {
        public void Action(float time, int id);
        public void Undo();
    }

    class CreateEvent : LogEvent {
        Vector3 Position;
        Vector3 Dir;
        string UnitCode;

        internal CreateEvent(Vector3 pos, Vector3 dir, string code)
        {
            Position = pos;
            Dir = dir;
            UnitCode = code;
        }
        internal CreateEvent(string[] code)
        {
            UnitCode = code[1];
            Position = new Vector3(float.Parse(code[2]), float.Parse(code[3]), float.Parse(code[4]));
            Dir = new Vector3(float.Parse(code[5]), float.Parse(code[6]), float.Parse(code[7]));
        }
        public override string ToString()
        {
            return "01/" + UnitCode + "/" + CostumVector3ToString(Position) + "/" + CostumVector3ToString(Dir);

        }
        public void Action(float time, int id)
        {
            Unit newUnit = AssetOpener.ImportGameObject("Prefabs/unit").GetComponent<Unit>();

            newUnit.transform.position = Position;
            newUnit.transform.up = Dir;

            newUnit.SetInfor(UnitDataManager.Instance.GetUnitData(UnitCode), time, 0);
            Instance._units.Add(newUnit);

        }
        public void Undo()
        {

        }
    }

    class LevelUpEvent : LogEvent {
        public override string ToString()
        {
            return "02/";
        }
        public void Action(float time, int id)
        {
            Instance._units[id].LevelUp();
        }
        public void Undo()
        {

        }
    }


    class RemoveEvent : LogEvent {
        Vector3 Position;
        Vector3 Dir;
        string UnitCode;

        internal RemoveEvent(Vector3 pos, Vector3 dir, string code)
        {
            Position = pos;
            Dir = dir;
            UnitCode = code;
        }
        internal RemoveEvent(string[] code)
        {
            UnitCode = code[1];
            Position = new Vector3(float.Parse(code[2]), float.Parse(code[3]), float.Parse(code[4]));
            Dir = new Vector3(float.Parse(code[5]), float.Parse(code[6]), float.Parse(code[7]));
        }
        public override string ToString()
        {
            return "03/" + UnitCode + "/" + CostumVector3ToString(Position) + "/" + CostumVector3ToString(Dir);

        }
        public void Action(float time, int id)
        {
            Instance._units[id].Remove();

        }
        public void Undo()
        {

        }
    }

    [System.Serializable]
    public struct Log {
        public float OccurrenceTime;
        public int TargetId;
        public string EventStr;

        internal Log(float time, int id, LogEvent even)
        {
            OccurrenceTime = time;
            TargetId = id;
            EventStr = even.ToString();
        }

        internal Log(float time, int id, string eventStr) {

            OccurrenceTime = time;
            TargetId = id;
            EventStr = eventStr;
        }

        public LogEvent GetEvent() {
            return GetEventFromString(EventStr);
        }

    }
}
