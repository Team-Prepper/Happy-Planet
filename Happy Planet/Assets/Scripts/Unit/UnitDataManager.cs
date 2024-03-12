using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class UnitDataManager : MonoSingleton<UnitDataManager>
{
    internal class UnitData {
        internal string name;
        internal string stringKey;
        internal string img;
        internal string prefabPath;

        public void Read(XmlNode node)
        {
            name = node.Attributes["name"].Value;
            stringKey = node.Attributes["stringKey"].Value;
            img = node.Attributes["img"].Value;
            prefabPath = "Assets/Prefabs/Objects/" + node.Attributes["prefab"].Value;
        }
    }

    public UnitInfor GetUnitData(string str) => _dic[str];

    Dictionary<string, UnitInfor> _dic;

    protected override void OnCreate()
    {
        _dic = new Dictionary<string, UnitInfor>();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load("Assets/XML/UnitInfor.xml");

        XmlNodeList nodes = xmlDoc.SelectNodes("UnitData/Unit");

        for (int i = 0; i < nodes.Count; i++)
        {
            UnitData unitData = new UnitData();
            unitData.Read(nodes[i]);

            //_dic.Add(unitData.name, unitData);
        }
    }

}
