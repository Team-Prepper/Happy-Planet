using EHTool.UIKit;
using EHTool.LangKit;
using UnityEngine;
using UnityEngine.UI;

public class GUIAuthData : GUIPopUp {

    [SerializeField] private Text _defaultName;
    [SerializeField] private Text _id;

    [SerializeField] private GUILoading _loading;

    public override void Open()
    {
        base.Open();
        
        if (!GameManager.Instance.Auth.IsSignIn())
        {
            UIManager.Instance.DisplayMessage("msg_NeedSignIn");
            Close();
            return;
        }
        
        _id.text = GameManager.Instance.Auth.GetUserId();
        _defaultName.text = string.Format("{0}: {1}", LangManager.Instance.GetStringByKey("label_Name"), GameManager.Instance.Auth.GetName());
    }

    public void SignOut()
    {
        GameManager.Instance.Auth.SignOut();
        Close();
    }

}
