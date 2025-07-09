using System;
using UnityEngine;

using FieldDataDB = EHTool.DBKit.IDatabaseConnector<string, FieldDataRecord>;
using LogDB = EHTool.DBKit.IDatabaseConnector<int, Log>;

public class DefaultField : IField {

    public PlanetData PlanetData { get; private set; }
         = FieldManager.Instance.DefaultFieldData();
    public FieldData FieldData { get; private set; } = new FieldData(1.36f, 0, 10);

    GameObject _planet;

    public void AddEnergy(int earn)
    {
        
    }

    public void AddMoney(int earn)
    {
    }

    public void LogAddUnit(IUnit data, int cost)
    {
    }

    public void ConnectDB(string targetAuth, string fieldName,
        FieldDataDB metaDataConnector, LogDB logDataConnector)
    {
        PlanetData = FieldManager.Instance.GetFieldData(fieldName);
    }

    public void Dispose()
    {
        if (_planet != null)
        {
            GameObject.Destroy(_planet);
        }

    }

    public void LoadLog(Action callback, Action<string> fallback)
    {
        callback?.Invoke();
    }

    public void LoadFieldMetaData(Action callback, Action<string> fallback)
    {
        _planet = FieldManager.Instance.InitPlanet(PlanetData.GetPrefab());
        callback?.Invoke();
    }

    public IUnit GetUnit(int id)
    {
        return null;
    }

    public void RegisterUnit(int id, IUnit unit)
    {
    }

    public void LogLevelUp(int id, int cost)
    {

    }

    public void LogRemoveUnit(IUnit data, int id, int cost)
    {
    }

    public void AddTime(float amount)
    {
    }

    public void UnregisterUnit(int id)
    {
    }
}