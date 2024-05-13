using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;

public class GUIUnitRemove : GUIPopUp
{
    Unit _target;

    public override void Open()
    {
        base.Open();
    }

    public void SetTarget(Unit target) {
        _target = target;
    }

    public void Cancle() {
        Close();
    }

    public void RemoveTarget() { 
        
    }
}
