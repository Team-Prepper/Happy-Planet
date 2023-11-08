using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class GaugeValue<T> where T: IComparable, IConvertible {

    [SerializeField] private T _curValue;
    [SerializeField] private T _maxValue;
    [SerializeField] private T _minValue;

    public T Value {
        get { 
            return _curValue; 
        }
    }

    public T MaxValue {
        get {
            return _maxValue;
        }
    }

    public GaugeValue() {
        _curValue = default;
        _maxValue = default;
        _minValue = default;
    }

    public GaugeValue(T curValue)
    {
        _curValue = curValue;
        _maxValue = curValue;
        _minValue = default;
    }

    public GaugeValue(T curValue, T maxValue) : this(curValue)
    {
        _maxValue = maxValue;
    }

    public GaugeValue(T curValue, T maxValue, T minValue) : this(curValue, maxValue)
    {
        _minValue = minValue;
    }


    public void AddValue(T addAmout) {
        _curValue = (_curValue.ConvertTo<double>()
            + addAmout.ConvertTo<double>()).ConvertTo<T>();

        if (_curValue.CompareTo(_maxValue) > 0)
            _curValue = _maxValue;
    }

    public void SubValue(T subAmout)
    {
        _curValue = (_curValue.ConvertTo<double>()
            - subAmout.ConvertTo<double>()).ConvertTo<T>();

        if (_curValue.CompareTo(_minValue) < 0)
            _curValue = _minValue;

    }

    public void FillMax() {
        _curValue = _maxValue;
    }

    public void SetValueMin() {
        _curValue = _minValue;
    }

    public float ConvertToRate()
    {
        return (_curValue.ConvertTo<float>() - _minValue.ConvertTo<float>())
            / (_maxValue.ConvertTo<float>() - _minValue.ConvertTo<float>());
    }

}
