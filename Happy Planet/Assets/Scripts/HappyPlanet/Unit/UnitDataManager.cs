using System.Collections.Generic;
using EasyH;
using EasyH.Unity;

public class UnitDataManager : Singleton<UnitDataManager>
{
    public UnitData GetUnitData(string str) => _dic[str];

    IDictionary<string, UnitData> _dic;

    protected override void OnCreate()
    {
        _dic = new Dictionary<string, UnitData>();

        IDictionaryConnector<string, string> connector
            = new JsonDictionaryConnector<string, string>();

        IDictionary<string, string> dic = connector.ReadData("UnitInfor");

        foreach (var value in dic) {
            _dic.Add(value.Key, ResourceManager.Instance.
                ResourceConnector.Import<UnitData>(value.Value));
        }
    }

}
