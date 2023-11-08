using System.Collections.Generic;
using System.Xml;
using ObserberPattern;

namespace LangSystem {

    public interface IStringListener {
        public void SetText(string key);

    }

    public class StringManager : Singleton<StringManager>, ISubject{
        // Start is called before the first frame update

        List<IObserver> _targets;

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

        class StringData {
            public string key;
            public string value;

            public void Read(XmlNode node)
            {
                key = node.Attributes["key"].Value;
                value = node.Attributes["value"].Value;
            }
        }

        Dictionary<string, string> _dic;

        string _nowLang = "Korean";

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