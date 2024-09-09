using EHTool.DBKit;

public class DefaultField : IField {

    public int Money => 0;

    public int Energy => 100;

    public float SpendTime => 1.36f;

    public int GetDay => 0;

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
    }

    public void Dispose()
    {

    }

    public void FieldLogDataRead(CallbackMethod callback)
    {
        callback();
    }

    public void FieldMetaDataRead(CallbackMethod callback, CallbackMethod fallback)
    {
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