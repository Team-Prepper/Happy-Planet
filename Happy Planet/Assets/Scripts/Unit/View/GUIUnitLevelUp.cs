using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
using static UnityEngine.GraphicsBuffer;

public class GUIUnitLevelUp : GUIPopUp
{

    IUnit _targetUnit;
    int _useMoney = 0;

    public void Set(IUnit infor) {
        _targetUnit = infor;
    }

    public void Cancle() {
        Close();
    }

    public void DoLevelUp() {
        if (GameManager.Instance.Money < _useMoney) {
            UIManager.Instance.DisplayMessage("Need More Money");
            return;
        }

        if (_targetUnit != null)
        {
            _targetUnit.LevelUp();
            DataManager.Instance.LevelUp(_targetUnit.Id, _useMoney);
        }
        else { 
            
        }

        Close();

    }
}
