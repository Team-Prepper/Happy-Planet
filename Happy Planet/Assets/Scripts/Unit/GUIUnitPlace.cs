using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIUnitPlace : GUIFullScreen {

    Unit _selectedUnit;
    int _unitPrice;

    protected override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_selectedUnit) return;

        if (Input.GetMouseButton(0))
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {

                Vector3 v3HitPos = hit.point - hit.transform.position;

                //Debug.Log(hit.transform.tag);

                _selectedUnit.transform.position = v3HitPos.normalized * 3.5f;
                _selectedUnit.transform.up = v3HitPos.normalized;

            }
            return;
        }

    }

    public void StartEditing(Unit selected, int price)
    {
        _selectedUnit = selected;
        _selectedUnit.transform.position = Vector3.zero;

        //GameManager.money -= price;
        _unitPrice = price;
        //transform.eulerAngles = trCameraSet.eulerAngles;
    }

    public void EndEdit()
    {
        UnitSaveManager.Instance.AddUnit(_selectedUnit);

        _selectedUnit = null;

        Close();
    }

    public void UndoBuy()
    {
        Destroy(_selectedUnit.gameObject);
        //GameManager.money += _unitPrice;

        _selectedUnit = null;
        Close();
    }

}
