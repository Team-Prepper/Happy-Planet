using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EHTool.UIKit;
using EHTool.LangKit;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIUnitPlace : GUIFullScreen {

    [SerializeField] Text _moneyText;

    Unit _selectedUnit;
    int _unitPrice;

    bool _isPlaced;

    // Update is called once per frame
    void Update()
    {
        if (_moneyText)
        {
            _moneyText.text = GameManager.Instance.Field.Money.ToString();
        }

        if (!_selectedUnit) return;
        if (_nowPopUp != null) return;

        if (!Input.GetMouseButton(0)) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (!CanPlace(out RaycastHit hit)) return;

        _isPlaced = true;

        _selectedUnit.transform.position = hit.point;
        _selectedUnit.transform.up = hit.normal;

    }

    bool CanPlace(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f, ~(1 << LayerMask.NameToLayer("Unit")))) {
            return true;
        }

        Vector3 point = ray.direction - ray.origin.normalized * Vector3.Dot(ray.direction, ray.origin.normalized);
        point *= ray.origin.magnitude;

        if (!Physics.Raycast(point, -point,
            out hit, 100f, ~((1 << LayerMask.NameToLayer("Unit"))))) return false;

        return true;
    }

    public void StartEditing(Unit selected, int price)
    {
        _selectedUnit = selected;
        _selectedUnit.transform.position = Vector3.zero;

        _unitPrice = price;
        _isPlaced = false;
    }

    public void EndEdit()
    {
        if (!_isPlaced) {
            UIManager.Instance.DisplayMessage("msg_NeedPlace");
            return;
        }

        string[] btnName = { "btn_UnitPlacement", "btn_PlacementCancle" };

        CallbackMethod[] callback = new CallbackMethod[2]{ () => {

            GameManager.Instance.Field.AddUnit(_selectedUnit, _unitPrice);
            _selectedUnit = null;

            Close();

        },
        () => {
            return;
        } };

        UIManager.Instance.OpenGUI<GUIChoose>("DoubleChoose").Set("label_UnitPlacement", string.Format(LangManager.Instance.GetStringByKey("msg_CostUseAsk"), _unitPrice), btnName, callback);

    }

    public void UndoBuy()
    {
        string[] btnName = { "btn_PlacementCancleDo", "btn_PlacementProgress" };

        CallbackMethod[] callback = new CallbackMethod[2]{ () => {

            _selectedUnit.Remove(GameManager.Instance.Field.SpendTime, true);

            _selectedUnit = null;

            Close();

        },
        () => {
            return;
        } };

        UIManager.Instance.OpenGUI<GUIChoose>("DoubleChoose").Set("label_PlacementCancle", "msg_PlacementCancleMsg", btnName, callback);

    }

}
