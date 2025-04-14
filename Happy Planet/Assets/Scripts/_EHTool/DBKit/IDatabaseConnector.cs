using System;
using System.Collections.Generic;

namespace EHTool.DBKit {

    public interface IDictionaryable<T> {

        public IDictionary<string, object> ToDictionary();
        public void SetValueFromDictionary(IDictionary<string, object> value);

    }

    public interface IDatabaseConnector<T> where T : IDictionaryable<T> {

        public void Connect(string authName, string databaseName);
        public bool IsDatabaseExist();
        public void AddRecord(T Record);
        public void GetRecordAt(Action<T> callback, Action<string> fallback, int idx);
        public void UpdateRecordAt(T Record, int idx);
        public void GetAllRecord(Action<IList<T>> callback, Action<string> fallback);

    }
}