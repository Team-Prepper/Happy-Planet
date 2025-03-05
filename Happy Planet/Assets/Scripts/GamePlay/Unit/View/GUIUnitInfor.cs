using UnityEngine;
using UnityEngine.UI;
using EHTool.UIKit;
using EHTool.LangKit;

public class GUIUnitInfor : GUIPanel {

    [SerializeField] private IUnit _targetUnit;

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

            if (GameManager.Instance.Field.FieldData.Money < cost) {
                UIManager.Instance.DisplayMessage("msg_NeedMoreMoney");
                Close();
                return;
            }

            _targetUnit.LevelUp(GameManager.Instance.Field.FieldData.SpendTime);
            GameManager.Instance.Field.LogLevelUp(_targetUnit.Id, cost);

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

            if (GameManager.Instance.Field.FieldData.Money < cost)
            {
                UIManager.Instance.DisplayMessage("msg_NeedMoreMoney");
                Close();
                return;
            }

            _targetUnit.Remove(GameManager.Instance.Field.FieldData.SpendTime);
            GameManager.Instance.Field.LogRemoveUnit(_targetUnit, _targetUnit.Id, cost);

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
