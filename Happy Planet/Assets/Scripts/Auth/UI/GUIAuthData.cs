
using EasyH.Unity.UI;
using EasyH.Tool.LangKit;
using UnityEngine;
using UnityEngine.UI;

public class GUIAuthData : GUIPopUp {

    [SerializeField] private Text _defaultName;
    [SerializeField] private Text _id;

    [SerializeField] private GUILoading _loading;

    public override void Open()
    {
        base.Open();
        
        if (!AuthManager.Instance.Auth.IsSignIn())
        {
            UIManager.Instance.DisplayMessage("msg_NeedSignIn");
            Close();
            return;
        }
        _UpdateData();
    }

    public override void SetOn()
    {
        base.SetOn();
        _UpdateData();
    }

    private void _UpdateData()
    {
        _id.text = AuthManager.Instance.Auth.GetUserId();
        _defaultName.text = string.Format("{0}: {1}",
            LangManager.Instance.GetStringByKey("label_Name"),
                AuthManager.Instance.Auth.GetName());
        
    }

    public void SignOut()
    {
        AuthManager.Instance.Auth.SignOut();
        Close();
    }

}
