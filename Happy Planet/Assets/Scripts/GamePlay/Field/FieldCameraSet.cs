using System.Collections;
using UnityEngine;
using System;

public class FieldCameraSet : MonoBehaviour {

    [System.Serializable]
    public class CameraSettingValue {
        [SerializeField] internal float view;
        [SerializeField] internal Vector3 axis;

        public CameraSettingValue(float view, Vector3 axis) {
            this.view = view;
            this.axis = axis;
        }
    }

    enum State { 
        Idle, Start, TimeSet, LogSet
    }

    private State _state;

    [SerializeField] private Transform _camera;
    [SerializeField] private SpriteRenderer _background;
    [SerializeField] private Color _color;
    [SerializeField] private Light _light;

    [SerializeField] private float _padding = 0.2f;
    [SerializeField] private float _maxLight;
    [SerializeField] private float _minLight;

    [SerializeField] float _duration = 1f;

    private Rigidbody _rb;
    private float _alpha;
    private float _startAngle;
    private float _axis;

    private Action _callback;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.maxAngularVelocity = 50f;

        Vector3 t = (Camera.main.WorldToScreenPoint(Vector3.up)
            - Camera.main.WorldToScreenPoint(Vector3.zero)).normalized;
        RotateAxis = Vector3.Cross(t, Vector3.forward);

        _axis = transform.eulerAngles.z;

    }

    private void Update()
    {
        _alpha = -Mathf.Cos(GetAngle() / 180f * Mathf.PI) + _padding;

        _background.color = new Color(_color.r, _color.g, _color.b, Mathf.Clamp(_alpha, 0.2f, 0.8f));
        _light.intensity = Mathf.Clamp(_alpha, _minLight, _maxLight);
    }

    public void StartSet(Action callback) {

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

    public void TimeSet(Action callback)
    {
        _callback = callback;
        _callback += () => {
            _state = State.LogSet;
        };

        StartCoroutine(_RotateCamera(0.2f, 0.5f));
    }

    public void LogSet(Action callback)
    {
        _callback = callback;
        _callback += () => {
            _state = State.Idle;

            Vector3 t = (Camera.main.WorldToScreenPoint(Vector3.up)
                - Camera.main.WorldToScreenPoint(Vector3.zero)).normalized;
            RotateAxis = Vector3.Cross(t, Vector3.forward);
        };

        StartCoroutine(_RotateCamera(0.5f, 1));
    }

    IEnumerator _RotateCamera(float startRatio, float endRatio)
    {
        float spendTime = startRatio * _duration;
        float goalAngle = (GameManager.Instance.Field.FieldData.SpendTime
            - GameManager.Instance.Field.FieldData.Day) * 360;
        float endTime = endRatio * _duration;

        while (spendTime < endTime) {
            float ratio = -Mathf.Cos(Mathf.PI * (spendTime / _duration)) * 0.5f + 0.5f;

            _camera.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 360, ratio));
            transform.eulerAngles = new Vector3
                (0, Mathf.Lerp(_startAngle, goalAngle, ratio), _axis);

            yield return null;
            spendTime += Time.deltaTime;
        }

        transform.eulerAngles = new Vector3
            (0, Mathf.Lerp(_startAngle, goalAngle, endRatio), _axis);

        _camera.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 360, endRatio));
        _callback?.Invoke();
    }

    public float GetAngle() => transform.eulerAngles.y;

    public Vector2 RotateAxis { get; private set; }
    
    public void SetRotateSpeed(float speed)
    {
        if (speed * _rb.angularVelocity.y < 0) {
            _rb.angularVelocity += Vector3.up * speed;
            return;
        }
        if (speed * speed < _rb.angularVelocity.sqrMagnitude) return;
        _rb.angularVelocity = Vector3.up * speed;
    }

    public void FixTo(float angle) {
        _rb.angularVelocity = Vector3.zero;
        transform.eulerAngles = new Vector3(0, angle, transform.eulerAngles.z);
    }
}
