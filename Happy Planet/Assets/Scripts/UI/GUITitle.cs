
using EasyH.Unity.UI;
using UnityEngine;
using System;

public class GUITitle : GUIPlanetRotate {

    [SerializeField] private GameObject _signIn;

    [SerializeField] private GameObject _defaultPlanet;

    public override void Open()
    {
        base.Open();
    }

    public void OpenField()
    {
        void callback()
        {
            UIManager.Instance.OpenGUI<GUIFullScreen>("Field");
            if (_defaultPlanet)
            {
                Destroy(_defaultPlanet);
            }
        }

        GUIFieldLoader loader = UIManager.Instance.
            OpenGUI<GUIFieldLoader>("FieldLoader");

        if (AuthManager.Instance.Auth.IsSignIn())
        {
            loader.FieldLoad(new PlaygroundField(),
                AuthManager.Instance.Auth.GetUserId(), "", callback);
            return;
        }

        loader.LocalFieldLoad(new PlaygroundField(),
            AuthManager.Instance.Auth.GetUserId(), "", callback);


    }

    protected override void Update()
    {
        bool isSignIn = AuthManager.Instance.Auth.IsSignIn();
        _signIn.SetActive(!isSignIn);

        base.Update();
    }

}