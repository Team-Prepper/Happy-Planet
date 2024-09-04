using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIFieldLoader : GUIFullScreen {

    FieldCameraSet _cameraSet;

    [SerializeField] GUILoading _loading;

    public void FieldLoad(string fieldName)
    {
        _loading.LoadingOn("InFieldLoad");

        _cameraSet = GameObject.FindWithTag("CameraSet").GetComponent<FieldCameraSet>();

        DataManager.Instance.FieldDataRead(_FieldDataReadCallback);

    }

    void _FieldDataReadCallback()
    {
        _cameraSet.TimeSet(_FieldAnimCallback);

    }

    void _FieldAnimCallback()
    {
        _loading.LoadingOff();
        _loading.LoadingOn("InUnitPlacement");

        DataManager.Instance.LogDataRead(() =>
        {
            _cameraSet.LogSet(_TimeSettingCallback);
        });

    }

    void _TimeSettingCallback()
    {
        _loading.LoadingOff();
        UIManager.Instance.OpenGUI<GUIFullScreen>("Field");
        Close();
    }

}
