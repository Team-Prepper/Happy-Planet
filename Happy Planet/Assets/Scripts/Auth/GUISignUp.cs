using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUISignUp : GUIPopUp {

    [SerializeField] InputField _id;
    [SerializeField] InputField _pw;

    [SerializeField] GUILoading _loading;

    bool _inProcess = false;

    public void SignUp()
    {
        if (_inProcess) return;

        _inProcess = true;

        _loading.LoadingOn("회원가입 시도중");
        GameManager.Instance.Auth.TrySignUp(_id.text, _pw.text, () => {
            _loading.LoadingOff();
            _inProcess = false;
            UIManager.Instance.DisplayMessage("회원가입 성공");
            Close();

        }, (msg) => {
            _loading.LoadingOff();
            _inProcess = false;
            UIManager.Instance.DisplayMessage(msg);

        });
    }
}
