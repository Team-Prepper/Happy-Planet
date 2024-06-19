using System.Collections.Generic;
using System.Xml;

namespace EHTool.LangKit {

    public interface IEHText {
        public void SetText(string key);

    }

    public class LangManager : Singleton<LangManager>, ISubject{

        IList<IObserver> _targets;

        public void AddObserver(IObserver ops)
        {
            _targets.Add(ops);
        }

        public void RemoveObserver(IObserver ops)
        {
            _targets.Remove(ops);
        }

        public void NotifyToObserver()
        {
            foreach (IObserver target in _targets) {
                target.Notified();
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
            _targets = new List<IObserver>();
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
            NotifyToObserver();

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