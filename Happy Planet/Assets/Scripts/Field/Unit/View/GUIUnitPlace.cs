using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EHTool.UIKit;
using EHTool.LangKit;

public class GUIUnitPlace : GUIFullScreen {

    [SerializeField] Unit _selectedUnit;
    int _unitPrice;

    bool _isPlaced;

    // Update is called once per frame
    void Update()
    {
        if (!_selectedUnit) return;
        if (_nowPopUp != null) return;

        if (!Input.GetMouseButton(0)) return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out hit, 100f, ~(1 << LayerMask.NameToLayer("Unit")))) return;

        _isPlaced = true;

        Vector3 v3HitPos = hit.point - hit.transform.position;

        _selectedUnit.transform.position = v3HitPos.normalized * 3.5f;
        _selectedUnit.transform.up = v3HitPos.normalized;

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
