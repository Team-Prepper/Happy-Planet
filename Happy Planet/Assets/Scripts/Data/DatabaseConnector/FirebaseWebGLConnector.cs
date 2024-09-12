using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using EHTool.DBKit;
using System.Linq;
using EHTool;
using Newtonsoft.Json;

static class FirebaseWebGLBridge {

    [DllImport("__Internal")]
    public static extern void FirebaseConnect(string path, string firebaseConfigValue);

    [DllImport("__Internal")]
    public static extern void FirebaseAddRecord(string path, string authName, string recordJson, int idx);

    [DllImport("__Internal")]
    public static extern void FirebaseUpdateRecordAt(string path, string authName, string recordJson, int idx);
    [DllImport("__Internal")]
    public static extern void FirebaseGetAllRecord(string path, string authName, string objectName, string callback, string fallback);
}

public class FirebaseWebGLConnector<T> : MonoBehaviour, IDatabaseConnector<T> where T : IDictionaryable<T> {

    static bool _isConnect = false;

    ISet<CallbackMethod<IList<T>>> _allListener;
    ISet<CallbackMethod<string>> _fallbackListener;
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
        _fallbackListener = new HashSet<CallbackMethod<string>>();

        _recordListener = new Dictionary<CallbackMethod<T>, ISet<IdxAndFallback>>();

        if (_isConnect) return;

        FirebaseWebGLBridge.FirebaseConnect("path", AssetOpener.ReadTextAsset("FirebaseConfig"));
        _isConnect = true;
    }

    public void AddRecord(T record)
    {
        if (!_isConnect) return;
        FirebaseWebGLBridge.FirebaseAddRecord(_dbName, _authName, JsonUtility.ToJson(record), 0);
    }

    public void UpdateRecordAt(T record, int idx)
    {
        if (!_isConnect) return;

        if (_dbExist)
        {
            FirebaseWebGLBridge.FirebaseUpdateRecordAt(_dbName, _authName, JsonUtility.ToJson(record), idx);
            return;
        }
        FirebaseWebGLBridge.FirebaseAddRecord(_dbName, _authName, JsonUtility.ToJson(record), idx);
        _dbExist = true;
    }

    public void GetAllRecord(CallbackMethod<IList<T>> callback, CallbackMethod<string> fallback)
    {
        if (!_isConnect) return;

        if (_allListener.Count > 0)
        {
            _allListener.Add(callback);
            _fallbackListener.Add(fallback);
            return;
        }

        _allListener.Add(callback);
        _fallbackListener.Add(fallback);


        FirebaseWebGLBridge.FirebaseGetAllRecord(_dbName, _authName, gameObject.name, "FBGetAllRecordCallback", "FBGetAllRecordFallback");
    }

    public void FBGetAllRecordCallback(string value)
    {

        _dbExist = true;

        Dictionary<string, object> snapshot = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);

        List<T> data = new List<T>();

        int beforeIdx = 0;

        foreach (KeyValuePair<string, object> json in snapshot.OrderBy(x => int.Parse(x.Key)))
        {
            if (!int.TryParse(json.Key, out int idx)) continue;
            if (idx != beforeIdx++) break;

            T temp = default;
            temp.SetValueFromDictionary(JsonConvert.DeserializeObject<Dictionary<string, object>>(json.Value.ToString()));

            data.Add(temp);
        }

        foreach (CallbackMethod<IList<T>> cb in _allListener)
        {
            cb(data);
        }

        _allListener = new HashSet<CallbackMethod<IList<T>>>();
        _fallbackListener = new HashSet<CallbackMethod<string>>();
    }

    public void FBGetAllRecordFallback()
    {
        _dbExist = false;

        List<T> data = new List<T>();

        foreach (CallbackMethod<string> cb in _fallbackListener)
        {
            cb?.Invoke("Network Error");
        }

        _allListener = new HashSet<CallbackMethod<IList<T>>>();
        _fallbackListener = new HashSet<CallbackMethod<string>>();

    }

    struct IdxAndFallback {
        public int idx;
        public CallbackMethod<string> fallback;

        public IdxAndFallback(int idx, CallbackMethod<string> fallback)
        {
            this.idx = idx;
            this.fallback = fallback;
        }
    }

    public void GetRecordAt(CallbackMethod<T> callback, CallbackMethod<string> fallback, int idx)
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

        GetAllRecord(Callback, Fallback);
    }

    public void Callback(IList<T> data)
    {
        foreach (KeyValuePair<CallbackMethod<T>, ISet<IdxAndFallback>> callback in _recordListener)
        {
            foreach (IdxAndFallback idx in callback.Value)
            {
                if (idx.idx >= data.Count)
                {
                    idx.fallback("No Idx");
                    continue;
                }

                callback.Key(data[idx.idx]);

            }
        }

        _recordListener = new Dictionary<CallbackMethod<T>, ISet<IdxAndFallback>>();
    }
    public void Fallback(string msg)
    {
        foreach (KeyValuePair<CallbackMethod<T>, ISet<IdxAndFallback>> callback in _recordListener)
        {
            foreach (IdxAndFallback idx in callback.Value)
            {
                idx.fallback(msg);

            }
        }

        _recordListener = new Dictionary<CallbackMethod<T>, ISet<IdxAndFallback>>();
    }
}