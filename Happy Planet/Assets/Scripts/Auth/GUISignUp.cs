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

        _loading.LoadingOn("msg_InTrySignUp");
        GameManager.Instance.Auth.TrySignUp(_id.text, _pw.text, () => {
            _loading.LoadingOff();
            _inProcess = false;
            UIManager.Instance.DisplayMessage("msg_SuccessSignUp");
            Close();
            //UIManager.Instance.OpenGUI<GUIAuthEdit>("AuthEdit");

        }, (msg) => {
            _loading.LoadingOff();
            _inProcess = false;

            UIManager.Instance.DisplayMessage(msg);

        });
    }
}
