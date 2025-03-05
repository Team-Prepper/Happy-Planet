using EHTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : Singleton<FieldManager> {

    private IDictionary<string, IField> _fields;

    IField _lastPlayerField = null;

    IDictionary<string, PlanetData> _dic;

    protected override void OnCreate()
    {
        _fields = new Dictionary<string, IField>();

        _dic = new Dictionary<string, PlanetData>();

        IDictionaryConnector<string, string> connector = new JsonDictionaryConnector<string, string>();
        IDictionary<string, string> dic = connector.ReadData("FieldInfor");

        foreach (var value in dic)
        {
            _dic.Add(value.Key, AssetOpener.Import<PlanetData>(value.Value));
        }
    }

    public PlanetData GetFieldData(string fieldName) {
        return _dic[fieldName];
    }

    public void AddFieldSet(string fieldName, IField field)
    {
        _lastPlayerField = field;
        _fields[fieldName] = field;
    }

    public IField GetLastPlayerField() { return _lastPlayerField; }

    public bool FieldExist(string fieldName, out IField field) {
        if (_fields.ContainsKey(fieldName)) {
            field = _fields[fieldName];
            return true;
        }
        field = null;
        return false;
    }

    public GameObject InitPlanet(GameObject prefab)
    {
        return Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
    }
}
