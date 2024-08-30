using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EHTool.UIKit;

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

        _unitPrice = price;
    }

    public void EndEdit()
    {

        string[] btnName = { "설치", "취소" };

        CallbackMethod[] callback = new CallbackMethod[2]{ () => {

            DataManager.Instance.AddUnit(_selectedUnit, _unitPrice);
            _selectedUnit = null;

            Close();

        },
        () => {
            return;
        } };

        UIManager.Instance.OpenGUI<GUIChoose>("DoubleChoose").Set("유닛 설치", string.Format("{0}코인이 소모됩니다. 진행하시겠습니까?", _unitPrice), btnName, callback);

    }

    public void UndoBuy()
    {
        string[] btnName = { "설치 취소", "설치 진행" };

        CallbackMethod[] callback = new CallbackMethod[2]{ () => {

            _selectedUnit.Remove();

            _selectedUnit = null;

            Close();

        },
        () => {
            return;
        } };

        UIManager.Instance.OpenGUI<GUIChoose>("DoubleChoose").Set("유닛 설치 취소", "설치를 취소하시겠습니까?", btnName, callback);

    }

}
