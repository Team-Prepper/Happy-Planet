using EasyH.Tool.DBKit;
using System;

using FieldDataDB = EasyH.Tool.DBKit.IDatabaseConnector<string, FieldDataRecord>;
using LogDB = EasyH.Tool.DBKit.IDatabaseConnector<int, Log>;

public interface IField {

    public PlanetData PlanetData { get; }
    public FieldData FieldData { get; }

    public void AddTime(float amount);
    
    public void AddMoney(int earn);
    public void AddEnergy(int earn);

    public void SetDB(string targetAuth, string fieldName,
        FieldDataDB metaDataConnector, LogDB logDataConnector);
    public void ConnectDB();
    public void Dispose();

    public void LogAddUnit(IUnit data, int cost);
    public void LogLevelUp(int id, int cost);
    public void LogRemoveUnit(IUnit data, int id, int cost);

    public void LoadFieldMetaData(Action callback, Action<string> fallback);
    public void LoadLog(Action callback, Action<string> fallback);

    public void RegisterUnit(int id, IUnit unit);
    public void UnregisterUnit(int id);
    public IUnit GetUnit(int id);

}