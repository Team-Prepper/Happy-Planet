using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIPopUp : GUIWindow
{
    protected override void Open()
    {
        if (UIManager.Instance.NowPopUp == null)
        {
            base.Open();
            return;
        }

        RectTransform rect = gameObject.GetComponent<RectTransform>();

        UIManager.Instance.NowPopUp.AddPopUp(this);
        rect.offsetMax = Vector3.zero;
        rect.offsetMin = Vector3.zero;
    }

    public override void Close()
    {
        UIManager.Instance.NowPopUp.ClosePopUP(this);
        base.Close();
    }
}
