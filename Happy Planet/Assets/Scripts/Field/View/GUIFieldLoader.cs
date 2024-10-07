using EHTool.DBKit;
using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TWebGLLog : FirestoreWebGLConnector<Log> { }
public class WebGLFieldMetaDataData : FirebaseWebGLConnector<IField.FieldMetaData> { }

public class GUIFieldLoader : GUIFullScreen {

    FieldCameraSet _cameraSet;

    [SerializeField] GUILoading _loading;
    CallbackMethod _callback;

    public override void Open()
    {
        base.Open();
        _cameraSet = GameObject.FindWithTag("CameraSet").GetComponent<FieldCameraSet>();
    }

    public void LocalFieldLoad(IField newField, string auth, string fieldName, CallbackMethod callback) {

        _callback = callback;

        IDatabaseConnector<IField.FieldMetaData> metaDBConnector = new LocalDatabaseConnector<IField.FieldMetaData>();
        IDatabaseConnector<Log> logDBConnector = new LocalDatabaseConnector<Log>();

        FieldLoad(SetField(newField, metaDBConnector, logDBConnector, auth, fieldName));
    }

    public void FieldLoad(IField newField, string auth, string fieldName, CallbackMethod callback)
    {
        _callback = callback;

        IDatabaseConnector<IField.FieldMetaData> metaDBConnector;
        IDatabaseConnector<Log> logDBConnector;

#if !UNITY_WEBGL || UNITY_EDITOR

        //metaDBConnector = new LocalDatabaseConnector<IField.FieldMetaData>();
        metaDBConnector = new FirebaseConnector<IField.FieldMetaData>();
        //metaDBConnector = new FirestoreConnector<GameManagerData>();

        //logDBConnector = new LocalDatabaseConnector<Log>();
        logDBConnector = new FirestoreConnector<Log>();

#else
        //metaDBConnector = new LocalDatabaseConnector<IField.FieldMetaData>();
        metaDBConnector = DataManager.Instance.AddComponent<WebGLFieldMetaDataData>();

        logDBConnector = DataManager.Instance.GetComponent<TWebGLLog>();
        logDBConnector ??= DataManager.Instance.AddComponent<TWebGLLog>();


#endif
        FieldLoad(SetField(newField, metaDBConnector, logDBConnector, auth, fieldName));

    }

    public void FieldClose() {
        FieldLoad(FieldManager.Instance.GetLastPlayerField());
    }

    IField SetField(IField newField, IDatabaseConnector<IField.FieldMetaData> metaDBConnector, IDatabaseConnector<Log> logDBConnector, string auth, string fieldName)
    {
        if (FieldManager.Instance.FieldExist(auth + fieldName, out IField existField)) {
            return existField;
        }

        newField?.ConnectDB(auth, fieldName, metaDBConnector, logDBConnector);

        return newField;

    }

    void FieldLoad(IField newField)
    {
        _loading.LoadingOn("msg_InFieldLoad");

        _cameraSet.StartSet(() => {
            GameManager.Instance.Field.Dispose();
            GameManager.Instance.Field = newField;
            
            _cameraSet.CameraSet(newField.CameraSettingValue);
            
            GameManager.Instance.Field.FieldMetaDataRead(_FieldDataReadCallback,
                (string msg) => {
                    UIManager.Instance.DisplayMessage("msg_NotExistPlanet");
                    _callback = null;
                    FieldClose();
                    
                });
        });

    }

    void _FieldDataReadCallback()
    {
        _cameraSet.TimeSet(_FieldAnimCallback);

    }

    void _FieldAnimCallback()
    {
        _loading.LoadingOff();
        _loading.LoadingOn("msg_InUnitPlacement");

        GameManager.Instance.Field.FieldLogDataRead(() =>
        {
            _cameraSet.LogSet(() =>
            {
                _loading.LoadingOff();
                Close();
                _callback?.Invoke();
            });

        }, (string msg) => {
            UIManager.Instance.DisplayMessage("msg_NotExistPlanet");
            _callback = null;
            FieldClose();

        });

    }

}
