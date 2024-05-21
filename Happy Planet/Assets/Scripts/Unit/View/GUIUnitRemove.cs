using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;

public class GUIUnitRemove : GUIPopUp
{
    IUnit _target;

    public override void Open()
    {
        base.Open();
    }

    public void SetTarget(IUnit target) {
        _target = target;
    }

    public void Cancle() {
        Close();
    }

    public void RemoveTarget() {
        _target.Remove();
    }
}
