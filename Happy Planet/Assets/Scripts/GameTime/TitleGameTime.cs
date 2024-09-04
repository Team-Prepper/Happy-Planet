using UnityEngine;

public class TitleGameTime : IGameTime {

    static readonly int TimeQuantization = 144;

    float _realSpendTime = 0;
    float _spendTime = 0;

    public float RealSpendTime => 0;

    public float SpendTime => _spendTime;

    public int GetDay => Mathf.Max(0, Mathf.FloorToInt(SpendTime));
    public float GetDayTime => SpendTime - GetDay;

    public void AddTime(float amount, int TimeQuantization, CallbackMethod timeChangeEvent)
    {
        _realSpendTime += amount;

    }

    public void TimeSet(float realTime, int TimeQuantization)
    {
        _realSpendTime = realTime;
        _spendTime = Mathf.Round(_realSpendTime * TimeQuantization) / realTime;
    }

}