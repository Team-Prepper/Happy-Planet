using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EHTool.UIKit;
using UnityEngine.UI;

public class GUILogIn : GUIPopUp
{

    [SerializeField] InputField _id;
    [SerializeField] InputField _pw;

    IAuth _auth;

    public override void SetOff()
    {
        Close();
    }

    public void LogIn() {
        _auth.TryAuth(_id.text, _pw.text, Callback);
    }

    public void Callback() {
        UIManager.Instance.OpenGUI<GUIDefault>("GUI_Default");
    }

    public void Fallback() {
        UIManager.Instance.DisplayMessage("로그인에 실패함!!");
    }

}
