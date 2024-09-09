#if !UNITY_WEBGL || UNITY_EDITOR
using System.Collections.Generic;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System.Linq;
using EHTool.DBKit;

public class FirestoreConnector<T> : IDatabaseConnector<T> where T : IDictionaryable<T> {

    DocumentReference docRef;

    ISet<CallbackMethod<IList<T>>> _allListener;
    IDictionary<CallbackMethod<T>, ISet<int>> _recordListener;

    bool _databaseExist = false;

    public bool IsDatabaseExist()
    {
        return _databaseExist;
    }

    public void Connect(string authName, string databaseName)
    {
        docRef = FirebaseFirestore.DefaultInstance.Collection(databaseName).Document(authName);

        _allListener = new HashSet<CallbackMethod<IList<T>>>();
        _recordListener = new Dictionary<CallbackMethod<T>, ISet<int>>();
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
            { idx.ToString(), record.ToDictionary() },
        };

        if (!_databaseExist)
        {
            docRef.SetAsync(updates);
            _databaseExist = true;
            return;

        }

        updates.Add((idx + 1).ToString(), FieldValue.Delete);
        docRef.UpdateAsync(updates);
    }

    public void GetAllRecord(CallbackMethod<IList<T>> callback)
    {
        if (_allListener.Count > 0)
        {
            _allListener.Add(callback);
            return;
        }

        _allListener.Add(callback);

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;

            _databaseExist = snapshot.Exists;

            if (snapshot.Exists)
            {
                List<T> data = new List<T>();

                int beforeIdx = 0;

                foreach (KeyValuePair<string, object> json in snapshot.ToDictionary().OrderBy(x => int.Parse(x.Key))) {

                    if (int.Parse(json.Key) != beforeIdx++) break;

                    T temp = default;
                    temp.SetValueFromDictionary(json.Value as Dictionary<string, object>);

                    data.Add(temp);

                }

                foreach (CallbackMethod<IList<T>> cb in _allListener)
                {
                    cb(data);
                }

                _allListener = new HashSet<CallbackMethod<IList<T>>>();
            }
            else
            {
                List<T> data = new List<T>();
                foreach (CallbackMethod<IList<T>> cb in _allListener)
                {
                    cb(data);
                }
                
                Debug.Log(string.Format("Document {0} does not exist!", snapshot.Id));
            }
        });
    }

    // GetRecordAll에서 모든 레코드 받아오면 거기서 원하는걸 찾아오는 방식임
    // 비효율적인 방식이지만 이 게임에서 이걸 사용하는 건 하나밖에 없어서(GameManagerData인데 이것도 Firestore 안쓸 예정) 일단은 이렇게 둠
    public void GetRecordAt(CallbackMethod<T> callback, CallbackMethod fallback, int idx)
    {
        if (!_recordListener.ContainsKey(callback))
        {
            _recordListener.Add(callback, new HashSet<int>());
        }

        _recordListener[callback].Add(idx);

        CallbackMethod<IList<T>> thisCallback = (IList<T> data) =>
        {
            if (idx < data.Count - 1) {
                callback?.Invoke(data[idx]);
                return;
            }

            fallback?.Invoke();
        };

        if (_allListener.Count > 0)
        {
            _allListener.Add(thisCallback);
            return;
        }
        GetAllRecord(thisCallback);
    }

}
#endif