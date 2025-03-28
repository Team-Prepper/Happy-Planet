using EHTool;
using EHTool.DBKit;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

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

public class FirebaseWebGLConnector<T> : MonoBehaviour, IDatabaseConnector<T> where T : struct, IDictionaryable<T> {

    static bool _isConnect = false;

    CallbackMethod<IList<T>> _allListener;
    CallbackMethod<string> _fallbackListener;

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

        _allListener = null;
        _fallbackListener = null;

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

        if (_allListener != null)
        {
            _allListener += callback;
            _fallbackListener += fallback;
            return;
        }

        _allListener += callback;
        _fallbackListener += fallback;


        FirebaseWebGLBridge.FirebaseGetAllRecord(_dbName, _authName, gameObject.name, "FBGetAllRecordCallback", "FBGetAllRecordFallback");
    }

    public void FBGetAllRecordCallback(string value)
    {

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

    public void FBGetAllRecordFallback()
    {
        _dbExist = false;

        _fallbackListener?.Invoke("Network Error");

        _allListener = null;
        _fallbackListener = null;

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

        CallbackMethod<IList<T>> thisCallback = (IList<T> data) =>
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