using EHTool.UIKit;
using EHTool.LangKit;
using UnityEngine;

public class GUIEHTextMessageBox : GUIMessageBox {

    [SerializeField] EHText _text;

    protected override void ShowMessage(string key)
    {
        _text.SetText(key);
    }

}