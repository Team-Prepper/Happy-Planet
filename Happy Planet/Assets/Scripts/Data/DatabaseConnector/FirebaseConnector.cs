#if !UNITY_WEBGL || UNITY_EDITOR
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using System.Linq;
using EHTool.DBKit;
using Google.MiniJSON;

public class FirebaseConnector<T> : IDatabaseConnector<T> where T : IDictionaryable<T> {

    DatabaseReference docRef;

    ISet<CallbackMethod<IList<T>>> _allListener;
    ISet<CallbackMethod<string>> _fallbackListener;

    IDictionary<CallbackMethod<T>, ISet<int>> _recordListener;

    bool _databaseExist = false;

    public bool IsDatabaseExist()
    {
        return _databaseExist;
    }

    public void Connect(string authName, string databaseName)
    {
        docRef = FirebaseDatabase.DefaultInstance.RootReference.Child(databaseName).Child(authName);

        _allListener = new HashSet<CallbackMethod<IList<T>>>();
        _fallbackListener = new HashSet<CallbackMethod<string>>();

        _recordListener = new Dictionary<CallbackMethod<T>, ISet<int>>();
    }

    public void AddRecord(T record)
    {

        // 나중에 수정 필요
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "0" , record }
        };

        docRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {
            Debug.Log("AddRecord");
        });
    }

    public void UpdateRecordAt(T record, int idx)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { idx.ToString(), record.ToDictionary() },
        };

        updates.Add((idx + 1).ToString(), null);
        docRef.UpdateChildrenAsync(updates);

        if (!_databaseExist)
        {
            _databaseExist = true;

        }
    }

    public void GetAllRecord(CallbackMethod<IList<T>> callback, CallbackMethod<string> fallback)
    {
        if (_allListener.Count > 0)
        {
            _allListener.Add(callback);
            _fallbackListener.Add(fallback);
            return;
        }

        _allListener.Add(callback);
        _fallbackListener.Add(fallback);

        docRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {

            DataSnapshot snapshot = task.Result;

            _databaseExist = snapshot.Exists;

            if (snapshot.Exists)
            {

                List<T> data = new List<T>();


                int beforeIdx = 0;

                foreach (DataSnapshot child in snapshot.Children)
                {

                    if (int.Parse(child.Key) != beforeIdx++) break;

                    Dictionary<string, object> tmp = child.Value as Dictionary<string, object>;

                    T temp = default;
                    temp.SetValueFromDictionary(tmp);

                    data.Add(temp);

                }

                foreach (CallbackMethod<IList<T>> cb in _allListener)
                {
                    cb?.Invoke(data);
                }
            }
            else
            {

                foreach (CallbackMethod<string> cb in _fallbackListener)
                {
                    cb?.Invoke(string.Format("Document {0} does not exist!", snapshot.Key.ToString()));
                }

            }

            _allListener = new HashSet<CallbackMethod<IList<T>>>();
            _fallbackListener = new HashSet<CallbackMethod<string>>();

        });
    }

    // GetRecordAll에서 모든 레코드 받아오면 거기서 원하는걸 찾아오는 방식임
    // 비효율적인 방식이지만 이 게임에서 이걸 사용하는 건 하나밖에 없어서(GameManagerData인데 이것도 Firebase 안쓸 예정) 일단은 이렇게 둠
    public void GetRecordAt(CallbackMethod<T> callback, CallbackMethod<string> fallback, int idx)
    {
        if (!_recordListener.ContainsKey(callback))
        {
            _recordListener.Add(callback, new HashSet<int>());
        }

        _recordListener[callback].Add(idx);

        CallbackMethod<IList<T>> thisCallback = (IList<T> data) =>
        {
            if (idx < data.Count) {
                callback?.Invoke(data[idx]);
                return;
            }

            fallback?.Invoke("No Idx");
        };

        if (_allListener.Count > 0)
        {
            _allListener.Add(thisCallback);
            return;
        }

        GetAllRecord(thisCallback, fallback);

    }

}
#endif