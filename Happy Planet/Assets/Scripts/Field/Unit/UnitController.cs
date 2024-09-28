using System.Collections;
using System.Collections.Generic;
using EHTool.UIKit;
using UnityEngine;

public class UnitController : MonoBehaviour, IUnitController
{
    IUnit _targetUnit;

    void Start() { 
        _targetUnit = GetComponent<IUnit>();
    }

    public void Initial(UnitData infor)
    { 

    }

    public void Interaction()
    {
        UIManager.Instance.OpenGUI<GUIUnitInfor>("UnitInfor").SetUnit(_targetUnit);
    }

}
