using UnityEngine;
using UnityEngine.UI;
using EasyH.Unity.UI;
using EasyH.Tool.LangKit;
using System;

public class GUIUnitInfor : GUIPanel {

    [SerializeField] private IUnit _targetUnit;

    [SerializeField] private GUIUnitUnitLevelDataBase _unitLevelData;
    [SerializeField] private Image _unitImage;
    [SerializeField] private Image _earnBar;
    [SerializeField] private Image _lifeSpanBar;

    [SerializeField] private EHText _unitName;
    [SerializeField] private Text _unitLevel;
    [SerializeField] private Text _unitMoneayEarn;
    [SerializeField] private Text _unitPollutionEarn;

    public void SetUnit(IUnit unit)
    {
        _targetUnit = unit;

        _unitName.SetText(unit.GetInfor().UnitName);

        _SetData();

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
        UnitData.LevelData earnData = _targetUnit.GetInfor().GetLevelData(_targetUnit.UnitState.Level);

        _unitLevelData?.SetUnitLevelData(
            _targetUnit.GetInfor(), _targetUnit.UnitState.Level);
        _unitImage.sprite = earnData.Sprite;

        _unitMoneayEarn.text = earnData.EarnMoney.ToString();
        _unitPollutionEarn.text = earnData.EarnEnergy.ToString() + " %";

        _lifeSpanBar.fillAmount = _targetUnit.LifeSpanRatio;
        _earnBar.fillAmount = _targetUnit.EarnRatio;
    }

    public void LevelUp()
    {
        if (_targetUnit.UnitState.Level >= _targetUnit.GetInfor().GetMaxLevel())
        {
            return;
        }

        string[] btnName = { "btn_LevelUp", "btn_Cancel" };

        int cost = _targetUnit.GetInfor().GetLevelData(
            _targetUnit.UnitState.Level + 1).LevelUpCost;

        Action[] callback = new Action[2]{ () => {

            if (_targetUnit == null)
            {
                return;
            }

            if (GameManager.Instance.Field.FieldData.Money < cost) {
                UIManager.Instance.DisplayMessage("msg_NeedMoreMoney");
                Close();
                return;
            }

            _targetUnit.LevelUp(GameManager.Instance.Field.FieldData.SpendTime);
            GameManager.Instance.Field.LogLevelUp(
                _targetUnit.UnitState.Id, cost);

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

        int cost = _targetUnit.GetInfor().GetLevelData(
            _targetUnit.UnitState.Level).RemoveCost;

        Action[] callback = new Action[2]{ () => {

            if (_targetUnit == null)
            {
                return;
            }

            if (GameManager.Instance.Field.FieldData.Money < cost)
            {
                UIManager.Instance.DisplayMessage("msg_NeedMoreMoney");
                Close();
                return;
            }

            _targetUnit.Remove(GameManager.Instance.Field.FieldData.SpendTime);
            GameManager.Instance.Field.LogRemoveUnit(
                _targetUnit, _targetUnit.UnitState.Id, cost);

        },
        () => {
            return;
        } };

        UIManager.Instance.OpenGUI<GUIChoose>("DoubleChoose").Set("label_UnitRemove",
            string.Format(LangManager.Instance.GetStringByKey("msg_CostUseAsk"), cost), btnName, callback);

    }
}
