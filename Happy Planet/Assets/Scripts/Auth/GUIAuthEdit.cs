using EHTool.LangKit;
using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIAuthEdit : GUIPopUp {

    [SerializeField] GameObject _verifingPart;
    [SerializeField] GameObject _verifiedPart;

    CallbackMethod _callback;
    CallbackMethod _fallback;

    [SerializeField] InputField _newName;
    [SerializeField] InputField _password;
    [SerializeField] InputField _newPW;
    [SerializeField] Text _defaultName;

    [SerializeField] GUILoading _loading;

    public override void Open()
    {
        base.Open();
        if (!GameManager.Instance.Auth.IsSignIn()) {
            UIManager.Instance.DisplayMessage("msg_NeedSignIn");
            Close();
            return;
        }
        _verifingPart.SetActive(true);
        _verifiedPart.SetActive(false);
        _defaultName.text = GameManager.Instance.Auth.GetName();
    }

    public void SetCallback(CallbackMethod callback, CallbackMethod fallback) {
        _callback = callback;
        _fallback = fallback;
    }

    public void ReVerify() {

        _loading.LoadingOn("msg_InCheckPW");

        GameManager.Instance.Auth.ReVerify(_password.text, () => {

            _loading.LoadingOff();
            _verifingPart.SetActive(false);
            _verifiedPart.SetActive(true);

        },
        (string msg) =>
        {
            _loading.LoadingOff();
            UIManager.Instance.DisplayMessage(msg);
            Debug.Log(msg);

        });
    }

    public void Setting() {

        if (_newName.text.CompareTo(_defaultName.text) != 0 && _newName.text.CompareTo("") != 0)
        {
            _loading.LoadingOn("msg_InChangeName");
            GameManager.Instance.Auth.DisplayNameChange(_newName.text, () =>
            {
                UIManager.Instance.DisplayMessage("msg_Applied");
                _defaultName.text = GameManager.Instance.Auth.GetName();
                _newName.text = "";
                _callback?.Invoke();
                _loading.LoadingOff();
            },
            (string msg) =>
            {
                UIManager.Instance.DisplayMessage(msg);
                _loading.LoadingOff();

            });
        }

        if (!_verifiedPart.activeSelf) return;

        if (_newPW.text.CompareTo("") == 0) return;

        _loading.LoadingOn("msg_InChangePW");

        GameManager.Instance.Auth.PasswordChange(_newPW.text, () =>
        {
            UIManager.Instance.DisplayMessage("msg_Applied");
            _newPW.text = string.Empty;
            _callback?.Invoke();
            _loading.LoadingOff();
        },
        (string msg) =>
        {
            UIManager.Instance.DisplayMessage(msg);
            _loading.LoadingOff();

        });

    }

    public void Delete() {


        string[] btnName = { "btn_Proceed", "btn_Cancle" };

        CallbackMethod[] callback = new CallbackMethod[2]{ () => {

            _loading.LoadingOn("msg_InDeleteAuth");

            GameManager.Instance.Auth.DeleteUser(() =>
            {
                _loading.LoadingOff();
                UIManager.Instance.DisplayMessage("msg_DeleteAuth");
                Close();
            },
            (string msg) =>
            {
                _loading.LoadingOff();
                UIManager.Instance.DisplayMessage(msg);
            });

        },
        () => {
            return;
        } };

        UIManager.Instance.OpenGUI<GUIChoose>("DoubleChoose")
            .Set("label_DeleteAuth", "msg_ProceedDeleteAuth", btnName, callback);

    }

}
