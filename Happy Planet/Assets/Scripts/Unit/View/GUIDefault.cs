using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UISystem;


public class GUIDefault : GUIFullScreen {

    Rigidbody _rbCameraSet;
    Transform _trCameraSet;

    [SerializeField] Text _timeText;
    [SerializeField] Text _dayText;
    [SerializeField] Text _moneyText;
    [SerializeField] Text _pollutionText;

    [SerializeField] float _bojung;

    float _moveAmount;

    public bool _ableTouch;

    Vector3 _rotateAxis;
    Vector3 _lastInputPos;

    float _lastAngle;

    protected override void Start()
    {
        base.Start();
        DataManager.Instance.MapGenerate();
        _moveAmount = -1;

        _rbCameraSet = GameObject.FindWithTag("CameraSet").GetComponent<Rigidbody>();
        _trCameraSet = GameObject.FindWithTag("CameraSet").transform;


        _trCameraSet.eulerAngles += Vector3.up * (GameManager.Instance.SpendTime - GameManager.Instance.GetDay()) * 360;

        _lastAngle = _trCameraSet.eulerAngles.y;
        _rotateAxis = new Vector3(_trCameraSet.right.x, -_trCameraSet.right.y);
        _rbCameraSet.maxAngularVelocity = 30f;
    }

    // Update is called once per frame
    void Update()
    {

        float gameTime = GameManager.Instance.SpendTime * 360;
        int planetTime = (int)(gameTime / 15);

        _timeText.text = ((planetTime + 12) % 24).ToString("00") + ":" + (((int)((gameTime % 15) * 4))).ToString("00");

        _dayText.text = string.Format("Day {0}", GameManager.Instance.GetDay());
        _moneyText.text = GameManager.Instance.Money.ToString();
        _pollutionText.text = GameManager.Instance.Pollution.ToString();

        _CalcTime();

        if (Input.GetMouseButton(0))
            _MouseHold();

        if (!Input.GetMouseButtonUp(0))
            return;

        if (_moveAmount < 0.2f)// && _ableTouch && _underPopUps.Count < 1)
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

        float power = Vector2.Dot((Input.mousePosition - _lastInputPos), _rotateAxis) * _bojung;

        _rbCameraSet.angularVelocity = Vector3.up * power;

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
