using UnityEngine;
using EHTool;

public enum Theme {
    Green, Yellow, White
}

public class GameManager : MonoSingleton<GameManager> {

    static readonly int TimeQuantization = 144;

    float _maxRotateSpeed;
    
    public IAuther Auth { get; set; }
    public int Money { get; private set; } = 1000;
    public int Energy { get; private set; } = 100;
    
    float _realSpendTime = 0;
    float _spendTime = 0;

    public float RealSpendTime => _realSpendTime;
    public float SpendTime => _spendTime;

    public static int CheckSameTime(float time1, float time2)
    {

        if (Mathf.Abs(time1 - time2) * TimeQuantization < 0.5f)
        {
            return 0;
        }
        if (time1 > time2) return 1;

        return -1;

    }

    protected override void OnCreate()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        Auth = new FirebaseAuther();
#else
        Auth = gameObject.AddComponent<FirebaseAuthWebGL>();
#endif
        Auth.Initialize();
        _maxRotateSpeed = 360f * Mathf.Deg2Rad * 4 / TimeQuantization;
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

    public float GetAngularSpeed(float amount)
    {

        if (amount > 0)
        {
            if (Energy <= 0) return 0;
            return Mathf.Min(amount, _maxRotateSpeed / Time.deltaTime);
        }

        if (SpendTime < 0) return 0;
        return Mathf.Max(amount, -_maxRotateSpeed / Time.deltaTime);
    }

    public void SetInitial(float spendTime, int money, int enegy) {
        TimeAdd(spendTime);
        Money = money;
        Energy = enegy;
    }

}
