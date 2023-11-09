using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UISystem;

public class GUIUnitInfor : GUIPopUp
{
    [SerializeField] Unit _targetUnit;

    [SerializeField] UnitInfor _targetData;

    [SerializeField] Image _unitImage;
    [SerializeField] Image _earnBar;
    [SerializeField] Image _lifeSpanBar;

    [SerializeField] Text _unitName;
    [SerializeField] Text _unitLevel;
    [SerializeField] Text _unitMoneayEarn;
    [SerializeField] Text _unitPollutionEarn;

    public void SetUnit(Unit unit) {
        _targetUnit = unit;
        _targetData = _targetUnit.GetData();

        _SetData();
        _unitMoneayEarn.text = _targetData.GetEarnMoney().ToString();
        _unitPollutionEarn.text = _targetData.GetEarnPollution().ToString() + " %";

    }

    // Update is called once per frame
    void Update()
    {
        if (_targetData == null)
        {
            Destroy(gameObject);
        }
        _SetData();
    }

    void _SetData() {
        _lifeSpanBar.fillAmount = _targetData.LifeSpanRatio();
        _earnBar.fillAmount = _targetData.EarnRatio();

        _unitName.text = _targetData.UnitCode;
        //_unitLevel.text = _targetData.NowLevel.ToString();
    }
}
