using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EHTool.UIKit;
using System;


public class GUIDefault : GUIFullScreen {

    [SerializeField] Text _timeText;
    [SerializeField] Text _dayText;
    [SerializeField] Text _moneyText;
    [SerializeField] Text _energyText;

    [SerializeField] float _moveDelta;

    CameraSet _cameraSet;

    float _moveAmount;
    float _lastAngle;

    Vector3 _lastInputPos;

    bool _isActive = false;

    protected override void Start()
    {
        base.Start();
        DataManager.Instance.MapGenerate(_MapGenerateCallback);
    }

    void _MapGenerateCallback()
    {
        _moveAmount = -1;

        _cameraSet = GameObject.FindWithTag("CameraSet").GetComponent<CameraSet>();
        
        _cameraSet.SetDefault(_TimeSettingCallback);
    }

    void _TimeSettingCallback() {

        _isActive = true;
        _lastAngle = _cameraSet.GetAngle();

    }

    // Update is called once per frame
    void Update()
    {
        if (!_isActive) return;

        int gameTime = Mathf.Max(0, Mathf.RoundToInt(GameManager.Instance.SpendTime * 1440));

        _timeText.text = string.Format("{0:D2}:{1:D2}", (gameTime / 60) % 24, gameTime % 60);

        _dayText.text = string.Format("Day {0}", GameManager.Instance.GetDay());
        _moneyText.text = GameManager.Instance.Money.ToString();
        _energyText.text = GameManager.Instance.Energy.ToString();

        _CalcTime();

        if (Input.GetMouseButton(0))
        {
            _MouseHold();
        }
        else if (gameTime <= 0)
        {
            _cameraSet.SetRotateSpeed(0);
        }

        if (!Input.GetMouseButtonUp(0))
        {
            return;

        }

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

        if (Mathf.Abs(gap) >= 0.5f)
            gap -= Mathf.Sign(gap);

        GameManager.Instance.TimeAdd(gap);

        _lastAngle = _cameraSet.GetAngle();
    }

    void _MouseHold()
    {

        if (Input.GetMouseButtonDown(0))
        {
            _lastInputPos = Input.mousePosition;
            _moveAmount = 0;
            return;
        }

        float power = Vector2.Dot((Input.mousePosition - _lastInputPos), _cameraSet.RotateAxis) * _moveDelta;

        _cameraSet.SetRotateSpeed(power * GameManager.Instance.GetAngularSpeed(power));

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
