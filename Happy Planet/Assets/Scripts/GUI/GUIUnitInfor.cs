using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UISystem;
using static UnityEngine.GraphicsBuffer;

public class GUIUnitInfor : GUIPopUp
{
    [SerializeField] Unit _targetUnit;

    [SerializeField] Image _unitImage;
    [SerializeField] Image _earnBar;
    [SerializeField] Image _lifeSpanBar;

    [SerializeField] Text _unitName;
    [SerializeField] Text _unitLevel;
    [SerializeField] Text _unitMoneayEarn;
    [SerializeField] Text _unitPollutionEarn;

    public void SetUnit(Unit unit)
    {
        _targetUnit = unit;
        UnitInfor data = unit.GetInfor();

        _unitName.text = data.UnitCode;
        _unitMoneayEarn.text = data.GetEarnMoney(_targetUnit.NowLevel).ToString();
        _unitPollutionEarn.text = data.GetEarnPollution(_targetUnit.NowLevel).ToString() + " %";

        _SetData();

    }

    // Update is called once per frame
    void Update()
    {
        if (_targetUnit == null)
        {
            Destroy(gameObject);
        }
        _SetData();
    }

    void _SetData() {
        _lifeSpanBar.fillAmount = _targetUnit.LifeSpanRatio;
        _earnBar.fillAmount = _targetUnit.EarnRatio;
        //_unitLevel.text = _targetUnit.NowLevel.ToString();
    }
}
