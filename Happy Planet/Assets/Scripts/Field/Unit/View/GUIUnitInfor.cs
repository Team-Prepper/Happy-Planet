using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EHTool.UIKit;
using static UnityEngine.GraphicsBuffer;

public class GUIUnitInfor : GUIPanel {
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

    public void LevelUp()
    {

        string[] btnName = { "레벨업", "취소" };

        int cost = _targetUnit.GetInfor().GetLevelData(_targetUnit.NowLevel).LevelUpCost;

        CallbackMethod[] callback = new CallbackMethod[2]{ () => {

            if (_targetUnit == null)
            {
                return;
            }

            if (GameManager.Instance.Money < cost) {
                UIManager.Instance.DisplayMessage("Need More Money");
                Close();
                return;
            }

            _targetUnit.LevelUp();
            DataManager.Instance.LevelUp(_targetUnit.Id, cost);

        },
        () => {
            return;
        } };

        UIManager.Instance.OpenGUI<GUIChoose>("DoubleChoose").Set("유닛 레벨 업", string.Format("{0}코인이 소모됩니다. 진행하시겠습니까?", cost), btnName, callback);

        //UIManager.Instance.OpenGUI<GUIUnitLevelUp>("UnitLevelUp").Set(_targetUnit);
    }

    public void Remove()
    {

        string[] btnName = { "제거", "취소" };

        int cost = _targetUnit.GetInfor().GetLevelData(_targetUnit.NowLevel).RemoveCost;

        CallbackMethod[] callback = new CallbackMethod[2]{ () => {

            if (_targetUnit == null)
            {
                return;
            }

            if (GameManager.Instance.Money < cost)
            {
                UIManager.Instance.DisplayMessage("Need More Money");
                Close();
                return;
            }

            _targetUnit.Remove();
            DataManager.Instance.RemoveUnit(_targetUnit, _targetUnit.Id, cost);

        },
        () => {
            return;
        } };

        UIManager.Instance.OpenGUI<GUIChoose>("DoubleChoose").Set("유닛 제거", string.Format("{0}코인이 소모됩니다. 진행하시겠습니까?", cost), btnName, callback);

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
