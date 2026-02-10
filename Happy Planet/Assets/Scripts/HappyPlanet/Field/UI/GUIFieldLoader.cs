using UnityEngine;
using System;
using EasyH.Unity.UI;

public class GUIFieldLoader : GUIFullScreen {

    private FieldCameraSet _cameraSet;

    [SerializeField] private GUILoading _loading;
    private Action _callback;

    public override void Open()
    {
        base.Open();
        _cameraSet = GameObject.FindWithTag("CameraSet").
            GetComponent<FieldCameraSet>();
    }

    public void LocalFieldLoad(IField newField, string auth,
        string fieldName, Action callback) {

        _callback = callback;

        FieldManager.Instance.SetLocalField(
            newField, auth, fieldName);

        FieldLoad(newField);
    }

    private string _GetAuthName(string name)
    {
        string ret = name.Replace(".", "_")
            .Replace("#", "_")
            .Replace("$", "_")
            .Replace("[", "_")
            .Replace("]", "_")
            .Replace("\n", "_");

        if (ret.Length == 0)
        {
            return "_";
        }

        return ret;
    }

    public void FieldLoad(IField newField, string auth,
        string fieldName, Action callback)
    {

        _callback = callback;

        FieldManager.Instance.SetField(
            newField, _GetAuthName(auth), fieldName);

        FieldLoad(newField);

    }

    public void FieldClose() {
        FieldLoad(FieldManager.Instance.GetLastPlayerField());
    }

    private void FieldLoad(IField newField)
    {
        newField.ConnectDB();
        _loading.LoadingOn("msg_InFieldLoad");

        _cameraSet.StartSet(() => {
            GameManager.Instance.Field.Dispose();
            GameManager.Instance.Field = newField;
            
            _cameraSet.CameraSet(newField.PlanetData.CameraSettingValue);
            
            GameManager.Instance.Field.LoadFieldMetaData(
                _FieldDataReadCallback,
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
