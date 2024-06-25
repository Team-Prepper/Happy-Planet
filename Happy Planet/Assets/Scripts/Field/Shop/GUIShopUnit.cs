using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.U2D;
using UnityEngine.UI;
using EHTool.LangKit;
using EHTool.UIKit;

public class GUIShopUnit : MonoBehaviour {

    [SerializeField] GameObject _needLevelPanel;
    [SerializeField] Unit _prefab;

    [SerializeField] Color _canBuyColor;
    [SerializeField] Color _canNotBuyColor;

    [SerializeField] EHText _unitName;
    [SerializeField] Text _needLevelText;
    [SerializeField] Text _moneyAmountText;

    [SerializeField] SpriteAtlas _atlas;
    [SerializeField] Image _unitImage;

    int _price;
    int _needLevel;

    UnitInfor _infor;
    public bool _canBuy;
    
    public void Set(string unitCode, int price, int level)
    {
        _infor = UnitDataManager.Instance.GetUnitData(unitCode);
        _price = price;
        _needLevel = level;

        SetOnScene();
    }

    public void SetOnScene()
    {

        _unitName.SetText(_infor.UnitCode);
        _unitImage.sprite = _infor.GetLevelData(0).Sprite;

        _moneyAmountText.text = _price.ToString();

        _needLevelPanel.SetActive(false);
        _canBuy = true;

    }

    public void Shopping()
    {
        if (!_canBuy)
        {
            UIManager.Instance.DisplayMessage("NeedMoreLevel");
            return;
        }

        if (GameManager.Instance.Money < _price)
        {
            UIManager.Instance.DisplayMessage("NeedMoreMoney");
            return;
        }

        Unit created = Instantiate(_prefab);
        created.SetInfor(_infor, GameManager.Instance.SpendTime, 0);

        UIManager.Instance.OpenGUI<GUIUnitPlace>("UnitPlace").StartEditing(created, _price);

    }
}
