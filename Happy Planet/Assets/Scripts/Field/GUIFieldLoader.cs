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

    public void FieldLoad(IField newField, string auth, string fieldName, CallbackMethod callback)
    {
        _callback = callback;
        _loading.LoadingOn("msg_InFieldLoad");
        _cameraSet = GameObject.FindWithTag("CameraSet").GetComponent<FieldCameraSet>();

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
        newField?.ConnectDB(auth, fieldName, metaDBConnector, logDBConnector);
        
        _cameraSet.StartSet(() => {
            GameManager.Instance.Field.Dispose();
            GameManager.Instance.Field = newField;
            GameManager.Instance.Field.FieldMetaDataRead(_FieldDataReadCallback, (string msg) => {
                _cameraSet.TimeSet(() => { _cameraSet.LogSet(() => { }); });
                Close();
                UIManager.Instance.DisplayMessage("msg_NotExistPlanet");
            });
        });
        
        //DataManager.Instance.FieldDataRead(_FieldDataReadCallback);

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
            _cameraSet.LogSet(_TimeSettingCallback);
        }, (string msg) => {
            UIManager.Instance.DisplayMessage(msg);
            _loading.LoadingOff();
            _cameraSet.LogSet(() => { });
            Close();
        });

    }

    void _TimeSettingCallback()
    {
        _loading.LoadingOff();
        _callback();
        Close();
    }

}
