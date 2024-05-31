using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;

public class GUIUnitRemove : GUIPopUp
{
    IUnit _target;
    int _useMoney = 0;

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

    public void RemoveTarget()
    {
        if (GameManager.Instance.Money < _useMoney)
        {
            UIManager.Instance.DisplayMessage("Need More Money");
            Close();
            return;
        }

        _target.Remove();
        DataManager.Instance.RemoveUnit(_target, _target.Id, 0);
        Close();
    }
}
