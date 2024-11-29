using UnityEngine;
using EHTool.UIKit;


public class GUIShop : GUIPanel {

    [SerializeField] float _gridSize = 0.4f;
    [SerializeField] float _padding = 32;

    [SerializeField] GUIShopUnit _shopUnit;

    [SerializeField] RectTransform _shopUnitContainer;

    // Start is called before the first frame update
    void Start()
    {
        SetShop();
    }

    void SetShop()
    {
        string[] list = ShopManager.Instance.GetShopItem("Default");

        SetGrid(list.Length * _gridSize + (list.Length + 1) * _padding);

        float gridSize = 1f / list.Length;

        for (int i = 0; i < list.Length; i++) {
            GUIShopUnit shopButton = CreateShopUnit(i * gridSize, (i + 1) * gridSize);

            shopButton.Set(list[i]);
        }

    }

    void SetGrid(float size)
    {
        _shopUnitContainer.sizeDelta = Vector2.right * size;

    }

    GUIShopUnit CreateShopUnit(float startAt, float endAt)
    {
        GUIShopUnit shopUnit = Instantiate(_shopUnit, _shopUnitContainer);

        RectTransform rect = shopUnit.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(startAt, 0);
        rect.anchorMax = new Vector2(endAt, 1);

        return shopUnit;

    }

}