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
    public static extern void FirestoreAddRecord(string path, string authName, string recordJson);

    [DllImport("__Internal")]
    public static extern void FirestoreUpdateRecordAt(string path, string authName, string recordJson, int idx);
    [DllImport("__Internal")]
    public static extern void FirestoreGetAllRecord(string path, string authName, string objectName, string callback, string fallback);
}

public class FirestoreWebGLConnector<T> : MonoBehaviour, IDatabaseConnector<T> where T : IDictionaryable<T> {

    static bool _isConnect = false;

    ISet<CallbackMethod<IList<T>>> _allListener;
    IDictionary<CallbackMethod<T>, ISet<int>> _recordListener;

    string _dbName;


    public bool IsDatabaseExist()
    {
        return false;
    }

    public void Connect(string databaseName)
    {
        _dbName = databaseName;
        _allListener = new HashSet<CallbackMethod<IList<T>>>();
        _recordListener = new Dictionary<CallbackMethod<T>, ISet<int>>();

        if (_isConnect) return;

        FirestoreWebGLBridge.FirestoreConnect("path", AssetOpener.ReadTextAsset("FirebaseConfig"));
        _isConnect = true;
    }

    public void AddRecord(T record)
    {
        if (!_isConnect) return;
        FirestoreWebGLBridge.FirestoreAddRecord(_dbName, GameManager.Instance.Auth.GetName(), JsonUtility.ToJson(record));
    }

    public void UpdateRecordAt(T record, int idx)
    {
        if (!_isConnect) return;
        Debug.Log(JsonUtility.ToJson(record));
        FirestoreWebGLBridge.FirestoreUpdateRecordAt(_dbName, GameManager.Instance.Auth.GetName(), JsonUtility.ToJson(record), idx);
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

        FirestoreWebGLBridge.FirestoreGetAllRecord(_dbName, GameManager.Instance.Auth.GetName(), gameObject.name, "GetAllRecordCallback", "GetAllRecordFallback");
    }

    public void GetAllRecordCallback(string value) {


        Debug.Log(value);

        Dictionary<string, object> snapshot = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);

        List<object> jsonList = snapshot.Values.ToList();
        List<T> data = new List<T>();

        foreach (object json in jsonList)
        {
            T temp = default;
            temp.SetValueFromDictionary(JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString()));

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

    }


    public void GetRecordAt(CallbackMethod<T> callback, CallbackMethod fallback, int idx)
    {
        if (!_isConnect) return;

        if (!_recordListener.ContainsKey(callback))
        {
            _recordListener.Add(callback, new HashSet<int>());
        }

        _recordListener[callback].Add(idx);

        if (_allListener.Count > 0)
        {
            _allListener.Add(Callback);
            return;
        }

        GetAllRecord(Callback);
    }

    public void Callback(IList<T> data)
    {
        foreach (KeyValuePair<CallbackMethod<T>, ISet<int>> callback in _recordListener)
        {
            foreach (int idx in callback.Value)
            {
                callback.Key(data[idx]);

            }
        }

        _recordListener = new Dictionary<CallbackMethod<T>, ISet<int>>();
    }
}