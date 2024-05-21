using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UISystem;

public class GUIUnitInfor : GUIPanel
{
    [SerializeField] IUnit _targetUnit;

    [SerializeField] Image _unitImage;
    [SerializeField] Image _earnBar;
    [SerializeField] Image _lifeSpanBar;

    [SerializeField] Text _unitName;
    [SerializeField] Text _unitLevel;
    [SerializeField] Text _unitMoneayEarn;
    [SerializeField] Text _unitPollutionEarn;

    public void SetUnit(IUnit unit)
    {
        _targetUnit = unit;

        UnitInfor data = unit.GetInfor();

        _unitName.text = data.UnitCode;
        _unitMoneayEarn.text = data.GetEarnMoney(_targetUnit.NowLevel).ToString();
        _unitPollutionEarn.text = data.GetEarnPollution(_targetUnit.NowLevel).ToString() + " %";

        _SetData();

    }

    public void LevelUp() {
        UIManager.Instance.OpenGUI<GUIUnitLevelUp>("UnitLevelUp").Set(_targetUnit);
    }

    public void Remove()
    {
        UIManager.Instance.OpenGUI<GUIUnitRemove>("UnitRemove").SetTarget(_targetUnit);
    }

    // Update is called once per frame
    void Update()
    {
        if (_targetUnit == null)
        {
            Close();
        }
        
        _SetData();

    }

    void _SetData() {
        _lifeSpanBar.fillAmount = _targetUnit.LifeSpanRatio;
        _earnBar.fillAmount = _targetUnit.EarnRatio;
    }
}
