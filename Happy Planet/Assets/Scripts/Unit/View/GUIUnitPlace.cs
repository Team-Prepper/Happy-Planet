using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
using static UnityEngine.GraphicsBuffer;

public class GUIUnitPlace : GUIFullScreen {

    [SerializeField] Unit _selectedUnit;
    int _unitPrice;

    // Update is called once per frame
    void Update()
    {
        if (!_selectedUnit) return;

        if (!Input.GetMouseButton(0)) return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out hit, 100f, ~(1 << LayerMask.NameToLayer("Unit")))) return;

        Vector3 v3HitPos = hit.point - hit.transform.position;

        _selectedUnit.transform.position = v3HitPos.normalized * 3.5f;
        _selectedUnit.transform.up = v3HitPos.normalized;

    }

    public void StartEditing(Unit selected, int price)
    {
        _selectedUnit = selected;
        _selectedUnit.transform.position = Vector3.zero;
        //
        _unitPrice = price;
        //transform.eulerAngles = trCameraSet.eulerAngles;
    }

    public void EndEdit()
    {
        DataManager.Instance.AddUnit(_selectedUnit);
        GameManager.Instance.AddMoney(-_unitPrice);

        _selectedUnit = null;

        Close();
    }

    public void UndoBuy()
    {
        _selectedUnit.Remove();

        _selectedUnit = null;
        Close();
    }

}
