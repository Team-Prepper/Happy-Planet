using EHTool.DBKit;
using System.Collections.Generic;
using UnityEngine;

public interface IField {

    [System.Serializable]
    public struct FieldMetaData : IDictionaryable<FieldMetaData> {
        public float _spendTime;
        public int _money;
        public int _energy;

        public FieldMetaData(float spendTime, int money, int energy)
        {
            _spendTime = spendTime;
            _money = money;
            _energy = energy;
        }

        public void SetValueFromDictionary(IDictionary<string, object> value)
        {
            _spendTime = float.Parse(value["_spendTime"].ToString());
            _money = int.Parse(value["_money"].ToString());
            _energy = int.Parse(value["_energy"].ToString());
        }

        public IDictionary<string, object> ToDictionary()
        {
            IDictionary<string, object> retval = new Dictionary<string, object>();

            retval["_spendTime"] = _spendTime;
            retval["_money"] = _money;
            retval["_energy"] = _energy;

            return retval;
        }
    }

    [System.Serializable]
    public class FieldLogData {
        public List<Log> _logs = new List<Log>();
    }

    public float Size { get; }
    public int Money { get; }
    public int Energy { get; }
    public float SpendTime { get; }
    public int Day { get; }
    public float MaxSpeed { get; }
    public FieldCameraSet.CameraSettingValue CameraSettingValue { get; }

    public void TimeAdd(float amount);

    public int CompareTime(float time);

    public int CompareTime(float time1, float time2);
    
    public void AddMoney(int earn);
    public void AddEnergy(int earn);

    public void ConnectDB(string targetAuth, string fieldName, IDatabaseConnector<FieldMetaData> metaDataConnector, IDatabaseConnector<Log> logDataConnector);
    public void Dispose();

    public void AddUnit(IUnit data, int cost);
    public void LevelUp(int id, int cost);
    public void RemoveUnit(IUnit data, int id, int cost);

    public void FieldMetaDataRead(CallbackMethod callback, CallbackMethod<string> fallback);
    public void FieldLogDataRead(CallbackMethod callback, CallbackMethod<string> fallback);

    public void RegisterUnit(int id, IUnit unit);
    public void UnregisterUnit(int id);
    public IUnit GetUnit(int id);
}