using System;
using UnityEngine;

using FieldDataDB = EHTool.DBKit.IDatabaseConnector<FieldDataRecord>;
using LogDB = EHTool.DBKit.IDatabaseConnector<Log>;

public class PlaygroundField : IField {

    public PlanetData PlanetData { get; private set; }
    public FieldData FieldData { get; private set; } = FieldData.Default;

    private FieldDataDB _metaDBConnector;
    [SerializeField] private float _saveRoutine = 1f;
    private int _routineId = -1;
    private bool _metaDataExist = false;

    private LogFile _logFile = new LogFile();
    private UnitList _unitList = new UnitList();
    
    private GameObject _planet;
    private string _fieldName;

    public void AddTime(float amount) {

        FieldData.AddTime(amount, () => {

            while (_logFile.TopUnder != null)
            {
                if (FieldData.CompareTime
                    (_logFile.TopUnder.Value.OccurrenceTime) >= 0) break;
                _logFile.Undo(this);
            }

            while (_logFile.Top != null)
            {
                if (FieldData.CompareTime
                    (_logFile.Top.Value.OccurrenceTime) < 0) break;
                _logFile.Redo(this);
            }

        });

    }

    public void AddMoney(int earn)
    {
        FieldData.AddMoney(earn);
    }

    public void AddEnergy(int earn) {
        FieldData.AddEnergy(earn);
    }

    public void ConnectDB(string auth, string fieldName,
        FieldDataDB metaDataConnector, LogDB logDataConnector) {

        PlanetData = FieldManager.Instance.GetFieldData(fieldName);
        _fieldName = string.Format("{0}{1}", auth, fieldName);

        metaDataConnector.Connect(auth, string.Format("MetaData{0}", fieldName));
        logDataConnector.Connect(auth, string.Format("LogData{0}", fieldName));

        _metaDBConnector = metaDataConnector;
        _logFile.SetDBConnector(logDataConnector);

    }

    public void Dispose() {
        if (_planet != null)
        {
            GameObject.Destroy(_planet);
        }
        if (_routineId != -1) {
            GameManager.Instance.RemoveRoutineMethod(_routineId);
        }
        _unitList.Clear(FieldData.SpendTime);
    }

    private void _AddLog(int id, ILogEvent logevent, int cost)
    {
        _logFile.AddLog(new Log(FieldData.SpendTime, id, cost, logevent.ToString()));

        AddMoney(-cost);

        _MetaDataWrite();
    }

    public void LogAddUnit(IUnit data, int cost)
    {
        _unitList.AddUnit(data);
        _AddLog(data.Id, new CreateEvent(data), cost);

    }

    public void LogLevelUp(int id, int cost)
    {
        _AddLog(id, new LevelUpEvent(), cost);
    }

    public void LogRemoveUnit(IUnit data, int id, int cost)
    {
        _AddLog(id, new RemoveEvent(data), cost);

        UnregisterUnit(id);
    }

    public void LoadFieldMetaData(Action callback, Action<string> fallback)
    {
        _planet = FieldManager.Instance.InitPlanet(PlanetData.GetPrefab());

        if (_logFile.IsLoaded) {
            callback?.Invoke();
            return;
        }

        _metaDBConnector.GetRecordAt((data) => {

            FieldData = new FieldData(data.SpendTime, data.Money, data.Energy);
            callback?.Invoke();

            _metaDataExist = true;

        }, (msg) => {

            FieldData = FieldData.Default;
            callback?.Invoke();

            _metaDataExist = false;

        }, 0);

    }

    public void LoadLog(Action callback, Action<string> fallback)
    {

        void FieldLoadSuccess() {

            callback?.Invoke();

            FieldManager.Instance.AddFieldSet(_fieldName, this);

            _routineId = GameManager.Instance.AddRoutineMethod
                (_MetaDataWrite, _saveRoutine);

        }

        _logFile.LoadLog(() => {

            while (_logFile.Top != null)
            {
                if (_logFile.Top.Value.OccurrenceTime > FieldData.SpendTime)
                {
                    break;
                }
                _logFile.Do(this);
            }

            FieldLoadSuccess();

        }, (msg) => {

            if (_metaDataExist) {
                fallback?.Invoke(msg);
                return;
            }

            _logFile.CreateDB();
            _metaDataExist = true;

            FieldLoadSuccess();
            
        });

    }

    void _MetaDataWrite()
    {
        _metaDBConnector.UpdateRecordAt(
            new FieldDataRecord(FieldData.SpendTime, FieldData.Money, FieldData.Energy), 0);
    }

    public IUnit GetUnit(int id)
        => _unitList.GetUnit(id);

    public void RegisterUnit(int id, IUnit unit)
        => _unitList.RegisterUnit(id, unit);

    public void UnregisterUnit(int id)
        => _unitList.UnregisterUnit(id);

}