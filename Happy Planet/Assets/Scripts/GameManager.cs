using EHTool;
using UnityEngine;

public enum Theme {
    Green, Yellow, White
}

public class GameManager : MonoSingleton<GameManager> {

    static readonly int TimeQuantization = 144;

    float _maxRotateSpeed;

    public IAuther Auth { get; set; }

    public IField Field { get; set; }

    protected override void OnCreate()
    {
        Field = new DefaultField();
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
            return Mathf.Min(amount, _maxRotateSpeed / Time.deltaTime) * Field.MaxSpeed;
        }

        if (Field.SpendTime < 0) return 0;
        return Mathf.Max(amount, -_maxRotateSpeed / Time.deltaTime) * Field.MaxSpeed;

    }

}
