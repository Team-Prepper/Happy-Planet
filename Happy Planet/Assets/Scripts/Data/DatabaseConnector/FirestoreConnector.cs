using System.Collections.Generic;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System;
using System.Linq;

public class FirestoreConnector<T> : IDatabaseConnector<T> {

    DocumentReference docRef;

    ISet<IDatabaseConnectorAllListener<T>> _allListener;
    IDictionary<IDatabaseConnectorRecordListener<T>, ISet<int>> _recordListener;

    public bool IsDatabaseExist()
    {
        return false;
    }

    public void Connect(string databaseName)
    {
        docRef = FirebaseFirestore.DefaultInstance.Collection(databaseName).Document("Test");

        _allListener = new HashSet<IDatabaseConnectorAllListener<T>>();
        _recordListener = new Dictionary<IDatabaseConnectorRecordListener<T>, ISet<int>>();
    }

    public void AddRecord(T record)
    {

        // 나중에 수정 필요
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "0" , record }
        };

        docRef.UpdateAsync(updates).ContinueWithOnMainThread(task => {
            Debug.Log("AddRecord");
        });
    }

    public void UpdateRecordAt(T record, int idx)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { idx.ToString(), JsonUtility.ToJson(record) }
        };

        docRef.UpdateAsync(updates).ContinueWithOnMainThread(task => {
            Debug.Log("UpdateRecord");
        });
    }

    public void GetAllRecord(IDatabaseConnectorAllListener<T> callback)
    {
        if (_allListener.Count > 0)
        {
            _allListener.Add(this);
            return;
        }

        _allListener.Add(callback);

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;

            if (snapshot.Exists)
            {
                List<object> jsonList = snapshot.ToDictionary().Values.ToList();
                List<T> data = new List<T>();

                foreach (object json in jsonList) {
                    data.Add(JsonUtility.FromJson<T>(json.ToString()));
                }

                foreach (IDatabaseConnectorAllListener<T> cb in _allListener)
                {
                    cb.Callback(data);
                }

                _allListener = new HashSet<IDatabaseConnectorAllListener<T>>();
            }
            else
            {
                Debug.Log(string.Format("Document {0} does not exist!", snapshot.Id));
            }
        });
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
        foreach (KeyValuePair<IDatabaseConnectorRecordListener<T>, ISet<int>> callback in _recordListener)
        {
            foreach (int idx in callback.Value)
            {
                callback.Key.Callback(data[idx]);

            }
        }

        _recordListener = new Dictionary<IDatabaseConnectorRecordListener<T>, ISet<int>>();
    }
}