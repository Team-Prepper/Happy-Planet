using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIFullScreen : GUIWindow
{
    [SerializeField] protected List<GUIPopUp> _underPopUps = new List<GUIPopUp>();

    public void AddPopUp(GUIPopUp newPopUp) {
        newPopUp.transform.SetParent(transform);
        _underPopUps.Add(newPopUp);
    }

    public void ClosePopUP(GUIPopUp newPopUp) { 
        _underPopUps.Remove(newPopUp);
    }


    protected override void Open()
    {
        base.Open();
        UIManager.Instance.EnrollmentGUI(this);
    }

    public override void Close()
    {
        UIManager.Instance.Pop();
        base.Close();
    }
}
