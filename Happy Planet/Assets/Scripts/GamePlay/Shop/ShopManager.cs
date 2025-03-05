using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using EHTool;
using System;

public class ShopManager : MonoSingleton<ShopManager> {

    public IDictionary<string, string[]> _dict;

    protected override void OnCreate()
    {
        IDictionaryConnector<string, string[]> connector =
            new JsonDictionaryConnector<string, string[]>();

        _dict = connector.ReadData("ShopInfor");
    }

    internal string[] GetShopItem(string v)
    {
        if (!_dict.ContainsKey(v)) {
            return new string[0];
        }

        return _dict[v];
    }
}
