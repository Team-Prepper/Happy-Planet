#if !UNITY_WEBGL || UNITY_EDITOR
using EHTool.DBKit;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class FirestoreConnector<K, T> : IDatabaseConnector<K, T> where T : struct, IDictionaryable<T>
{

    DocumentReference docRef;

    Action<IDictionary<K, T>> _allListener;
    Action<string> _fallbackListener;

    bool _databaseExist = false;

    public bool IsDatabaseExist()
    {
        return _databaseExist;
    }

    public void Connect(string authName, string databaseName)
    {
        docRef = FirebaseFirestore.DefaultInstance.Collection(authName).Document(databaseName);

        _allListener = null;
        _fallbackListener = null;
    }

    public void AddRecord(T record)
    {

        // 추후 수정 필요
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "0" , record }
        };

        docRef.UpdateAsync(updates).ContinueWithOnMainThread(task =>
        {
            Debug.Log("AddRecord");
        });
    }

    public void UpdateRecordAt(K idx, T record)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { idx.ToString(), record.ToDictionary() },
        };

        if (!_databaseExist)
        {
            docRef.SetAsync(updates);
            _databaseExist = true;
            return;

        }

        //updates.Add((idx + 1).ToString(), FieldValue.Delete);
        docRef.UpdateAsync(updates);
    }

    public void GetAllRecord(Action<IDictionary<K, T>> callback, Action<string> fallback)
    {
        if (_allListener != null)
        {
            _allListener += callback;
            _fallbackListener += fallback;
            return;
        }

        _allListener += callback;
        _fallbackListener += fallback;

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;

            _databaseExist = snapshot.Exists;

            if (snapshot.Exists)
            {
                IDictionary<string, object> snapshotData = snapshot.ToDictionary();

                IDictionary<K, T> data = new Dictionary<K, T>();

                foreach (var tuple in snapshotData)
                {
                    K key = (K)Convert.ChangeType(tuple.Key, typeof(K));
                    T value = Activator.CreateInstance<T>();

                    if (tuple.Value is IDictionary<string, object> childDataDictionary)
                    {
                        value.SetValueFromDictionary(childDataDictionary);
                    }

                    data.Add(key, value);
                }
                _allListener?.Invoke(data);
                
            }
            else
            {
                _fallbackListener?.Invoke(string.Format("Document {0} does not exist!", snapshot.Id));
            }

            _allListener = null;
            _fallbackListener = null;
        });
    }

    // GetRecordAll에서 모든 레코드 받아오면 거기서 원하는걸 찾아오는 방식임
    // 비효율적인 방식이지만 이 게임에서 이걸 사용하는 건 하나밖에 없어서(GameManagerData인데 이것도 Firestore 안쓸 예정) 일단은 이렇게 둠
    public void GetRecordAt(K idx, Action<T> callback, Action<string> fallback)
    {

        Action<IDictionary<K, T>> thisCallback = (IDictionary<K, T> data) =>
        {
            if (data.ContainsKey(idx))
            {
                callback?.Invoke(data[idx]);
                return;
            }

            fallback?.Invoke("No Idx");
        };

        GetAllRecord(thisCallback, fallback);
    }
    public void DeleteRecordAt(K idx)
    {

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { idx.ToString(), FieldValue.Delete },
        };

        docRef.UpdateAsync(updates);
    }

}
#endif