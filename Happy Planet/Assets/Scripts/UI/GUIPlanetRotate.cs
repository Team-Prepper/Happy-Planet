using UnityEngine;
using System;
using EasyH.Unity.UI;
using EasyH.Unity.SoundKit;

public class GUIPlanetRotate : GUIFullScreen
{

    [SerializeField] private float _moveDelta = 12f;
    [SerializeField] private float _beepAmount = 10f;

    protected FieldCameraSet _cameraSet;

    private float _moveAmount;

    private Vector3 _lastInputPos;
    private float _lastAngle;
    private float _lastTickAngle = 0;
    private float _lastTickTime = 0;
    private float _lastBeepAmount = 0;

    protected Action _touchEvent;

    private static readonly int TimeQuantization = 144;
    private static readonly float _maxRotateSpeed = 360f * Mathf.Deg2Rad * 20 / TimeQuantization;

    public override void Open()
    {
        base.Open();

        _cameraSet = GameObject.FindWithTag("CameraSet").GetComponent<FieldCameraSet>();
        _moveAmount = -1;
        _lastAngle = _cameraSet.GetAngle();
        _lastTickAngle = _lastAngle;

        float correction = (Camera.main.WorldToScreenPoint(Vector3.up)
            - Camera.main.WorldToScreenPoint(Vector3.zero)).magnitude;
        _moveDelta /= correction;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _CalcTime();
        
        FieldData fieldData = GameManager.Instance.Field.FieldData;

        if (_moveAmount < 0 && (fieldData.SpendTime < 0 || fieldData.Energy <= 0))
        {
            _cameraSet.FixTo((fieldData.SpendTime - fieldData.Day) * 360f);
        }

        if ((_nowPanel != null && _nowPanel.MouseOn()) ||
            !Input.GetMouseButton(0) || _nowPopUp != null)
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

        GameManager.Instance.Field.AddTime(gap);

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

        FieldData fieldData = GameManager.Instance.Field.FieldData;
        
        if (amount > 0)
        {
            if (fieldData.Energy <= 0)
            {
                if (_moveAmount > _lastBeepAmount + _beepAmount)
                {
                    _lastBeepAmount = _moveAmount;
                    SoundManager.Instance.PlaySound("Block", "VFX");
                }
                _cameraSet.FixTo((fieldData.SpendTime - fieldData.Day) * 360f);
                return;
            }
            _cameraSet.SetRotateSpeed(
                Mathf.Min(amount, _maxRotateSpeed / Time.deltaTime)
                * GameManager.Instance.Field.PlanetData.Speed);
            return;
        }

        if (fieldData.SpendTime < 0)
        {
            _cameraSet.FixTo((fieldData.SpendTime - fieldData.Day) * 360f);
            return;
        }
        _cameraSet.SetRotateSpeed(
            Mathf.Max(amount, -_maxRotateSpeed / Time.deltaTime)
            * GameManager.Instance.Field.PlanetData.Speed);

    }

}
