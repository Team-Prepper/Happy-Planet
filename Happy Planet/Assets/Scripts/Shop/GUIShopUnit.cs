using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.U2D;
using UnityEngine.UI;
using UISystem;
using LangSystem;

public class GUIShopUnit : MonoBehaviour {

    [SerializeField] GameObject _needLevelPanel;

    [SerializeField] Color _canBuyColor;
    [SerializeField] Color _canNotBuyColor;

    [SerializeField] xText _unitName;
    [SerializeField] Text _needLevelText;
    [SerializeField] Text _moneyAmountText;

    [SerializeField] SpriteAtlas _atlas;
    [SerializeField] Image _unitImage;

    string _unitCode;

    int _price;
    int _needLevel;

    public bool _canBuy;
    
    public void Set(string unitCode, int price, int level)
    {
        _unitCode = unitCode;
        _price = price;
        _needLevel = level;

        SetOnScene();
    }

    public void SetOnScene()
    {
        /*
        ShopManager.Instance.shops.Add(this);
        _unitName.SetText(UnitDataManager.GetStringKey(_unitCode));
        _moneyAmountText.text = _price.ToString();

        _unitImage.sprite = _atlas.GetSprite(UnitDataManager.GetSpriteKey(_unitCode));

        if (EXP.Instance._level < _needLevel)
        {
            _needLevelText.text = string.Format(StringManager.Instance.GetStringByKey("NeedMoreLevel"), _needLevel);
            _needLevelPanel.SetActive(true);
            _canBuy = false;
            return;
        }
        _needLevelPanel.SetActive(false);
        _canBuy = true;
        */
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
        
        Unit created = UnitDataManager.CreateUnit(_unitCode);

        created.transform.localScale = created.transform.localScale * Random.Range(.9f, 1.1f);

        UIManager.Instance.OpenGUI<GUIUnitPlace>("UnitPlace").StartEditing(created, _price);

    }
}
