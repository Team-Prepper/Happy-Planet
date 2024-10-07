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
        CallbackMethod callback = () =>
        {
            UIManager.Instance.OpenGUI<GUIFullScreen>("Field");
            if (_defaultPlanet)
            {
                Destroy(_defaultPlanet);
            }
        };

        GUIFieldLoader loader = UIManager.Instance.OpenGUI<GUIFieldLoader>("FieldLoader");

        if (GameManager.Instance.Auth.IsSignIn())
        {
            loader.FieldLoad(new PlaygroundField(), GameManager.Instance.Auth.GetUserId(), "", callback);
            return;
        }

        loader.LocalFieldLoad(new PlaygroundField(), GameManager.Instance.Auth.GetUserId(), "", callback);


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