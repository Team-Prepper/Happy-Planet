using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UISystem;


public class GUIDefault : GUIFullScreen {

    [SerializeField] Text _timeText;
    [SerializeField] Text _dayText;
    [SerializeField] Text _moneyText;
    [SerializeField] Text _energyText;

    [SerializeField] float _moveDelta;

    Rigidbody _rbCameraSet;
    Transform _trCameraSet;

    float _moveAmount;
    float _lastAngle;

    Vector3 _rotateAxis;
    Vector3 _lastInputPos;

    protected override void Start()
    {
        base.Start();
        DataManager.Instance.MapGenerate();
        _moveAmount = -1;

        _rbCameraSet = GameObject.FindWithTag("CameraSet").GetComponent<Rigidbody>();
        _rbCameraSet.maxAngularVelocity = 50f;

        _trCameraSet = GameObject.FindWithTag("CameraSet").transform;
        _rotateAxis = new Vector3(_trCameraSet.right.x, -_trCameraSet.right.y);

        _trCameraSet.eulerAngles += Vector3.up * (GameManager.Instance.SpendTime - GameManager.Instance.GetDay()) * 360;
        _lastAngle = _trCameraSet.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {

        int gameTime = Mathf.RoundToInt(GameManager.Instance.SpendTime * 1440);

        _timeText.text = string.Format("{0:D2}:{1:D2}", (gameTime / 60 + 12) % 24, gameTime % 60);

        _dayText.text = string.Format("Day {0}", GameManager.Instance.GetDay());
        _moneyText.text = GameManager.Instance.Money.ToString();
        _energyText.text = GameManager.Instance.Energy.ToString();

        _CalcTime();

        if (Input.GetMouseButton(0))
            _MouseHold();

        if (!Input.GetMouseButtonUp(0))
            return;

        if (_moveAmount < 0.2f)
        {
            IInteractable target = _GetInteractable();
            if (target == null) return;
            target.Interaction();
        }
        _moveAmount = -1;

    }

    void _CalcTime() {

        float gap = (_trCameraSet.eulerAngles.y - _lastAngle) / 360;

        if (Mathf.Abs(gap) >= 0.5f)
            gap -= Mathf.Sign(gap);

        GameManager.Instance.TimeAdd(gap);

        _lastAngle = _trCameraSet.eulerAngles.y;
    }

    void _MouseHold() {

        if (Input.GetMouseButtonDown(0)) {
            _lastInputPos = Input.mousePosition;
            _moveAmount = 0;
            return;
        }

        float power = Vector2.Dot((Input.mousePosition - _lastInputPos), _rotateAxis) * _moveDelta;

        _rbCameraSet.angularVelocity = Vector3.up * power * GameManager.Instance.GetAngularSpeed(power);

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
