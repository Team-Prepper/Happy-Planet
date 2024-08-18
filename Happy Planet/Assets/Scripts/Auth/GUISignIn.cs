using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EHTool.UIKit;
using UnityEngine.UI;

public class GUISignIn : GUIPopUp
{

    [SerializeField] InputField _id;
    [SerializeField] InputField _pw;

    [SerializeField] GUILoading _loading;

    bool _inProcess = false;

    public void SignIn() {

        if (_inProcess) return;

        _inProcess = true;

        _loading.LoadingOn("회원가입 시도중");
        GameManager.Instance.Auth.TrySignIn(_id.text, _pw.text, () => {
            _loading.LoadingOff();
            _inProcess = false;
            UIManager.Instance.DisplayMessage("로그인 성공");
            Close();

        }, (msg) => {
            _loading.LoadingOff();
            _inProcess = false;
            UIManager.Instance.DisplayMessage(msg);
        });

    }

}
