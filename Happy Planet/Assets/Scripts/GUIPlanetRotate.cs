using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIPlanetRotate : GUIFullScreen
{

    [SerializeField] float _moveDelta = 12f;
    [SerializeField] float _beepAmount = 10f;

    protected FieldCameraSet _cameraSet;

    float _moveAmount;

    Vector3 _lastInputPos;
    float _lastAngle;
    float _lastTickAngle = 0;
    float _lastTickTime = 0;
    float _lastBeepAmount = 0;

    protected CallbackMethod _touchEvent;

    static readonly int TimeQuantization = 144;
    static readonly float _maxRotateSpeed = 360f * Mathf.Deg2Rad * 20 / TimeQuantization;

    public override void Open()
    {
        base.Open();

        _cameraSet = GameObject.FindWithTag("CameraSet").GetComponent<FieldCameraSet>();
        _moveAmount = -1;
        _lastAngle = _cameraSet.GetAngle();
        _lastTickAngle = _lastAngle;

        float correction = (Camera.main.WorldToScreenPoint(Vector3.up) - Camera.main.WorldToScreenPoint(Vector3.zero)).magnitude;
        _moveDelta /= correction;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _CalcTime();

        if (_moveAmount < 0 && (GameManager.Instance.Field.SpendTime < 0 || GameManager.Instance.Field.Energy <= 0))
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

        if (Mathf.Abs(Mathf.RoundToInt((_lastTickAngle - _lastAngle)) * 0.4f) >= 1)
        {
            _lastTickAngle = _lastAngle;
            if (_lastTickTime + 0.1f < Time.realtimeSinceStartup)
            {
                SoundManager.Instance.PlaySound("Tick", "VFX");
                _lastTickTime = Time.realtimeSinceStartup;
            }
        }

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
            _lastBeepAmount = 0;
            return;
        }

        if (_moveAmount < 0) return;

        Vector3 diff = Input.mousePosition - _lastInputPos;

        float power = Vector2.Dot(diff, _cameraSet.RotateAxis) * _moveDelta;

        GetAngularSpeed(power);

        _lastInputPos = Input.mousePosition;

        _moveAmount += Mathf.Abs(power);
    }

    public void GetAngularSpeed(float amount)
    {
        if (amount == 0) return;
        
        if (amount > 0)
        {
            if (GameManager.Instance.Field.Energy <= 0)
            {
                if (_moveAmount > _lastBeepAmount + _beepAmount)
                {
                    _lastBeepAmount = _moveAmount;
                    SoundManager.Instance.PlaySound("Block", "VFX");
                }
                _cameraSet.FixTo((GameManager.Instance.Field.SpendTime - GameManager.Instance.Field.Day) * 360f);
                return;
            }
            _cameraSet.SetRotateSpeed(
                Mathf.Min(amount, _maxRotateSpeed / Time.deltaTime) * GameManager.Instance.Field.MaxSpeed);
            return;
        }

        if (GameManager.Instance.Field.SpendTime < 0)
        {
            _cameraSet.FixTo((GameManager.Instance.Field.SpendTime - GameManager.Instance.Field.Day) * 360f);
            return;
        }
        _cameraSet.SetRotateSpeed(
            Mathf.Max(amount, -_maxRotateSpeed / Time.deltaTime) * GameManager.Instance.Field.MaxSpeed);

    }

}
