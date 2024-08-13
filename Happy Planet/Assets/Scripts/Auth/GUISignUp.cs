using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUISignUp : GUIPopUp {

    [SerializeField] InputField _id;
    [SerializeField] InputField _pw;

    public override void SetOff()
    {
        Close();
    }

    public void SignIn()
    {
        GameManager.Instance.Auth.TrySignUp(_id.text, _pw.text, () => {
            UIManager.Instance.DisplayMessage("회원가입 성공");
            Close();

        }, (msg) => { 
            
        });
    }
}
