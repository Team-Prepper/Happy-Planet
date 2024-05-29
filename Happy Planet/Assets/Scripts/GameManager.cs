using UnityEngine;

public enum Theme {
    Green, Yellow, White
}

public class GameManager : Singleton<GameManager> {

    static readonly int TimeQuantization = 144;
    public int Money { get; private set; } = 1000;

    public int Pollution { get; private set; } = 100;

    float _realSpendTime = 0;
    float _spendTime = 0;

    public float SpendTime {
        
        get {
            if (_spendTime < 0)
                return 0;

            return _spendTime;
        }
    }

    public void TimeAdd(float spendTime) {
        _realSpendTime += spendTime;

        float tmp = Mathf.Round(_realSpendTime * TimeQuantization);

        if (tmp != _spendTime * TimeQuantization) {
            _spendTime = tmp / TimeQuantization;
            DataManager.Instance.TimeChangeEvent();
        }

    }

    public void AddMoney(int earn) {
        Money += earn;
    }

    public int GetDay() {
        return Mathf.FloorToInt(SpendTime);
    }

    public void AddPollution(int earn) {
        Pollution += earn;

        if (Pollution < 0)
            Pollution = 0;
        else if (Pollution > 100)
            Pollution = 100;
    }

    public void SetInitial(float spendTime, int money) {
        TimeAdd(spendTime);
        Money = money;
    }

}
