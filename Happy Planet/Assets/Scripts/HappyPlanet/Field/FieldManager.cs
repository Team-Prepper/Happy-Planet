using System.Collections.Generic;
using System;
using EasyH;
using EasyH.Tool.DBKit;

using FieldDataDB = EasyH.Tool.DBKit.IDatabaseConnector<string, FieldDataRecord>;
using LogDB = EasyH.Tool.DBKit.IDatabaseConnector<int, Log>;

public class WebGLFieldMetaDataData : FirebaseWebGLConnector<string, FieldDataRecord> { }
public class TWebGLLog : FirestoreWebGLConnector<int, Log> { }

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
    
    public void SetLocalField(IField newField, string auth, string fieldName)
    {

        FieldDataDB metaDBConnector =
            new LocalDatabaseConnector<string, FieldDataRecord>();
        LogDB logDBConnector =
            new LocalDatabaseConnector<int, Log>();

        SetField(newField, metaDBConnector,
            logDBConnector, auth, fieldName);
    }

    public void SetField(IField newField, string auth, string fieldName)
    {

        FieldDataDB metaDBConnector;
        LogDB logDBConnector;

#if !UNITY_WEBGL || UNITY_EDITOR

        //metaDBConnector = new LocalDatabaseConnector<IField.FieldMetaData>();
        metaDBConnector = new FirebaseConnector<string, FieldDataRecord>();
        //metaDBConnector = new FirestoreConnector<GameManagerData>();

        //logDBConnector = new LocalDatabaseConnector<Log>();
        logDBConnector = new FirestoreConnector<int, Log>();
#else
        //metaDBConnector = new LocalDatabaseConnector<IField.FieldMetaData>();
        metaDBConnector = GameManager.Instance.GetComponent<WebGLFieldMetaDataData>();
        metaDBConnector ??= GameManager.Instance.gameObject.AddComponent<WebGLFieldMetaDataData>();

        logDBConnector = GameManager.Instance.GetComponent<TWebGLLog>();
        logDBConnector ??= GameManager.Instance.gameObject.AddComponent<TWebGLLog>();
#endif

        SetField(newField, metaDBConnector,
                    logDBConnector, auth, fieldName);

    }
    
    private IField SetField(IField newField, FieldDataDB metaDBConnector, LogDB logDBConnector,
        string auth, string fieldName)
    {
        if (FieldExist(auth + fieldName,
            out IField existField))
        {
            return existField;
        }

        newField?.ConnectDB(auth, fieldName, metaDBConnector, logDBConnector);

        return newField;

    }
}
