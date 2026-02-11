
using EasyH.Unity.UI;
using UnityEngine;
using System;

public class GUITitle : GUIPlanetRotate
{

    [SerializeField] private GameObject _signIn;

    [SerializeField] private GameObject _defaultPlanet;

    public override void Open()
    {
        base.Open();
    }

    public void OpenField()
    {
        GUIFieldLoader loader = UIManager.Instance.
            OpenGUI<GUIFieldLoader>("FieldLoader");

        loader.FieldLoad(new PlaygroundField(),
            AuthManager.Instance.Auth.GetUserId(), "", () =>
            {

                UIManager.Instance.OpenGUI<GUIFullScreen>("Field");
                if (_defaultPlanet)
                {
                    Destroy(_defaultPlanet);
                }
            });

    }

    protected override void Update()
    {
        bool isSignIn = AuthManager.Instance.Auth.IsSignIn();
        _signIn.SetActive(!isSignIn);

        base.Update();
    }

}