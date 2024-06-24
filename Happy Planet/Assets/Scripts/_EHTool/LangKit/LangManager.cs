using System;
using System.Collections.Generic;
using System.Xml;

namespace EHTool.LangKit {

    public interface IEHText : IObserver<IEHLangManager> {
        public void SetText(string key);

    }

    public interface IEHLangManager { 
        
    }

    public class LangManager : Singleton<LangManager>, IEHLangManager, IObservable<IEHLangManager> {
        
        private readonly ISet<IObserver<IEHLangManager>> _observers = new HashSet<IObserver<IEHLangManager>>();

        public IDisposable Subscribe(IObserver<IEHLangManager> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);

                observer.OnNext(this);
            }

            return new Unsubscriber<IEHLangManager>(_observers, observer);
        }

        private void _NotifyToObserver()
        {
            foreach (IObserver<IEHLangManager> target in _observers) {
                target.OnNext(this);
            } 
        }

        class StringData : XMLNodeReader {
            public string key;
            public string value;

            public void Read(XmlNode node)
            {
                key = node.Attributes["key"].Value;
                value = node.Attributes["value"].Value;
            }
        }

        IDictionary<string, string> _dic;

        string _nowLang = "Kor";

        protected override void OnCreate()
        {
            _ReadStringFromXml();
        }

        private void _ReadStringFromXml()
        {

            _dic = new Dictionary<string, string>();
            XmlDocument xmlDoc = AssetOpener.ReadXML("String/" + _nowLang);

            XmlNodeList nodes = xmlDoc.SelectNodes("List/Element");

            for (int i = 0; i < nodes.Count; i++)
            {
                StringData stringData = new StringData();
                stringData.Read(nodes[i]);

                _dic.Add(stringData.key, stringData.value);
            }

        }

        public void UpdateData()
        {
            _ReadStringFromXml();
            _NotifyToObserver();

        }
        public void ChangeLang(string lang)
        {
            _nowLang = lang;
            UpdateData();
        }

        public string GetStringByKey(string key)
        {

            if (_dic.TryGetValue(key, out string value))
            {
                return value;
            }
            return key;

        }
    }

}