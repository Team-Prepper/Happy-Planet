using EHTool;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class FieldManager : Singleton<FieldManager> {
    internal class FieldInfor {
        internal string name;
        internal string path;

        public void Read(XmlNode node)
        {
            name = node.Attributes["name"].Value;
            path = node.Attributes["path"].Value;
        }
    }

    private IDictionary<string, IField> _fields;

    IField _lastPlayerField = null;

    IDictionary<string, FieldData> _dic;

    protected override void OnCreate()
    {
        _fields = new Dictionary<string, IField>();

        _dic = new Dictionary<string, FieldData>();

        XmlDocument xmlDoc = AssetOpener.ReadXML("FieldData");
        XmlNodeList nodes = xmlDoc.SelectNodes("FieldData/Field");

        for (int i = 0; i < nodes.Count; i++)
        {
            FieldInfor unitData = new FieldInfor();
            unitData.Read(nodes[i]);
            FieldData infor = AssetOpener.Import<FieldData>(unitData.path);
            _dic.Add(unitData.name, infor);
        }
    }

    public FieldData GetFieldData(string fieldName) {
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
