using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class FieldCameraSet : MonoBehaviour {

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

    CallbackMethod _callback;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.maxAngularVelocity = 50f;

        Vector3 t = (Camera.main.WorldToScreenPoint(Vector3.up) - Camera.main.WorldToScreenPoint(Vector3.zero)).normalized;

        RotateAxis = Vector3.Cross(t, Vector3.forward);
        _startAngle = transform.eulerAngles.y;
    }

    private void Update()
    {
        _alpha = -Mathf.Cos(GetAngle() / 180f * Mathf.PI) + _padding;

        _background.color = new Color(_color.r, _color.g, _color.b, Mathf.Clamp(_alpha, 0.2f, 0.8f));
        _light.intensity = Mathf.Clamp(_alpha, _minLight, _maxLight);
    }

    public void TimeSet(CallbackMethod callback) {
        _callback = callback;
        StartCoroutine(_RotateCamera(0, 0.5f));
    }

    public void LogSet(CallbackMethod callback)
    {
        _callback = callback;
        StartCoroutine(_RotateCamera(0.5f, 1));
    }

    IEnumerator _RotateCamera(float startRatio, float endRatio)
    {
        float spendTime = startRatio * _duration;
        float goalAngle = (GameManager.Instance.SpendTime - GameManager.Instance.GetDay()) * 360;
        float endTime = endRatio * _duration;

        while (spendTime < endTime) {
            float ratio = -Mathf.Cos(Mathf.PI * (spendTime / _duration)) * 0.5f + 0.5f;

            _camera.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 360, ratio));
            transform.eulerAngles = new Vector3(0, Mathf.Lerp(_startAngle, goalAngle, ratio), transform.eulerAngles.z);

            yield return null;
            spendTime += Time.deltaTime;
        }

        _camera.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 360, endRatio));
        transform.eulerAngles = new Vector3(0, Mathf.Lerp(_startAngle, goalAngle, endRatio), transform.eulerAngles.z);
        _callback();
    }

    public float GetAngle() => transform.eulerAngles.y;

    public Vector2 RotateAxis { get; private set; }

    public void SetRotateSpeed(float speed)
    {
        _rb.angularVelocity = Vector3.up * speed;
    }
}
