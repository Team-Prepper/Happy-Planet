using EHTool.DBKit;
using System.Collections.Generic;
using UnityEngine;
using static IField;

public class PlaygroundField : IField {

    FieldData _fieldData;
    GameObject _planet;

    static readonly int TimeQuantization = 144;
    string _fieldName;

    [SerializeField] float _saveRoutine = 1f;

    IList<Log> _logs;
    int _validLogCount;
    int _logCursor;

    bool _isLoaded = false;
    bool _metaDataExist = false;

    IList<IUnit> _units;

    IDatabaseConnector<Log> _logDBConnector;
    IDatabaseConnector<FieldMetaData> _metaDBConnector;

    public float Size => _fieldData.Size;
    public int Money { get; private set; } = 1000;
    public int Energy { get; private set; } = 100;

    public float SpendTime => _spendTime;

    public int Day => Mathf.Max(0, Mathf.FloorToInt(SpendTime));

    public float MaxSpeed {
        get {
            return _fieldData.Speed;
        }
    }

    public FieldCameraSet.CameraSettingValue CameraSettingValue => _fieldData.CameraSettingValue;

    float _realSpendTime = 0;
    float _spendTime = 0;

    public void TimeAdd(float amount) {

        _realSpendTime += amount;

        float tmp = Mathf.Round(_realSpendTime * TimeQuantization);

        if (CompareTime(_realSpendTime) == 0) return;

        _spendTime = tmp / TimeQuantization;
        SoundManager.Instance.PlaySound("Tick");

        if (_spendTime < 0) {
            _spendTime = -1f / TimeQuantization;
        }

        while (_logCursor > 0 && CompareTime(_logs[_logCursor - 1].OccurrenceTime) < 0)
        {
            _PopLog();
        }

        while (_logCursor < _validLogCount && CompareTime(_logs[_logCursor].OccurrenceTime) >= 0)
        {
            _logs[_logCursor].Redo(this);
            _logCursor++;
        }
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

    public void AddMoney(int earn)
    {
        Money += earn;
    }

    public void AddEnergy(int earn) {

        Energy += earn;

        if (Energy < 0)
            Energy = 0;
    }

    public void ConnectDB(string targetAuth, string fieldName, IDatabaseConnector<FieldMetaData> metaDataConnector, IDatabaseConnector<Log> logDataConnector) {

        _fieldName = targetAuth + fieldName;
        _fieldData = FieldManager.Instance.GetFieldData(fieldName);

        _metaDBConnector = metaDataConnector;
        _logDBConnector = logDataConnector;

        _metaDBConnector.Connect(targetAuth, string.Format("MetaData{0}", fieldName));
        _logDBConnector.Connect(targetAuth, string.Format("LogData{0}", fieldName));

    }

    public void Dispose() {
        if (_planet != null)
        {
            Object.Destroy(_planet);
        }

        foreach (IUnit unit in _units) {
            if (unit == null) continue;
            unit.Remove(SpendTime);
        }
        _units = new List<IUnit>();
    }
    
    private void _PopLog()
    {
        _logs[--_logCursor].Undo(this);
    }

    private void _AddLog(Log log, int cost)
    {
        if (_logCursor < _logs.Count) _logs[_logCursor] = log;
        else _logs.Add(log);


        _validLogCount = ++_logCursor;

        AddMoney(-cost);

        _MetaDataWrite();
        _logDBConnector.UpdateRecordAt(log, _validLogCount - 1);
    }

    public void AddUnit(IUnit data, int cost)
    {
        _AddLog(new Log(SpendTime, _units.Count, cost, new CreateEvent(data)), cost);

        data.SetId(_units.Count);
        _units.Add(data);

    }

    public void LevelUp(int id, int cost)
    {
        _AddLog(new Log(SpendTime, id, cost, new LevelUpEvent()), cost);
    }

    public void RemoveUnit(IUnit data, int id, int cost)
    {
        _AddLog(new Log(SpendTime, id, cost, new RemoveEvent(data)), cost);

        UnregisterUnit(id);
    }

    public void FieldMetaDataRead(CallbackMethod callback, CallbackMethod<string> fallback)
    {
        _planet = FieldManager.Instance.InitPlanet(_fieldData.GetPlanetPrefab());

        if (_isLoaded) {
            callback?.Invoke();
            return;
        }

        _units = new List<IUnit>();
        _logs = new List<Log>();

        _metaDBConnector.GetRecordAt((FieldMetaData data) => {
            
            _realSpendTime = data._spendTime;
            _spendTime = data._spendTime;
            Money = data._money;
            Energy = data._energy;

            callback?.Invoke();

            DataManager.Instance.RoutineCallMethod(_MetaDataWrite, _saveRoutine);

            _metaDataExist = true;

        }, (string msg) => {

            _realSpendTime = -1f / TimeQuantization;
            _spendTime = _realSpendTime;

            _metaDataExist = false;

            callback();

            DataManager.Instance.RoutineCallMethod(_MetaDataWrite, _saveRoutine);

        }, 0);

    }

    public void FieldLogDataRead(CallbackMethod callback, CallbackMethod<string> fallback)
    {
        if (_isLoaded) {
            _DoEvent();
            callback?.Invoke();
            FieldManager.Instance.AddFieldSet(_fieldName, this);
            return;
        }

        _logDBConnector.GetAllRecord((IList<Log> data) => {

            _logs = data;
            _DoEvent();

            _isLoaded = true;

            FieldManager.Instance.AddFieldSet(_fieldName, this);
            callback?.Invoke();

        }, (string msg) => {

            if (_metaDataExist) {
                fallback?.Invoke(msg);
                return;
            }

            _logDBConnector.UpdateRecordAt(new Log(-1, -1, 0, ""), -1);
            callback();

            _metaDataExist = true;
            _isLoaded = true;

        });
    }

    void _DoEvent()
    {
        _logCursor = 0;
        _validLogCount = _logs.Count;

        foreach (Log log in _logs)
        {
            if (log.OccurrenceTime > _realSpendTime)
            {
                continue;
            }
            log.Action(this);
            _logCursor++;
        }

    }

    public IUnit GetUnit(int id)
    {
        return _units[id];
    }

    public void RegisterUnit(int id, IUnit unit)
    {
        while (id >= _units.Count)
        {
            _units.Add(null);
        }

        _units[id] = unit;
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

    void _MetaDataWrite()
    {

        FieldMetaData data = new FieldMetaData(SpendTime, Money, Energy);

        _metaDBConnector.UpdateRecordAt(data, 0);

    }
}