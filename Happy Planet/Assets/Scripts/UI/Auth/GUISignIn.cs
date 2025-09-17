using UnityEngine;
using EasyH.Unity.UI;
using UnityEngine.UI;

public class GUISignIn : GUIPopUp
{

    [SerializeField] private InputField _id;
    [SerializeField] private InputField _pw;

    [SerializeField] private GUILoading _loading;

    private bool _inProcess = false;

    private void Update()
    {
        if (!AuthManager.Instance.Auth.IsSignIn()) return;

        Close();
    }

    public void SignIn() {

        if (_inProcess) return;

        _inProcess = true;

        _loading.LoadingOn("msg_InTrySignIn");

        AuthManager.Instance.Auth.TrySignIn(_id.text, _pw.text, () => {
            
            UIManager.Instance.DisplayMessage("msg_SuccessSignIn");

            _loading.LoadingOff();
            _inProcess = false;

            Close();

        }, (msg) => {
            _loading.LoadingOff();
            _inProcess = false;
            UIManager.Instance.DisplayMessage(msg);
        });

    }

}
