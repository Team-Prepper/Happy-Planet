using System;
using System.Collections.Generic;

public interface IDatabaseConnector<T> {

    public void Connect(string databaseName);
    public bool IsDatabaseExist();
    public void AddRecord(T Record);
    public void GetRecordAt(CallbackMethod<T> callback, CallbackMethod fallback, int idx);
    public void UpdateRecordAt(T Record, int idx);
    public void GetAllRecord(CallbackMethod<IList<T>> callback);

}