using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EHTool.UIKit;
using UnityEngine.UI;

public class GUISignIn : GUIPopUp
{

    [SerializeField] InputField _id;
    [SerializeField] InputField _pw;

    public void SignIn() {

        GameManager.Instance.Auth.TrySignIn(_id.text, _pw.text, () => {
            UIManager.Instance.DisplayMessage("로그인 성공");
            Close();

        }, (msg) => {
            UIManager.Instance.DisplayMessage(msg);
        });

    }

}
