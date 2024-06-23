using System.Collections.Generic;

public interface IDatabaseConnectorRecordListener<T> {
    public void Callback(T data);
}

public interface IDatabaseConnectorAllListener<T> {
    public void Callback(IList<T> data);

}


public interface IDatabaseConnector<T> : IDatabaseConnectorAllListener<T> {

    public void Connect(string databaseName);
    public bool IsDatabaseExist();
    public void AddRecord(T Record);
    public void GetRecordAt(IDatabaseConnectorRecordListener<T> callback, int idx);
    public void UpdateRecordAt(T Record, int idx);
    public void GetAllRecord(IDatabaseConnectorAllListener<T> callback);

}