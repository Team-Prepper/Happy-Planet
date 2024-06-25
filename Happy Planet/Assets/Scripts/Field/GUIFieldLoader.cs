using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIFieldLoader : GUIFullScreen {

    FieldCameraSet _cameraSet;

    [SerializeField] Text _state;
    [SerializeField] Text _progress;

    [SerializeField] float _progressRoutine = -.4f;

    IEnumerator _DotRoutine() {

        int dotcount = 0;
        _progress.text = " ";
        while (gameObject.activeSelf) {

            if (dotcount < 4)
            {
                _progress.text += ". ";
                dotcount++;
            }
            else {

                _progress.text = " ";
                dotcount = 0;
            }

            yield return new WaitForSeconds(_progressRoutine);
        }
    }

    public void FieldLoad(string fieldName)
    {
        _state.text = "필드 정보 로딩중";
        _cameraSet = GameObject.FindWithTag("CameraSet").GetComponent<FieldCameraSet>();
        DataManager.Instance.FieldDataRead(_FieldDataReadCallback);

        StartCoroutine(_DotRoutine());

    }

    void _FieldDataReadCallback()
    {
        _cameraSet.TimeSet(_FieldAnimCallback);

    }

    void _FieldAnimCallback()
    {
        DataManager.Instance.LogDataRead(_LogDataReadCallback);
        _state.text = "유닛 배치 중";

    }

    void _LogDataReadCallback()
    {
        _cameraSet.LogSet(_TimeSettingCallback);

    }

    void _TimeSettingCallback()
    {
        UIManager.Instance.OpenGUI<GUIFullScreen>("Field");
    }

}
