using UnityEngine;
using System;

using FieldDataDB = EHTool.DBKit.IDatabaseConnector<FieldDataRecord>;
using LogDB = EHTool.DBKit.IDatabaseConnector<Log>;

public class TourField : IField {

    public PlanetData PlanetData { get; private set; }
    public FieldData FieldData { get; private set; } = FieldData.Default;
    private FieldDataDB _metaDBConnector;

    private LogFile _logFile = new LogFile();
    private UnitList _unitList = new UnitList();

    private GameObject _planet;

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

    public void ConnectDB(string targetAuth, string fieldName,
        FieldDataDB metaDataConnector, LogDB logDataConnector)
    {
        PlanetData = FieldManager.Instance.GetFieldData(fieldName);

        metaDataConnector.Connect(targetAuth, string.Format("MetaData{0}", fieldName));
        logDataConnector.Connect(targetAuth, string.Format("LogData{0}", fieldName));

        _metaDBConnector = metaDataConnector;
        _logFile.SetDBConnector(logDataConnector);
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
        _planet = FieldManager.Instance.InitPlanet(PlanetData.GetPrefab());

        _metaDBConnector.GetRecordAt((data) => {
            
            FieldData = new FieldData(data.SpendTime, data.Money, data.Energy);
            callback?.Invoke();

        }, (msg) => {
            fallback?.Invoke(msg);
        }, 0);
    }
    
    public void LoadLog(Action callback, Action<string> fallback)
    {
        _logFile.LoadLog(() => {
            while (_logFile.Top != null)
            {
                if (_logFile.Top.Value.OccurrenceTime > FieldData.SpendTime)
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