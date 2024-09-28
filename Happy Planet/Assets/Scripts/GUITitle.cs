using EHTool.UIKit;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GUITitle : GUIPlanetRotate {

    [SerializeField] GameObject _signIn;
    [SerializeField] GameObject _signOut;

    [SerializeField] GameObject _defaultPlanet;

    public override void Open()
    {
        base.Open();
    }

    public void OpenField()
    {
        UIManager.Instance.OpenGUI<GUIFieldLoader>("FieldLoader").FieldLoad(new PlaygroundField(),
            GameManager.Instance.Auth.GetUserId(), "", () =>
            {
                UIManager.Instance.OpenGUI<GUIFullScreen>("Field");
                if (_defaultPlanet)
                {
                    Destroy(_defaultPlanet);
                }
            });
    }

    public void SignOut()
    {
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