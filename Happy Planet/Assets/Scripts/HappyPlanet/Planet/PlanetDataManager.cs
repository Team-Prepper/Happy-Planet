using System.Collections.Generic;
using EasyH;
using EasyH.Unity;

public class PlanetDataManager : Singleton<PlanetDataManager>
{
    private IDictionary<string, PlanetData> _dic;

    protected override void OnCreate()
    {
        _dic = new Dictionary<string, PlanetData>();

        IDictionaryConnector<string, string> connector =
            new JsonDictionaryConnector<string, string>();
        IDictionary<string, string> dic = connector.ReadData("FieldInfor");

        foreach (var value in dic)
        {
            _dic.Add(value.Key, ResourceManager.Instance.
                ResourceConnector.Import<PlanetData>(value.Value));
        }
    }

    public PlanetData DefaultPlanetData()
    {
        return GetPlanetData("");
    }

    public PlanetData GetPlanetData(string fieldName)
    {
        return _dic[fieldName];
    }
    
}
