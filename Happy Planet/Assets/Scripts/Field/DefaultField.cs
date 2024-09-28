using EHTool.DBKit;
using System.Drawing;
using UnityEngine;

public class DefaultField : IField {

    FieldData _fieldData;
    GameObject _planet;

    public float Size => 3.5f;
    public int Money => 0;
    public int Energy => 100;
    public float SpendTime => 1.36f;
    public int Day => 0;
    public float MaxSpeed {
        get {
            return 1f;
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

    public int CompareTime(float time)
    {
        return 0;
    }

    public int CompareTime(float time, float time2)
    {
        return 0;
    }

    public void ConnectDB(string targetAuth, string fieldName, IDatabaseConnector<IField.FieldMetaData> metaDataConnector, IDatabaseConnector<Log> logDataConnector)
    {
        _fieldData = FieldManager.Instance.GetFieldData(fieldName);
    }

    public void Dispose()
    {
        if (_planet != null)
        {
            Object.Destroy(_planet);
        }

    }

    public void FieldLogDataRead(CallbackMethod callback, CallbackMethod<string> fallback)
    {
        callback();
    }

    public void FieldMetaDataRead(CallbackMethod callback, CallbackMethod<string> fallback)
    {
        _planet = FieldManager.Instance.InitPlanet(_fieldData.GetPlanetPrefab());
        callback();
    }

    public IUnit GetUnit(int id)
    {
        return null;
    }

    public void LevelUp(int id, int cost)
    {

    }

    public void RegisterUnit(int id, IUnit unit)
    {
    }

    public void RemoveUnit(IUnit data, int id, int cost)
    {
    }

    public void TimeAdd(float amount)
    {
    }

    public void UnregisterUnit(int id)
    {
    }
}