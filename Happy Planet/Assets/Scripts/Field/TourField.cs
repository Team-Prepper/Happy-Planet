using EHTool.DBKit;
using static IField;
using System.Collections.Generic;
using UnityEngine;

public class TourField : IField {

    FieldData _fieldData;
    GameObject _planet;

    static readonly int TimeQuantization = 144;

    IList<IUnit> _units;

    IDatabaseConnector<Log> _logDBConnector;
    IDatabaseConnector<FieldMetaData> _metaDBConnector;

    float _spendTime = 3f;

    public float Size => _fieldData.Size;
    public int Money => 0;
    public int Energy => 100;
    public float SpendTime => _spendTime;
    public int Day => Mathf.Max(0, Mathf.FloorToInt(SpendTime));

    public float MaxSpeed {
        get {
            return _fieldData.Speed;
        }
    }
    public FieldCameraSet.CameraSettingValue CameraSettingValue => _fieldData.CameraSettingValue;

    public void AddEnergy(int earn)
    {
    }

    public void AddMoney(int earn)
    {
    }

    public void AddUnit(IUnit data, int cost)
    {
    }

    public void LevelUp(int id, int cost)
    {

    }

    public void RemoveUnit(IUnit data, int id, int cost)
    {
    }

    public void TimeAdd(float amount)
    {

    }

    public int CompareTime(float time)
    {
        return CompareTime(SpendTime, time);
    }

    public int CompareTime(float time1, float time2)
    {
        if (Mathf.Abs(time1 - time2) * TimeQuantization < 0.5f)
        {
            return 0;
        }
        if (time1 > time2) return 1;

        return -1;
    }

    public void ConnectDB(string targetAuth, string fieldName, IDatabaseConnector<FieldMetaData> metaDataConnector, IDatabaseConnector<Log> logDataConnector)
    {
        _fieldData = FieldManager.Instance.GetFieldData(fieldName);

        _metaDBConnector = metaDataConnector;
        _logDBConnector = logDataConnector;

        _metaDBConnector.Connect(targetAuth, string.Format("MetaData{0}", fieldName));
        _logDBConnector.Connect(targetAuth, string.Format("LogData{0}", fieldName));
    }

    public void Dispose()
    {
        if (_planet != null)
        {
            Object.Destroy(_planet);
        }

        foreach (IUnit unit in _units)
        {
            if (unit == null) continue;
            unit.Remove(SpendTime);
        }
        _units = new List<IUnit>();

    }

    public void FieldMetaDataRead(CallbackMethod callback, CallbackMethod<string> fallback)
    {
        _planet = FieldManager.Instance.InitPlanet(_fieldData.GetPlanetPrefab());

        _units = new List<IUnit>();

        _metaDBConnector.GetRecordAt((FieldMetaData data) => {
            
            _spendTime = Mathf.Max(data._spendTime, 0);

            callback?.Invoke();

        }, (string msg) => {
            fallback?.Invoke(msg);
        }, 0);
    }
    
    public void FieldLogDataRead(CallbackMethod callback, CallbackMethod<string> fallback)
    {
        _logDBConnector.GetAllRecord((IList<Log> data) => {

            foreach (Log log in data)
            {
                if (log.OccurrenceTime > _spendTime)
                {
                    continue;
                }
                log.Action(this);
            }

            callback?.Invoke();
        }, (string msg) => {
            fallback?.Invoke(msg);
        });
    }

    public void RegisterUnit(int id, IUnit unit)
    {
        while (id >= _units.Count)
        {
            _units.Add(null);
        }

        _units[id] = unit;
    }

    public IUnit GetUnit(int id)
    {
        return _units[id];
    }

    public void UnregisterUnit(int id)
    {
        if (id < _units.Count - 1)
        {
            _units[id] = null;
            return;
        }

        _units.RemoveAt(_units.Count - 1);

        while (_units.Count > 0 && _units[_units.Count - 1] == null)
        {
            _units.RemoveAt(_units.Count - 1);

        }
    }
}