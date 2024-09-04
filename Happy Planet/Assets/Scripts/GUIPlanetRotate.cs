using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIPlanetRotate : GUIFullScreen
{

    [SerializeField] float _moveDelta;

    FieldCameraSet _cameraSet;

    float _moveAmount;

    Vector3 _lastInputPos;
    float _lastAngle;

    public override void Open()
    {
        base.Open();

        _cameraSet = GameObject.FindWithTag("CameraSet").GetComponent<FieldCameraSet>();
        _moveAmount = -1;
        _lastAngle = _cameraSet.GetAngle();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _CalcTime();


        if (GameManager.Instance.SpendTime < 0 || GameManager.Instance.Energy <= 0)
        {
            _cameraSet.SetRotateSpeed(0);
        }

        if ((_nowPanel != null && _nowPanel.MouseOn()) || !Input.GetMouseButton(0))
        {
            if (_moveAmount < 0)
            {
                return;
            }
            _TouchEnd();
            _moveAmount = -1;
            return;
        }

        _MouseHold();

    }

    void _TouchEnd()
    {
        if (_moveAmount < 0.2f)
        {
            IInteractable target = _GetInteractable();
            if (target == null) return;
            target.Interaction();
        }

        _moveAmount = -1;

    }

    void _CalcTime()
    {

        float gap = (_cameraSet.GetAngle() - _lastAngle) / 360;
        _lastAngle = _cameraSet.GetAngle();

        if (Mathf.Abs(gap) >= 0.5f)
            gap -= Mathf.Sign(gap);

        GameManager.Instance.TimeAdd(gap);

    }

    void _MouseHold()
    {

        if (_moveAmount < 0)
        {
            _lastInputPos = Input.mousePosition;
            _moveAmount = 0;
            return;
        }

        float power = Vector2.Dot((Input.mousePosition - _lastInputPos), _cameraSet.RotateAxis) * _moveDelta;

        _cameraSet.SetRotateSpeed(GameManager.Instance.GetAngularSpeed(power));

        _lastInputPos = Input.mousePosition;

        _moveAmount += Mathf.Abs(power);
    }
    IInteractable _GetInteractable()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.TryGetComponent(out IInteractable retval))
                return retval;
        }
        return null;
    }

}
