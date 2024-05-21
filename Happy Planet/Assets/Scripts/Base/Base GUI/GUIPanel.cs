using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class GUIPanel : GUIWindow, IGUIPanel
{
    public override void Open()
    {
        base.Open();
        UIManager.Instance.NowDisplay.AddPanel(this);
    }

    public override void Close()
    {
        UIManager.Instance.NowDisplay.ClosePanel();
        base.Close();

    }
}
