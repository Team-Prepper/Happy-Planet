#if !UNITY_WEBGL || UNITY_EDITOR
using EHTool.DBKit;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseConnector<T> : IDatabaseConnector<T> where T : IDictionaryable<T> {

    DatabaseReference docRef;

    CallbackMethod<IList<T>> _allListener;
    CallbackMethod<string> _fallbackListener;

    bool _databaseExist = false;

    public bool IsDatabaseExist()
    {
        return _databaseExist;
    }

    public void Connect(string authName, string databaseName)
    {
        docRef = FirebaseDatabase.DefaultInstance.RootReference.Child(authName).Child(databaseName);

        _allListener = null;
        _fallbackListener = null;

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
            {(idx + 1).ToString(), null }
        };

        docRef.UpdateChildrenAsync(updates);

        if (!_databaseExist)
        {
            _databaseExist = true;
        }
    }

    public void GetAllRecord(CallbackMethod<IList<T>> callback, CallbackMethod<string> fallback)
    {
        if (_allListener != null)
        {
            _allListener += callback;
            _fallbackListener += fallback;
            return;
        }

        _allListener = callback;
        _fallbackListener = fallback;

        docRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {

            DataSnapshot snapshot = task.Result;

            _databaseExist = snapshot.Exists;

            if (snapshot.Exists)
            {
                List<T> data = new List<T>();

                int expectIdx = 0;

                foreach (DataSnapshot child in snapshot.Children)
                {
                    if (!int.TryParse(child.Key, out int idx)) continue;
                    if (idx < 0) continue;
                    if (idx != expectIdx++) break;

                    Dictionary<string, object> tmp = child.Value as Dictionary<string, object>;

                    T temp = default;
                    temp.SetValueFromDictionary(tmp);

                    data.Add(temp);

                }

                _allListener?.Invoke(data);
            }
            else
            {
                _fallbackListener?.Invoke(string.Format("Document {0} does not exist!", snapshot.Key.ToString()));

            }

            _allListener = null;
            _fallbackListener = null;

        });
    }

    // GetRecordAll에서 모든 레코드 받아오면 거기서 원하는걸 찾아오는 방식임
    // 비효율적인 방식이지만 이 게임에서 이걸 사용하는 건 하나밖에 없어서(GameManagerData인데 이것도 Firebase 안쓸 예정) 일단은 이렇게 둠
    public void GetRecordAt(CallbackMethod<T> callback, CallbackMethod<string> fallback, int idx)
    {

        CallbackMethod<IList<T>> thisCallback = (IList<T> data) =>
        {
            if (idx < data.Count) {
                callback?.Invoke(data[idx]);
                return;
            }

            fallback?.Invoke("No Idx");
        };

        GetAllRecord(thisCallback, fallback);

    }

}
#endif