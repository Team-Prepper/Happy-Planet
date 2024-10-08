using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIPlanetRotate : GUIFullScreen
{

    [SerializeField] float _moveDelta = 12f;

    protected FieldCameraSet _cameraSet;

    float _moveAmount;

    Vector3 _lastInputPos;
    float _lastAngle;

    protected CallbackMethod _touchEvent;

    public override void Open()
    {
        base.Open();

        _cameraSet = GameObject.FindWithTag("CameraSet").GetComponent<FieldCameraSet>();
        _moveAmount = -1;
        _lastAngle = _cameraSet.GetAngle();

        float correction = (Camera.main.WorldToScreenPoint(Vector3.up) - Camera.main.WorldToScreenPoint(Vector3.zero)).magnitude;
        _moveDelta /= correction;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _CalcTime();

        if (GameManager.Instance.Field.SpendTime < 0 || GameManager.Instance.Field.Energy <= 0)
        {
            _cameraSet.FixTo((GameManager.Instance.Field.SpendTime - GameManager.Instance.Field.Day) * 360f);
        }

        if ((_nowPanel != null && _nowPanel.MouseOn()) || !Input.GetMouseButton(0) || _nowPopUp != null)
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
            _touchEvent?.Invoke();
        }

        _moveAmount = -1;

    }

    void _CalcTime()
    {

        float gap = (_cameraSet.GetAngle() - _lastAngle) / 360;
        _lastAngle = _cameraSet.GetAngle();

        if (Mathf.Abs(gap) >= 0.5f)
            gap -= Mathf.Sign(gap);

        GameManager.Instance.Field.TimeAdd(gap);

    }

    void _MouseHold()
    {

        if (Input.GetMouseButtonDown(0))
        {
            _lastInputPos = Input.mousePosition;
            _moveAmount = 0;
            return;
        }

        if (_moveAmount < 0) return;

        Vector3 diff = Input.mousePosition - _lastInputPos;

        float power = Vector2.Dot(diff, _cameraSet.RotateAxis) * _moveDelta;

        _cameraSet.SetRotateSpeed(GameManager.Instance.GetAngularSpeed(power));

        _lastInputPos = Input.mousePosition;

        _moveAmount += Mathf.Abs(power);
    }

}
