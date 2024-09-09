using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using EHTool.DBKit;
using System.Linq;
using EHTool;
using Newtonsoft.Json;

static class FirestoreWebGLBridge {

    [DllImport("__Internal")]
    public static extern void FirestoreConnect(string path, string firebaseConfigValue);

    [DllImport("__Internal")]
    public static extern void FirestoreAddRecord(string path, string authName, string recordJson, int idx);

    [DllImport("__Internal")]
    public static extern void FirestoreUpdateRecordAt(string path, string authName, string recordJson, int idx);
    [DllImport("__Internal")]
    public static extern void FirestoreGetAllRecord(string path, string authName, string objectName, string callback, string fallback);
}

public class FirestoreWebGLConnector<T> : MonoBehaviour, IDatabaseConnector<T> where T : IDictionaryable<T> {

    static bool _isConnect = false;

    ISet<CallbackMethod<IList<T>>> _allListener;
    IDictionary<CallbackMethod<T>, ISet<IdxAndFallback>> _recordListener;

    string _dbName;
    string _authName;

    bool _dbExist;

    public bool IsDatabaseExist()
    {
        return false;
    }

    public void Connect(string authName, string databaseName)
    {
        _authName = authName;
        _dbName = databaseName;

        _allListener = new HashSet<CallbackMethod<IList<T>>>();
        _recordListener = new Dictionary<CallbackMethod<T>, ISet<IdxAndFallback>>();

        if (_isConnect) return;

        FirestoreWebGLBridge.FirestoreConnect("path", AssetOpener.ReadTextAsset("FirebaseConfig"));
        _isConnect = true;
    }

    public void AddRecord(T record)
    {
        if (!_isConnect) return;
        FirestoreWebGLBridge.FirestoreAddRecord(_dbName, _authName, JsonUtility.ToJson(record), 0);
    }

    public void UpdateRecordAt(T record, int idx)
    {
        if (!_isConnect) return;

        if (_dbExist)
        {
            FirestoreWebGLBridge.FirestoreUpdateRecordAt(_dbName, _authName, JsonUtility.ToJson(record), idx);
            return;
        }
        FirestoreWebGLBridge.FirestoreAddRecord(_dbName, _authName, JsonUtility.ToJson(record), idx);
        _dbExist = true;
    }

    public void GetAllRecord(CallbackMethod<IList<T>> callback)
    {
        if (!_isConnect) return;

        if (_allListener.Count > 0)
        {
            _allListener.Add(callback);
            return;
        }

        _allListener.Add(callback);

        FirestoreWebGLBridge.FirestoreGetAllRecord(_dbName, _authName, gameObject.name, "GetAllRecordCallback", "GetAllRecordFallback");
    }

    public void GetAllRecordCallback(string value) {

        _dbExist = true;

        Dictionary<string, object> snapshot = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);

        List<T> data = new List<T>();

        int beforeIdx = 0;

        foreach (KeyValuePair<string, object> json in snapshot.OrderBy(x => int.Parse(x.Key)))
        {
            if (int.Parse(json.Key) != beforeIdx++) break;

            T temp = default;
            temp.SetValueFromDictionary(JsonConvert.DeserializeObject<Dictionary<string, object>>(json.Value.ToString()));

            data.Add(temp);
        }

        foreach (CallbackMethod<IList<T>> cb in _allListener)
        {
            cb(data);
        }

        _allListener = new HashSet<CallbackMethod<IList<T>>>();
    }

    public void GetAllRecordFallback()
    {
        _dbExist = false;

        List<T> data = new List<T>();

        foreach (CallbackMethod<IList<T>> cb in _allListener)
        {
            cb(data);
        }

        _allListener = new HashSet<CallbackMethod<IList<T>>>();

    }

    struct IdxAndFallback {
        public int idx;
        public CallbackMethod fallback;

        public IdxAndFallback(int idx, CallbackMethod fallback) {
            this.idx = idx;
            this.fallback = fallback;
        }
    }

    public void GetRecordAt(CallbackMethod<T> callback, CallbackMethod fallback, int idx)
    {
        if (!_isConnect) return;

        if (!_recordListener.ContainsKey(callback))
        {
            _recordListener.Add(callback, new HashSet<IdxAndFallback>());
        }

        _recordListener[callback].Add(new IdxAndFallback(idx, fallback));

        if (_allListener.Count > 0)
        {
            _allListener.Add(Callback);
            return;
        }

        GetAllRecord(Callback);
    }

    public void Callback(IList<T> data)
    {
        foreach (KeyValuePair<CallbackMethod<T>, ISet<IdxAndFallback>> callback in _recordListener)
        {
            foreach (IdxAndFallback idx in callback.Value)
            {
                if (idx.idx >= data.Count)
                {
                    idx.fallback();
                    continue;
                }

                callback.Key(data[idx.idx]);

            }
        }

        _recordListener = new Dictionary<CallbackMethod<T>, ISet<IdxAndFallback>>();
    }
}