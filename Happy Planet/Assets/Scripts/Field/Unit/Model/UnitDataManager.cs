using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using EHTool;

public class UnitDataManager : Singleton<UnitDataManager>
{
    internal class UnitData {
        internal string name;
        internal string path;

        public void Read(XmlNode node)
        {
            name = node.Attributes["name"].Value;
            path = node.Attributes["path"].Value;
        }
    }

    public UnitInfor GetUnitData(string str) => _dic[str];

    IDictionary<string, UnitInfor> _dic;

    protected override void OnCreate()
    {
        _dic = new Dictionary<string, UnitInfor>();
        XmlDocument xmlDoc = AssetOpener.ReadXML("UnitInfor");

        XmlNodeList nodes = xmlDoc.SelectNodes("UnitData/Unit");

        for (int i = 0; i < nodes.Count; i++)
        {
            UnitData unitData = new UnitData();
            unitData.Read(nodes[i]);
            UnitInfor infor = AssetOpener.Import<UnitInfor>(unitData.path);
            _dic.Add(infor.UnitCode, infor);
        }
    }

}
