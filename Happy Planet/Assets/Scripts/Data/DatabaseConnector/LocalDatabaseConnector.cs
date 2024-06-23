using Google.MiniJSON;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 테스트 및 Firestore가 갖춰지지 않았을 때 사용되는 DB 연결자
// Firestore와 연결하더라도 캐시 DB로 사용할 수 있을듯
public class LocalDatabaseConnector<T> : IDatabaseConnector<T> {

    DataTable _data;
    private string _path;

    ISet<IDatabaseConnectorAllListener<T>> _allListener;
    IDictionary<IDatabaseConnectorRecordListener<T>, ISet<int>> _recordListener;

    class DataTable {
        public List<T> value;
    }

    public bool IsDatabaseExist()
    {
        return File.Exists(_path);
    }

    private DataTable _GetDataTable() {


        if (_data == null)
        {
            string json;

            if (IsDatabaseExist())
                json = File.ReadAllText(_path);
            else
            {
                json = "{\"value\":[]}";
            }
            _data = JsonUtility.FromJson<DataTable>(json);
        }

        return _data;
    }

    public void Connect(string databaseName)
    {
#if UNITY_EDITOR
        _path = string.Format("{0}/{1}/{2}.json", Application.dataPath, "/Resources", databaseName);
#else
        _path = string.Format("{0}/{1}".json, Application.persistentDataPath, databaseName);
#endif
        _data = null;

        _allListener = new HashSet<IDatabaseConnectorAllListener<T>>();
        _recordListener = new Dictionary<IDatabaseConnectorRecordListener<T>, ISet<int>>();
    }

    public void AddRecord(T record)
    {
        DataTable table = _GetDataTable();
        table.value.Add(record);

        string json = JsonUtility.ToJson(table, true);

        File.WriteAllText(_path, json);
    }

    public void UpdateRecordAt(T record, int idx)
    {
        DataTable table = _GetDataTable();
        int removeStartIdx = Mathf.Min(table.value.Count, idx);

        table.value.RemoveRange(removeStartIdx, table.value.Count - removeStartIdx);
        table.value.Add(record);

        string json = JsonUtility.ToJson(table, true);

        File.WriteAllText(_path, json);
    }

    public void GetAllRecord(IDatabaseConnectorAllListener<T> callback)
    {
        _allListener.Add(callback);

        IList<T> data = _GetDataTable().value;

        foreach (IDatabaseConnectorAllListener<T> cb in _allListener) {
            cb.Callback(data);
        }

        _allListener = new HashSet<IDatabaseConnectorAllListener<T>>();
    }

    public void GetRecordAt(IDatabaseConnectorRecordListener<T> callback, int idx)
    {
        if (!_recordListener.ContainsKey(callback))
        {
            _recordListener.Add(callback, new HashSet<int>());
        }

        _recordListener[callback].Add(idx);

        if (_allListener.Count > 0)
        {
            _allListener.Add(this);
            return;
        }
        _allListener.Add(this);
        GetAllRecord(this);
    }

    public void Callback(IList<T> data)
    {
        foreach (KeyValuePair<IDatabaseConnectorRecordListener<T>, ISet<int>> callback in _recordListener) {
            foreach (int idx in callback.Value)
            {
                callback.Key.Callback(data[idx]);

            }
        }

        _recordListener = new Dictionary<IDatabaseConnectorRecordListener<T>, ISet<int>>();
    }
}