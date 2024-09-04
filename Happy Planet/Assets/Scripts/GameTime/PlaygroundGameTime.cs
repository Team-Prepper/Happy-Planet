using UnityEngine;

public class PlaygroundGameTime : IGameTime {

    static readonly int TimeQuantization = 144;

    float _realSpendTime = 0;
    float _spendTime = 0;

    public float RealSpendTime => _realSpendTime;

    public float SpendTime => _spendTime;

    public int GetDay => Mathf.Max(0, Mathf.FloorToInt(SpendTime));

    public float GetDayTime => SpendTime - GetDay;

    public void AddTime(float amount, int TimeQuantization, CallbackMethod timeChangeEvent)
    {
        _realSpendTime += amount;
        
        if (CheckSameTime(_realSpendTime, SpendTime) != 0)
        {
            _spendTime = Mathf.Round(_realSpendTime * TimeQuantization) / TimeQuantization;
            timeChangeEvent();
        }
    }

    public void TimeSet(float realTime, int TimeQuantization)
    {
        _realSpendTime = realTime;
        _spendTime = Mathf.Round(_realSpendTime * TimeQuantization) / realTime;
    }

    public static int CheckSameTime(float time1, float time2)
    {

        if (Mathf.Abs(time1 - time2) * TimeQuantization < 0.5f)
        {
            return 0;
        }
        if (time1 > time2) return 1;

        return -1;
    
    }

}