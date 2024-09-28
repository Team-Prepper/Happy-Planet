using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using EHTool;

public class UnitDataManager : Singleton<UnitDataManager>
{
    internal class UnitInfor {
        internal string name;
        internal string path;

        public void Read(XmlNode node)
        {
            name = node.Attributes["name"].Value;
            path = node.Attributes["path"].Value;
        }
    }

    public UnitData GetUnitData(string str) => _dic[str];

    IDictionary<string, UnitData> _dic;

    protected override void OnCreate()
    {
        _dic = new Dictionary<string, UnitData>();
        XmlDocument xmlDoc = AssetOpener.ReadXML("UnitInfor");

        XmlNodeList nodes = xmlDoc.SelectNodes("UnitData/Unit");

        for (int i = 0; i < nodes.Count; i++)
        {
            UnitInfor unitData = new UnitInfor();
            unitData.Read(nodes[i]);
            global::UnitData infor = AssetOpener.Import<global::UnitData>(unitData.path);
            _dic.Add(infor.UnitCode, infor);
        }
    }

}
