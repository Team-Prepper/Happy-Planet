
using EasyH.Unity.UI;
using EasyH.Tool.LangKit;
using UnityEngine;

public class GUIEHTextMessageBox : GUIMessageBox {

    [SerializeField] EHText _text;

    protected override void ShowMessage(string key)
    {
        _text.SetText(key);
    }

}