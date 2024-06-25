using EHTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DataManager : MonoSingleton<DataManager> {

    [SerializeField] float _saveRoutine = 1f;

    IList<Log> _logs;
    int _validLogCount;
    int _logCursor;

    IList<IUnit> _units;
    ISet<CallbackMethod> _callbacks;

    IDatabaseConnector<Log> _logDBConnector;
    IDatabaseConnector<GameManagerData> _gmDBConnector;

    CallbackMethod _fieldDataReadCallback;
    CallbackMethod _logDataReadCallback;

    [System.Serializable]
    public class GameManagerData {
        public float _spendTime = 0;
        public int _money = 0;
        public int _enegy = 0;

    }

    [System.Serializable]
    public class LogData {
        public List<Log> _logs = new List<Log>();
    }

    private void _AddLog(Log log, int cost)
    {
        if (_logCursor < _logs.Count) _logs[_logCursor] = log;
        else _logs.Add(log);


        _validLogCount = ++_logCursor;

        GameManager.Instance.AddMoney(-cost);

        _GameDataWrite();
        _logDBConnector.UpdateRecordAt(log, _validLogCount - 1);
    }

    private void _PopLog()
    {
        _logs[--_logCursor].Undo();
    }

    public void AddUnit(IUnit data, int cost) {
        _AddLog(new Log(GameManager.Instance.SpendTime, _units.Count, cost, new CreateEvent(data)), cost);

        data.SetId(_units.Count);
        _units.Add(data);

    }

    public void LevelUp(int id, int cost)
    {
        _AddLog(new Log(GameManager.Instance.SpendTime, id, cost, new LevelUpEvent()), cost);
    }

    public void RemoveUnit(IUnit data, int id, int cost)
    {
        _AddLog(new Log(GameManager.Instance.SpendTime, id, cost, new RemoveEvent(data)), cost);

        if (id < _units.Count)
        {
            _units[id] = null;
            return;
        }

        _units.RemoveAt(_units.Count - 1);

        while (_units[_units.Count - 1] == null)
        {
            _units.RemoveAt(_units.Count - 1);

        }
    }

    protected override void OnCreate()
    {
        _units = new List<IUnit>();
        _logs = new List<Log>();

        _gmDBConnector = new LocalDatabaseConnector<GameManagerData>();
        //_gmDBConnector = new FirestoreConnector<GameManagerData>();
        _gmDBConnector.Connect("GameManagerData");

        _logDBConnector = new LocalDatabaseConnector<Log>();
        //_logDBConnector = new FirestoreConnector<Log>();
        _logDBConnector.Connect("LogData");

        _callbacks = new HashSet<CallbackMethod>();
    }

    private void Start()
    {
        StartCoroutine(_RoutineDataSave());
    }

    public void TimeChangeEvent(float nowTime)
    {

        while (_logCursor > 0 && _logs[_logCursor - 1].OccurrenceTime > nowTime)
        {
            _PopLog();
        }

        while (_logCursor < _validLogCount && _logs[_logCursor].OccurrenceTime <= nowTime) {

            _logs[_logCursor].Redo();
            _logCursor++;
        }

    }

    void _GameDataWrite()
    {

        GameManagerData data = new GameManagerData();
        data._spendTime = GameManager.Instance.RealSpendTime;
        data._money = GameManager.Instance.Money;
        data._enegy = GameManager.Instance.Energy;

        _gmDBConnector.UpdateRecordAt(data, 0);

    }

    IEnumerator _RoutineDataSave()
    {
        while (true) {
            yield return new WaitForSecondsRealtime(_saveRoutine);
            _GameDataWrite();
        }

    }

    public void FieldDataRead(CallbackMethod callback)
    {
        _fieldDataReadCallback = callback;

        _units = new List<IUnit>();
        _logs = new List<Log>();
        _gmDBConnector.GetRecordAt(GetGameManagerDataCallback, GetGameManagerDataFallback, 0);
    }

    private void GetGameManagerDataCallback(GameManagerData data)
    {
        GameManager.Instance.SetInitial(data._spendTime, data._money, data._enegy);
        _fieldDataReadCallback();

    }

    private void GetGameManagerDataFallback() {
        _fieldDataReadCallback();
    }

    public void LogDataRead(CallbackMethod callback)
    {

        _logDataReadCallback = callback;
        _logDBConnector.GetAllRecord(GetLogDataCallback);

    }

    private void GetLogDataCallback(IList<Log> data)
    {

        _logs = data;
        _logCursor = 0;
        _validLogCount = data.Count;

        foreach (Log log in _logs)
        {
            if (log.OccurrenceTime > GameManager.Instance.RealSpendTime)
            {
                continue;
            }
            log.Action();
            _logCursor++;
        }

        _logDataReadCallback();
    }
}
