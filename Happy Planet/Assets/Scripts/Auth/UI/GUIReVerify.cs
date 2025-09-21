
using EasyH.Unity.UI;
using UnityEngine;
using UnityEngine.UI;

public class GUIAuthReVerify : GUIPopUp
{
    [SerializeField] private InputField _password;
    [SerializeField] private GUILoading _loading;

    [SerializeField] private string _verifyOpenWindow;

    public override void Open()
    {
        base.Open();
        if (AuthManager.Instance.Auth.IsSignIn())
        {
            return;
        }
        UIManager.Instance.DisplayMessage("msg_NeedSignIn");
        Close();

    }

    public void ReVerify()
    {

        _loading.LoadingOn("msg_InCheckPW");

        AuthManager.Instance.Auth.ReVerify(_password.text, () =>
        {
            _loading.LoadingOff();
            OpenWindow(_verifyOpenWindow);
            Close();

        },
        (string msg) =>
        {
            _loading.LoadingOff();
            UIManager.Instance.DisplayMessage(msg);

        });
    }

}
