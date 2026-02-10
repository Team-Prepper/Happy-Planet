using UnityEngine;
using System;

using FieldDataDB = EasyH.Tool.DBKit.IDatabaseConnector<string, FieldDataRecord>;
using LogDB = EasyH.Tool.DBKit.IDatabaseConnector<int, Log>;

public class TourField : IField {

    public PlanetData PlanetData { get; private set; }
    public FieldData FieldData { get; private set; } = FieldData.Default;
    private FieldDataDB _metaDBConnector;
    private LogDB _logDBConnector;

    private string _authId;

    private LogFile _logFile = new LogFile();
    private UnitList _unitList = new UnitList();

    private GameObject _planet;
    private string _fieldName;

    public void AddTime(float amount)
    {

    }

    public void AddMoney(int earn)
    {
        
    }

    public void AddEnergy(int earn)
    {
    }

    public void LogAddUnit(IUnit data, int cost)
    {
    }

    public void LogLevelUp(int id, int cost)
    {

    }

    public void LogRemoveUnit(IUnit data, int id, int cost)
    {
        
    }

    public void SetDB(string targetAuth, string fieldName,
        FieldDataDB metaDataConnector, LogDB logDataConnector)
    {
        _fieldName = fieldName.Equals("") ? "earth" : fieldName;
        PlanetData = PlanetDataManager.Instance.
            GetPlanetData(fieldName);

            _authId = targetAuth;

        _metaDBConnector = metaDataConnector;
        _logDBConnector = logDataConnector;
        _logFile.SetDBConnector(logDataConnector);
    }

    public void ConnectDB()
    {

        _metaDBConnector.Connect(new string[2] { "metadata", _authId });

        _logDBConnector.Connect(
            new string[4] { "users", _authId, "log", _fieldName });
        
    }

    public void Dispose()
    {
        if (_planet != null)
        {
            GameObject.Destroy(_planet);
        }

        _unitList.Clear(FieldData.SpendTime);

    }

    public void LoadFieldMetaData(Action callback, Action<string> fallback)
    {
        _planet = UnityEngine.Object.
            Instantiate(PlanetData.GetPrefab());

        _metaDBConnector.GetRecordAt(_fieldName, (data) => {
            
            FieldData = new FieldData(data.SpendTime, data.Money, data.Energy);
            callback?.Invoke();

        }, (msg) => {
            fallback?.Invoke(msg);
        });
    }
    
    public void LoadLog(Action callback, Action<string> fallback)
    {
        _logFile.LoadLog(() => {
            while (_logFile.Top != null)
            {
                if (_logFile.Top.OccurrenceTime > FieldData.SpendTime)
                {
                    break;
                }
                _logFile.Do(this);
            }
            callback?.Invoke();

        }, (msg) => {
            fallback?.Invoke(msg);
        });
    }

    public IUnit GetUnit(int id)
        => _unitList.GetUnit(id);

    public void RegisterUnit(int id, IUnit unit)
        => _unitList.RegisterUnit(id, unit);

    public void UnregisterUnit(int id)
        => _unitList.UnregisterUnit(id);

}