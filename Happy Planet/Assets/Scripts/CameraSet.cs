using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraSet : MonoBehaviour {

    [SerializeField] Transform _camera;
    [SerializeField] SpriteRenderer _background;
    [SerializeField] Color _color;
    [SerializeField] Light _light;

    [SerializeField] float _maxLight;
    [SerializeField] float _minLight;

    [SerializeField] float _duration = 1f;

    Rigidbody _rb;
    float _alpha;

    CallbackMethod _callback;

    private void Start()
    {

        _rb = GetComponent<Rigidbody>();
        _rb.maxAngularVelocity = 50f;
        

        RotateAxis = (Camera.main.WorldToScreenPoint(Vector3.right) - Camera.main.WorldToScreenPoint(Vector3.zero)).normalized;

    }

    private void Update()
    {

        _alpha = -Mathf.Cos(GetAngle() / 180f * Mathf.PI);

        _background.color = new Color(_color.r, _color.g, _color.b, Mathf.Clamp(_alpha, 0.2f, 0.8f));
        _light.intensity = Mathf.Clamp(_alpha, _minLight, _maxLight);

    }

    public void SetDefault(CallbackMethod callback) {
        _callback = callback;
        StartCoroutine(_RotateCamera());
    }

    IEnumerator _RotateCamera()
    {
        float spendTime = 0;
        float startAngle = transform.eulerAngles.y;
        float goalAngle = (GameManager.Instance.SpendTime - GameManager.Instance.GetDay()) * 360;

        while (spendTime < _duration) {
            float ratio = -Mathf.Cos(Mathf.PI * (spendTime / _duration)) * 0.5f + 0.5f;

            _camera.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 360, ratio));
            transform.eulerAngles = new Vector3(0, Mathf.Lerp(startAngle, goalAngle, ratio), transform.eulerAngles.z);

            yield return null;
            spendTime += Time.deltaTime;
        }

        _camera.localEulerAngles = Vector3.zero;
        transform.eulerAngles = new Vector3(0, goalAngle, transform.eulerAngles.z);
        _callback();
    }

    public float GetAngle() => transform.eulerAngles.y;

    public Vector2 RotateAxis { get; private set; }

    public void SetRotateSpeed(float speed)
    {
        _rb.angularVelocity = Vector3.up * speed;
    }
}
