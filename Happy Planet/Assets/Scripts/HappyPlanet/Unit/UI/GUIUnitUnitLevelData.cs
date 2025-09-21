using UnityEngine;
using UnityEngine.UI;

public class GUIUnitUnitLevelData : GUIUnitUnitLevelDataBase
{
    [SerializeField] private Image _unitImage;
    [SerializeField] private Text _unitMoneayEarn;
    [SerializeField] private Text _unitPollutionEarn;

    protected override void SetGUI(UnitData.LevelData data)
    {
        _unitImage.sprite = data.Sprite;

        _unitMoneayEarn.text =
            data.EarnMoney.ToString();
        _unitPollutionEarn.text = string.Format(
            "{0} %", data.EarnEnergy.ToString());

    }
}