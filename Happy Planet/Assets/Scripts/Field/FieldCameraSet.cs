using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class FieldCameraSet : MonoBehaviour {

    [System.Serializable]
    public class CameraSettingValue {
        [SerializeField] internal float view;
        [SerializeField] internal Vector3 axis;
    }

    enum State { 
        Idle, Start, TimeSet, LogSet
    }

    State _state;

    [SerializeField] Transform _camera;
    [SerializeField] SpriteRenderer _background;
    [SerializeField] Color _color;
    [SerializeField] Light _light;

    [SerializeField] float _padding = 0.2f;
    [SerializeField] float _maxLight;
    [SerializeField] float _minLight;

    [SerializeField] float _duration = 1f;

    Rigidbody _rb;
    float _alpha;
    float _startAngle;
    float _axis;

    CallbackMethod _callback;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.maxAngularVelocity = 50f;

        Vector3 t = (Camera.main.WorldToScreenPoint(Vector3.up) - Camera.main.WorldToScreenPoint(Vector3.zero)).normalized;
        RotateAxis = Vector3.Cross(t, Vector3.forward);

        _axis = transform.eulerAngles.z;

    }

    private void Update()
    {
        _alpha = -Mathf.Cos(GetAngle() / 180f * Mathf.PI) + _padding;

        _background.color = new Color(_color.r, _color.g, _color.b, Mathf.Clamp(_alpha, 0.2f, 0.8f));
        _light.intensity = Mathf.Clamp(_alpha, _minLight, _maxLight);
    }

    public void StartSet(CallbackMethod callback) {

        if (_state == State.TimeSet || _state == State.LogSet) {
            callback?.Invoke();
            return;
        }

        _callback = callback;
        _callback += () => { _state = State.TimeSet; };

        _startAngle = transform.eulerAngles.y;
        StartCoroutine(_RotateCamera(0, 0.2f));

    }

    public void CameraSet(CameraSettingValue value) {
        Camera.main.fieldOfView = value.view;
        _axis = value.axis.z;
    }

    public void TimeSet(CallbackMethod callback)
    {
        _callback = callback;
        _callback += () => {
            _state = State.LogSet;
        };

        StartCoroutine(_RotateCamera(0.2f, 0.5f));
    }

    public void LogSet(CallbackMethod callback)
    {
        _callback = callback;
        _callback += () => {
            _state = State.Idle;

            Vector3 t = (Camera.main.WorldToScreenPoint(Vector3.up) - Camera.main.WorldToScreenPoint(Vector3.zero)).normalized;
            RotateAxis = Vector3.Cross(t, Vector3.forward);
        };

        StartCoroutine(_RotateCamera(0.5f, 1));
    }

    IEnumerator _RotateCamera(float startRatio, float endRatio)
    {
        float spendTime = startRatio * _duration;
        float goalAngle = (GameManager.Instance.Field.SpendTime - GameManager.Instance.Field.Day) * 360;
        float endTime = endRatio * _duration;

        while (spendTime < endTime) {
            float ratio = -Mathf.Cos(Mathf.PI * (spendTime / _duration)) * 0.5f + 0.5f;

            _camera.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 360, ratio));
            transform.eulerAngles = new Vector3(0, Mathf.Lerp(_startAngle, goalAngle, ratio), _axis);

            yield return null;
            spendTime += Time.deltaTime;
        }

        _camera.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 360, endRatio));
        transform.eulerAngles = new Vector3(0, Mathf.Lerp(_startAngle, goalAngle, endRatio), _axis);
        _callback?.Invoke();
    }

    public float GetAngle() => transform.eulerAngles.y;

    public Vector2 RotateAxis { get; private set; }
    
    public void SetRotateSpeed(float speed)
    {
        if (Mathf.Abs(speed * speed) < Mathf.Abs(_rb.angularVelocity.sqrMagnitude)) return;
        _rb.angularVelocity = Vector3.up * speed;
    }

    public void FixTo(float angle) {
        _rb.angularVelocity = Vector3.zero;
        transform.eulerAngles = new Vector3(0, angle, transform.eulerAngles.z);
    }
}
