using EHTool.UIKit;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUITitle : GUIPlanetRotate {

    [SerializeField] GameObject _signIn;
    [SerializeField] GameObject _signOut;

    public void OpenField()
    {
        UIManager.Instance.OpenGUI<GUIFieldLoader>("FieldLoader").FieldLoad(new PlaygroundField(), GameManager.Instance.Auth.GetUserId(), "", ()=> {
            UIManager.Instance.OpenGUI<GUIFullScreen>("Field");
        });
    }

    public void SignOut() {
        GameManager.Instance.Auth.SignOut();
    }

    protected override void Update()
    {
        bool isSignIn = GameManager.Instance.Auth.IsSignIn();
        _signIn.SetActive(!isSignIn);
        _signOut.SetActive(isSignIn);

        base.Update();
    }

}