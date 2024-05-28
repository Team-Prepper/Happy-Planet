using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
using static UnityEngine.GraphicsBuffer;

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
            _targetUnit.LevelUp();
            DataManager.Instance.LevelUp(_targetUnit.Id);
        }
        else { 
            
        }

        Close();

    }
}
