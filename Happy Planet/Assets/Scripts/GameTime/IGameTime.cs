public interface IGameTime {


    public float RealSpendTime { get; }
    public float SpendTime { get; }
    public int GetDay { get; }
    public float GetDayTime { get; }

    public void TimeSet(float realtime, int TimeQuantization);
    public void AddTime(float amount, int TimeQuantization, CallbackMethod timeChangeEvent);
}