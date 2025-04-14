using EHTool.UIKit;
using EHTool.LangKit;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GUIUnitPlace : GUIFullScreen {

    [SerializeField] private Text _moneyText;

    private Unit _selectedUnit;
    private int _unitPrice;

    private Vector3 _lastTickPos;

    private bool _isPlaced;

    // Update is called once per frame
    void Update()
    {
        if (_moneyText)
        {
            _moneyText.text = GameManager.Instance.Field.FieldData.Money.ToString();
        }

        if (!_selectedUnit) return;
        if (_nowPopUp != null) return;

        if (!Input.GetMouseButton(0)) return;
        if (MobileUITouchDetector.IsPointerOverUIObject()) return;

        if (!CanPlace(out RaycastHit hit)) return;

        if (!_isPlaced) {
            _isPlaced = true;
            _lastTickPos = hit.point;
            SoundManager.Instance.PlaySound("Place", "VFX");
        }

        if (Vector3.SqrMagnitude(hit.point - _lastTickPos) > 0.1f) {
            _lastTickPos = hit.point;
            SoundManager.Instance.PlaySound("Tick", "VFX");
        }

        _selectedUnit.transform.position = hit.point;
        _selectedUnit.transform.up = hit.normal;

    }

    bool CanPlace(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f, ~(1 << LayerMask.NameToLayer("Unit")))) {
            return true;
        }

        Vector3 point = ray.direction - ray.origin.normalized
            * Vector3.Dot(ray.direction, ray.origin.normalized);
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

        string[] btnName = { "btn_UnitPlacement", "btn_PlacementCancel" };

        Action[] callback = new Action[2]{ () => {

            GameManager.Instance.Field.LogAddUnit(_selectedUnit, _unitPrice);
            _selectedUnit = null;

            Close();

        },
        () => {
            return;
        } };

        UIManager.Instance.OpenGUI<GUIChoose>("DoubleChoose").
            Set("label_UnitPlacement",
            string.Format(LangManager.Instance.GetStringByKey("msg_CostUseAsk"),
            _unitPrice), btnName, callback);

    }

    public void UndoBuy()
    {
        string[] btnName = { "btn_PlacementCancelDo", "btn_PlacementProgress" };

        Action[] callback = new Action[2]{ () => {

            _selectedUnit.Remove
                (GameManager.Instance.Field.FieldData.SpendTime, true);

            _selectedUnit = null;

            Close();

        },
        () => {
            return;
        } };

        UIManager.Instance.OpenGUI<GUIChoose>("DoubleChoose").
            Set("label_UnitPlacementCancel", "msg_PlacementCancelMsg",
            btnName, callback);

    }

}
