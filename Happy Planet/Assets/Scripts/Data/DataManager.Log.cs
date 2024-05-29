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

        internal RemoveEvent(IUnit data)
        {
            Position = data.Pos;
            Dir = data.Dir;
            UnitCode = data.GetInfor().UnitCode;
            Level = data.NowLevel;
        }
        internal RemoveEvent(string[] code)
        {
            UnitCode = code[1];
            Position = new Vector3(float.Parse(code[2]), float.Parse(code[3]), float.Parse(code[4]));
            Dir = new Vector3(float.Parse(code[5]), float.Parse(code[6]), float.Parse(code[7]));
            Level = int.Parse(code[8]);
        }
        public override string ToString()
        {
            return "02/" + UnitCode + "/" + CostumVector3ToString(Position) + "/" + CostumVector3ToString(Dir) + "/" + Level;

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

            newUnit.SetInfor(UnitDataManager.Instance.GetUnitData(UnitCode), time, id);
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
