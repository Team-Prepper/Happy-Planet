using EHTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EHTool.DBKit;

public class WebGLLog : FirestoreWebGLConnector<DataManager.Log> { }
public class WebGLGameManagerData : FirestoreWebGLConnector<DataManager.GameManagerData> { }

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
    public struct GameManagerData : IDictionaryable<GameManagerData> {
        public float _spendTime;
        public int _money;
        public int _enegy;

        public GameManagerData(float spendTime, int money, int enegy)
        {
            _spendTime = spendTime;
            _money = money;
            _enegy = enegy;
        }

        public void SetValueFromDictionary(IDictionary<string, object> value)
        {
            _spendTime = float.Parse(value["_spendTime"].ToString());
            _money = int.Parse(value["_money"].ToString());
            _enegy = int.Parse(value["_enegy"].ToString());
        }

        public IDictionary<string, object> ToDictionary()
        {
            IDictionary<string, object> retval = new Dictionary<string, object>();

            retval["_spendTime"] = _spendTime;
            retval["_money"] = _money;
            retval["_enegy"] = _enegy;

            return retval;
        }
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

        if (id < _units.Count - 1)
        {
            _units[id] = null;
            return;
        }

        _units.RemoveAt(_units.Count - 1);

        while (Instance._units.Count > 0 && _units[_units.Count - 1] == null)
        {
            _units.RemoveAt(_units.Count - 1);

        }
    }

    protected override void OnCreate()
    {
        _units = new List<IUnit>();
        _logs = new List<Log>();

#if !UNITY_WEBGL || UNITY_EDITOR

        _gmDBConnector = new LocalDatabaseConnector<GameManagerData>();
        //_gmDBConnector = new FirestoreConnector<GameManagerData>();

        //_logDBConnector = new LocalDatabaseConnector<Log>();
        _logDBConnector = new FirestoreConnector<Log>();

#else
        _gmDBConnector = new LocalDatabaseConnector<GameManagerData>();
        //_gmDBConnector = gameObject.AddComponent<WebGLGameManagerData>();

        _logDBConnector = new LocalDatabaseConnector<Log>();
        //_logDBConnector = gameObject.AddComponent<WebGLLog>();

#endif

        _gmDBConnector.Connect("GameManagerData");
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

        GameManagerData data = new GameManagerData(GameManager.Instance.RealSpendTime, GameManager.Instance.Money, GameManager.Instance.Energy);

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

        _gmDBConnector.GetRecordAt((GameManagerData data) => {

            GameManager.Instance.SetInitial(data._spendTime, data._money, data._enegy);
            _fieldDataReadCallback();

        }, () => {
            _fieldDataReadCallback();

        }, 0);
    }

    public void LogDataRead(CallbackMethod callback)
    {

        _logDBConnector.GetAllRecord((IList<Log> data) => {

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

            callback();
        });

    }

}