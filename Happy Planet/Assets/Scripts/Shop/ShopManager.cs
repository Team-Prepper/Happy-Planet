using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ShopManager : MonoSingleton<ShopManager> {

    public List<GUIShopUnit> shops = new List<GUIShopUnit>();

    public class ShopData {
        internal string unitCode;
        internal int level;
        internal int price;

        public void Read(XmlNode node)
        {
            unitCode = node.Attributes["unitCode"].Value;
            level = int.Parse(node.Attributes["level"].Value);
            price = int.Parse(node.Attributes["price"].Value);
        }
    }

    public List<ShopData> _list;

    protected override void OnCreate()
    {
        _list = new List<ShopData>();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load("Assets/XML/ShopInfor.xml");

        XmlNodeList nodes = xmlDoc.SelectNodes("ShopData/ShopUnit");

        for (int i = 0; i < nodes.Count; i++)
        {
            ShopData shopData = new ShopData();
            shopData.Read(nodes[i]);

            _list.Add(shopData);
        }
    }
}
