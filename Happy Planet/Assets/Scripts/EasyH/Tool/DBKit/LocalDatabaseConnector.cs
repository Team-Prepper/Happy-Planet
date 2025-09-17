using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace EasyH.Tool.DBKit {
    
    public class LocalDatabaseConnector<K, T> : IDatabaseConnector<K, T> where T : IDictionaryable<T>
    {

        private Dictionary<K, T> _data;
        private string _path;

        private ISet<Action<IDictionary<K, T>>> _allCallback;
        private IDictionary<Action<T>, ISet<K>> _recordCallback;

        private IDictionary<Action<T>, Action<string>> _recordFallback;

        public bool IsDatabaseExist()
        {
            return File.Exists(_path);
        }

        private Dictionary<K, T> _GetDataTable()
        {

            if (_data == null)
            {
                _data = new Dictionary<K, T>();

                string json;

                if (IsDatabaseExist())
                    json = File.ReadAllText(_path);
                else
                {
                    json = "{ }";
                }

                IDictionary<K, object> snapshot =
                    JsonConvert.DeserializeObject<Dictionary<K, object>>(json);
                    
                foreach (var childSnapshot in snapshot)
                {

                    K key = (K)Convert.ChangeType(childSnapshot.Key, typeof(K));
                    T value = Activator.CreateInstance<T>();

                    if (childSnapshot.Value is IDictionary<string, object> childDataDictionary)
                    {
                        value.SetValueFromDictionary(childDataDictionary);
                    }

                    _data.Add(key, value);
                }
            }

            return _data;
        }

        public void Connect(string[] args)
        { 
            
#if UNITY_EDITOR
            _path = string.Format("{0}/{1}/{2}.json", Application.dataPath, "/Resources", string.Join("/", args));
#else
            _path = string.Format("{0}/{1}.json", Application.persistentDataPath, string.Join("/", args));
#endif
            _data = null;

            _allCallback = new HashSet<Action<IDictionary<K, T>>>();
            _recordCallback = new Dictionary<Action<T>, ISet<K>>();
            _recordFallback = new Dictionary<Action<T>, Action<string>>();
        }

        public void Connect(string authName, string databaseName)
        {
            Connect(new string[2] { databaseName, authName });
        }

        public void AddRecord(T record)
        {
            Dictionary<K, T> table = _GetDataTable();
            table.Add(default, record);

            string json = JsonConvert.SerializeObject(table);

            File.WriteAllText(_path, json);
        }

        public void UpdateRecordAt(K idx, T record)
        {
            UpdateRecord(new IDatabaseConnector<K, T>.UpdateLog[1] { new(idx, record)});
        }
        
        public void UpdateRecord(IDatabaseConnector<K, T>.UpdateLog[] updates)
        {
            Dictionary<K, T> table = _GetDataTable();

            foreach (var r in updates)
            { 
                
                if (table.ContainsKey(r.Idx))
                {
                    table[r.Idx] = r.Record;
                }
                else
                {
                    table.Add(r.Idx, r.Record);
                }
            }

            string json = JsonConvert.SerializeObject(table);

            // Get the directory path from the full file path
            string directoryPath = Path.GetDirectoryName(_path);

            // Check if the directory exists and create it if it doesn't
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            File.WriteAllText(_path, json);

        }

        public void GetAllRecord(Action<IDictionary<K, T>> callback, Action<string> fallback)
        {

            if (_allCallback.Count > 0)
            {
                _allCallback.Add(Callback);
                return;
            }

            _allCallback.Add(callback);

            IDictionary<K, T> data = _GetDataTable();

            foreach (Action<IDictionary<K, T>> cb in _allCallback)
            {
                cb(data);
            }

            _allCallback = new HashSet<Action<IDictionary<K, T>>>();
        }

        public void GetRecordAt(K idx, Action<T> callback, Action<string> fallback)
        {
            if (!_recordCallback.ContainsKey(callback))
            {
                _recordCallback.Add(callback, new HashSet<K>());
                _recordFallback.Add(callback, fallback);
            }

            _recordCallback[callback].Add(idx);
            _recordFallback[callback] = fallback;

            GetAllRecord(Callback, Fallback);
        }

        public void Callback(IDictionary<K, T> data)
        {
            foreach (KeyValuePair<Action<T>, ISet<K>> callback in _recordCallback)
            {
                foreach (K idx in callback.Value)
                {
                    if (data.ContainsKey(idx))
                        callback.Key(data[idx]);
                    else
                        _recordFallback[callback.Key]("No Idx");

                }
            }

            _recordCallback = new Dictionary<Action<T>, ISet<K>>();
            _recordFallback = new Dictionary<Action<T>, Action<string>>();
        }

        public void Fallback(string msg)
        {

            foreach (KeyValuePair<Action<T>, ISet<K>> callback in _recordCallback)
            {
                foreach (K idx in callback.Value)
                {
                    _recordFallback[callback.Key]?.Invoke(msg);

                }
            }

            _recordCallback = new Dictionary<Action<T>, ISet<K>>();
            _recordFallback = new Dictionary<Action<T>, Action<string>>();
        }
        
        public void DeleteRecordAt(K idx)
        {
            
        }
    }
}