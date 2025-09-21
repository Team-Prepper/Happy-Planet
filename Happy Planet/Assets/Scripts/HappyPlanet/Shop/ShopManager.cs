using System.Collections.Generic;
using EasyH;

public class ShopDataManager : Singleton<ShopDataManager> {

    public IDictionary<string, string[]> _dict;

    protected override void OnCreate()
    {
        IDictionaryConnector<string, string[]> connector =
            new JsonDictionaryConnector<string, string[]>();

        _dict = connector.ReadData("ShopInfor");
    }

    public string[] GetShopItem(string v)
    {
        if (!_dict.ContainsKey(v)) {
            return new string[0];
        }

        return _dict[v];
    }
}
