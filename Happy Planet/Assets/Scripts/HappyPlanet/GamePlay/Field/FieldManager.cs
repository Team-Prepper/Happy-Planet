using EasyH;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : Singleton<FieldManager> {

    private IDictionary<string, IField> _fields;

    private IField _lastPlayerField;

    protected override void OnCreate()
    {
        _fields = new Dictionary<string, IField>();
    }

    public void AddFieldSet(string fieldName, IField field)
    {
        _lastPlayerField = field;
        _fields[fieldName] = field;
    }

    public IField GetLastPlayerField() {
        if (_lastPlayerField == null)
            _lastPlayerField = new DefaultField();
        return _lastPlayerField;
    }

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
