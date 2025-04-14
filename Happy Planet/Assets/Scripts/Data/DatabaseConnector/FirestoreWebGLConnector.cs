using EHTool;
using EHTool.DBKit;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

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

public class FirestoreWebGLConnector<T> : MonoBehaviour, IDatabaseConnector<T> where T : struct, IDictionaryable<T> {

    private static bool _isConnect = false;

    private Action<IList<T>> _allListener;
    private Action<string> _fallbackListener;

    private string _dbName;
    private string _authName;

    private bool _dbExist;

    public bool IsDatabaseExist()
    {
        return false;
    }

    public void Connect(string authName, string databaseName)
    {
        _authName = authName;
        _dbName = databaseName;

        _allListener = null;
        _fallbackListener = null;

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

    public void GetAllRecord(Action<IList<T>> callback, Action<string> fallback)
    {
        if (!_isConnect) return;

        if (_allListener != null)
        {
            _allListener += callback;
            _fallbackListener += fallback;
            return;
        }

        _allListener += callback;
        _fallbackListener += fallback;

        FirestoreWebGLBridge.FirestoreGetAllRecord(_dbName, _authName, gameObject.name, "FSGetAllRecordCallback", "FSGetAllRecordFallback");
    }

    public void FSGetAllRecordCallback(string value) {

        _dbExist = true;

        Dictionary<string, object> snapshot = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);

        List<T> data = new List<T>();

        int expectIdx = 0;

        foreach (KeyValuePair<string, object> child in snapshot.OrderBy(x => int.Parse(x.Key)))
        {
            if (!int.TryParse(child.Key, out int idx)) continue;
            if (idx < 0) continue;
            if (idx != expectIdx++) break;

            T temp = default;
            temp.SetValueFromDictionary(JsonConvert.DeserializeObject<Dictionary<string, object>>(child.Value.ToString()));

            data.Add(temp);
        }


        _allListener?.Invoke(data);

        _allListener = null;
        _fallbackListener = null;
    }

    public void FSGetAllRecordFallback()
    {
        _dbExist = false;

        _fallbackListener?.Invoke("Network Error");

        _allListener = null;
        _fallbackListener = null;

    }

    struct IdxAndFallback {
        public int idx;
        public Action<string> fallback;

        public IdxAndFallback(int idx, Action<string> fallback) {
            this.idx = idx;
            this.fallback = fallback;
        }
    }

    public void GetRecordAt(Action<T> callback, Action<string> fallback, int idx)
    {
        if (!_isConnect) return;

        Action<IList<T>> thisCallback = (IList<T> data) =>
        {
            if (idx < data.Count)
            {
                callback?.Invoke(data[idx]);
                return;
            }

            fallback?.Invoke("No Idx");
        };

        GetAllRecord(thisCallback, fallback);
    }

}