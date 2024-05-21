using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;

public class GUIUnitLevelUp : GUIPopUp
{

    IUnit _targetUnit;

    public void Set(IUnit infor) {
        _targetUnit = infor;
    }

    public void Cancle() {
        Close();
    }

    public void DoLevelUp() {
        if (_targetUnit != null)
        {

        }
        else { 
            
        }

        Close();

    }
}
