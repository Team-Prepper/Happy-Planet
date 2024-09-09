using UnityEngine;
using EHTool;
using System.Collections;
using System.Collections.Generic;

public enum Theme {
    Green, Yellow, White
}

public class GameManager : MonoSingleton<GameManager> {

    static readonly int TimeQuantization = 144;

    float _maxRotateSpeed;
    
    public IAuther Auth { get; set; }

    public IList<IField> _fieldStack = new List<IField>();

    public IField Field {
        get {
            if (_fieldStack.Count == 0) { 
                _fieldStack.Add(new DefaultField());
            }
            return _fieldStack[_fieldStack.Count - 1];
        }
        set {
            if (value == null)
            {
                _fieldStack.RemoveAt(_fieldStack.Count - 1);
                return;
            }
            if (Field == value) return;

            _fieldStack.Add(value);
        }
    }

    protected override void OnCreate()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        Auth = new FirebaseAuther();
#else
        Auth = gameObject.AddComponent<FirebaseAuthWebGL>();
#endif
        Auth.Initialize();
        _maxRotateSpeed = 360f * Mathf.Deg2Rad * 20 / TimeQuantization;
    }

    public float GetAngularSpeed(float amount)
    {

        if (amount > 0)
        {
            if (Field.Energy <= 0) return 0;
            return Mathf.Min(amount, _maxRotateSpeed / Time.deltaTime);
        }

        if (Field.SpendTime < 0) return 0;
        return Mathf.Max(amount, -_maxRotateSpeed / Time.deltaTime);

    }

}
