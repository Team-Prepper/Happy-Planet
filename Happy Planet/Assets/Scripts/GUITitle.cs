using EHTool.UIKit;
using UnityEngine;

public class GUITitle : GUIFullScreen {

    [SerializeField] GameObject _signIn;
    [SerializeField] GameObject _signOut;

    public void OpenField() {
        UIManager.Instance.OpenGUI<GUIFieldLoader>("FieldLoader").FieldLoad("temp");
    }

    public void SignOut() {
        GameManager.Instance.Auth.SignOut();
    }

    private void Update()
    {
        bool isSignIn = GameManager.Instance.Auth.IsSignIn();
        _signIn.SetActive(!isSignIn);
        _signOut.SetActive(isSignIn);
    }

}