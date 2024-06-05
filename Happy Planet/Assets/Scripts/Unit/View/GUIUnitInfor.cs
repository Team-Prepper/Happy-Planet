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

        _unitName.text = unit.GetInfor().UnitCode;

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
        if (!_targetUnit.Exist())
        {
            Close();
            return;
        }
        
        _SetData();

    }

    void _SetData()
    {
        IUnit.LevelData earnData = _targetUnit.GetInfor().GetLevelData(_targetUnit.NowLevel);

        _unitImage.sprite = earnData.Sprite;

        _unitMoneayEarn.text = earnData.EarnMoney.ToString();
        _unitPollutionEarn.text = earnData.EarnEnergy.ToString() + " %";

        _lifeSpanBar.fillAmount = _targetUnit.LifeSpanRatio;
        _earnBar.fillAmount = _targetUnit.EarnRatio;
    }
}
