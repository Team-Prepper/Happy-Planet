using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

namespace EHTool {

    public class XMLDictionaryReader<K, V> : IDictionaryConnector<K, V> {

        class XMLKeyValue : XMLNodeReader {
            public K key;
            public V value;

            public void Read(XmlNode node)
            {
                key = (K)Convert.ChangeType(node.Attributes["key"].Value, typeof(K));
                value = (V)Convert.ChangeType(node.Attributes["value"].Value, typeof(V));
            }
        }

        public XMLDictionaryReader()
        {

        }

        public string GetDefaultPath() => "XML";
        public string GetExtensionName() => ".xml";

        public IDictionary<K, V> ReadData(string path)
        {
            XmlDocument xmlDoc = AssetOpener.ReadXML(path);

            IDictionary<K, V> retval = new Dictionary<K, V>();

            if (xmlDoc == null) return retval;

            XmlNodeList nodes = xmlDoc.SelectNodes("List/Element");

            for (int i = 0; i < nodes.Count; i++)
            {
                XMLKeyValue xmlData = new XMLKeyValue();
                xmlData.Read(nodes[i]);

                retval.Add(xmlData.key, xmlData.value);
            }

            return retval;

        }

        public void Save(IDictionary<K, V> data, string path)
        {
#if !UNITY_EDITOR
            return;
#endif

            XmlDocument xmlDoc = new XmlDocument();

            XmlNode root = xmlDoc.CreateElement("List");
            xmlDoc.AppendChild(root);

            //√‚√≥: https://blog.naver.com/kmc7468/220660088517
            foreach (KeyValuePair<K, V> obj in data)
            {
                XmlNode node = xmlDoc.CreateElement("Element");

                XmlAttribute key = xmlDoc.CreateAttribute("key");
                key.Value = obj.Key.ToString();
                XmlAttribute value = xmlDoc.CreateAttribute("value");
                value.Value = obj.Value.ToString();

                node.Attributes.SetNamedItem(key);
                node.Attributes.SetNamedItem(value);
                root.AppendChild(node);
            }

            if (!Directory.Exists("Assets/Resources/XML"))
            {
                Directory.CreateDirectory("Assets/Resources/XML");
            }

            xmlDoc.Save(string.Format("Assets/Resources/XML/{0}.xml", path));
        }
    }
}