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

    private void Update()
    {
        if (!GameManager.Instance.Auth.IsSignIn()) return;

        Close();
    }

    public void SignIn() {

        if (_inProcess) return;

        _inProcess = true;

        _loading.LoadingOn("msg_InTrySignIn");
        GameManager.Instance.Auth.TrySignIn(_id.text, _pw.text, () => {
            _loading.LoadingOff();
            _inProcess = false;
            UIManager.Instance.DisplayMessage("msg_SuccessSignIn");
            Close();

        }, (msg) => {
            _loading.LoadingOff();
            _inProcess = false;
            UIManager.Instance.DisplayMessage(msg);
        });

    }

}
