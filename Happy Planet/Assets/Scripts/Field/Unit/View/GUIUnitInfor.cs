using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EHTool.UIKit;
using EHTool.LangKit;

public class GUIUnitInfor : GUIPanel {
    [SerializeField] IUnit _targetUnit;

    [SerializeField] Image _unitImage;
    [SerializeField] Image _earnBar;
    [SerializeField] Image _lifeSpanBar;

    [SerializeField] EHText _unitName;
    [SerializeField] Text _unitLevel;
    [SerializeField] Text _unitMoneayEarn;
    [SerializeField] Text _unitPollutionEarn;

    public void SetUnit(IUnit unit)
    {
        _targetUnit = unit;

        _unitName.SetText(unit.GetInfor().UnitName);

        _SetData();

    }

    public void LevelUp()
    {
        if (_targetUnit.NowLevel >= _targetUnit.GetInfor().GetMaxLevel())
        {
            return;
        }

        string[] btnName = { "btn_LevelUp", "btn_Cancel" };

        int cost = _targetUnit.GetInfor().GetLevelData(_targetUnit.NowLevel + 1).LevelUpCost;

        CallbackMethod[] callback = new CallbackMethod[2]{ () => {

            if (_targetUnit == null)
            {
                return;
            }

            if (GameManager.Instance.Field.Money < cost) {
                UIManager.Instance.DisplayMessage("msg_NeedMoreMoney");
                Close();
                return;
            }

            _targetUnit.LevelUp(GameManager.Instance.Field.SpendTime);
            GameManager.Instance.Field.LevelUp(_targetUnit.Id, cost);

        },
        () => {
            return;
        } };

        UIManager.Instance.OpenGUI<GUIChoose>("DoubleChoose").Set("label_UnitLevelUp",
            string.Format(LangManager.Instance.GetStringByKey("msg_CostUseAsk"), cost), btnName, callback);

    }

    public void Remove()
    {

        string[] btnName = { "btn_UnitRemove", "btn_Cancel" };

        int cost = _targetUnit.GetInfor().GetLevelData(_targetUnit.NowLevel).RemoveCost;

        CallbackMethod[] callback = new CallbackMethod[2]{ () => {

            if (_targetUnit == null)
            {
                return;
            }

            if (GameManager.Instance.Field.Money < cost)
            {
                UIManager.Instance.DisplayMessage("msg_NeedMoreMoney");
                Close();
                return;
            }

            _targetUnit.Remove(GameManager.Instance.Field.SpendTime);
            GameManager.Instance.Field.RemoveUnit(_targetUnit, _targetUnit.Id, cost);

        },
        () => {
            return;
        } };

        UIManager.Instance.OpenGUI<GUIChoose>("DoubleChoose").Set("label_UnitRemove",
            string.Format(LangManager.Instance.GetStringByKey("msg_CostUseAsk"), cost), btnName, callback);

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
