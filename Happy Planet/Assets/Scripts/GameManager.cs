using UnityEngine;
using EHTool;

public enum Theme {
    Green, Yellow, White
}

public class GameManager : MonoSingleton<GameManager> {

    static readonly int TimeQuantization = 144;

    public IAuther Auth { get; set; }
    public int Money { get; private set; } = 1000;
    public int Energy { get; private set; } = 100;
    
    float _realSpendTime = 0;
    float _spendTime = 0;

    public float RealSpendTime => _realSpendTime;
    public float SpendTime => _spendTime;

    protected override void OnCreate()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        Auth = new FirebaseAuther();
#else
        Auth = gameObject.AddComponent<FirebaseAuthWebGL>();
#endif
        Auth.Initialize();
    }

    public void TimeAdd(float spendTime) {
        _realSpendTime += spendTime;

        float tmp = Mathf.Round(_realSpendTime * TimeQuantization);

        if (Mathf.Abs(tmp - _spendTime * TimeQuantization) > 0.5f)
        {
            _spendTime = tmp / TimeQuantization;
            DataManager.Instance.TimeChangeEvent(_spendTime);
        }

    }

    public void AddMoney(int earn) {
        Money += earn;
    }

    public int GetDay() {
        return Mathf.Max(0, Mathf.FloorToInt(SpendTime));
    }

    public void AddEnegy(int earn) {
        Energy += earn;

        if (Energy < 0)
            Energy = 0;
    }

    public float GetAngularSpeed(float amount) {

        if (amount > 0) {
            if (Energy <= 0) return 0;

            return 1;
        }

        if (_spendTime < 0) return 0;

        return 1;
    }

    public void SetInitial(float spendTime, int money, int enegy) {
        TimeAdd(spendTime);
        Money = money;
        Energy = enegy;
    }

}
