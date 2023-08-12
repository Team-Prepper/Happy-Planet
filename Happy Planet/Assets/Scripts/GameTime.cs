using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTime : MonoBehaviour {

    [SerializeField] SpriteRenderer _background;
    [SerializeField] Color _color;
    [SerializeField] Light _light;

    [SerializeField] float _maxLight;
    [SerializeField] float _minLight;

    float _alpha;

    private void Update() {

        _alpha = Mathf.Cos(GameManager.Instance.SpendTime * Mathf.PI * 2);

        _background.color = new Color(_color.r, _color.g, _color.b, Mathf.Clamp(_alpha, 0.2f, 0.8f));

        _light.intensity = Mathf.Clamp(_alpha, _minLight, _maxLight);

    }

}
