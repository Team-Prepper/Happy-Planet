using EHTool.DBKit;
using EHTool.UIKit;
using UnityEngine;
using System;

using FieldDataDB = EHTool.DBKit.IDatabaseConnector<string, FieldDataRecord>;
using LogDB = EHTool.DBKit.IDatabaseConnector<int, Log>;

public class WebGLFieldMetaDataData : FirebaseWebGLConnector<string, FieldDataRecord> { }
public class TWebGLLog : FirestoreWebGLConnector<int, Log> { }

public class GUIFieldLoader : GUIFullScreen {

    private FieldCameraSet _cameraSet;

    [SerializeField] private GUILoading _loading;
    private Action _callback;

    public override void Open()
    {
        base.Open();
        _cameraSet = GameObject.FindWithTag("CameraSet").GetComponent<FieldCameraSet>();
    }

    public void LocalFieldLoad(IField newField, string auth, string fieldName, Action callback) {

        _callback = callback;

        FieldDataDB metaDBConnector = new LocalDatabaseConnector<string, FieldDataRecord>();
        LogDB logDBConnector = new LocalDatabaseConnector<int, Log>();

        FieldLoad(SetField(newField, metaDBConnector, logDBConnector, auth, fieldName));
    }

    public void FieldLoad(IField newField, string auth, string fieldName, Action callback)
    {
        _callback = callback;

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

        FieldLoad(SetField(newField, metaDBConnector, logDBConnector, auth, fieldName));

    }

    public void FieldClose() {
        FieldLoad(FieldManager.Instance.GetLastPlayerField());
    }

    private IField SetField(IField newField, FieldDataDB metaDBConnector, LogDB logDBConnector,
        string auth, string fieldName)
    {
        if (FieldManager.Instance.FieldExist(auth + fieldName, out IField existField)) {
            return existField;
        }

        newField?.ConnectDB(auth, fieldName, metaDBConnector, logDBConnector);

        return newField;

    }

    private void FieldLoad(IField newField)
    {
        _loading.LoadingOn("msg_InFieldLoad");

        _cameraSet.StartSet(() => {
            GameManager.Instance.Field.Dispose();
            GameManager.Instance.Field = newField;
            
            _cameraSet.CameraSet(newField.PlanetData.CameraSettingValue);
            
            GameManager.Instance.Field.LoadFieldMetaData(_FieldDataReadCallback,
                (msg) => {
                    UIManager.Instance.DisplayMessage("msg_NotExistPlanet");
                    _callback = null;
                    FieldClose();
                    
                });
        });

    }

    private void _FieldDataReadCallback()
    {
        _cameraSet.TimeSet(_FieldAnimCallback);

    }

    private void _FieldAnimCallback()
    {
        _loading.LoadingOff();
        _loading.LoadingOn("msg_InUnitPlacement");

        GameManager.Instance.Field.LoadLog(() =>
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
