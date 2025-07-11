using System;
using System.Collections.Generic;
using EHTool.DBKit;
using UnityEngine;

public class FieldDataRecord : IDictionaryable<FieldDataRecord> {

    public float SpendTime { get; private set; }
    public int Energy { get; private set;}
    public int Money { get; private set; }

    public FieldDataRecord()
    {
        SpendTime = 0;
        Money = 0;
        Energy = 100;
    }

    public FieldDataRecord(float spendTime, int money, int energy)
    {
        SpendTime = spendTime;
        Money = money;
        Energy = energy;
    }

    public void SetValueFromDictionary(IDictionary<string, object> value)
    {
        SpendTime = float.Parse(value["_spendTime"].ToString());
        Money = int.Parse(value["_money"].ToString());
        Energy = int.Parse(value["_energy"].ToString());
    }

    public IDictionary<string, object> ToDictionary()
    {
        IDictionary<string, object> retval = new Dictionary<string, object>();

        retval["_spendTime"] = SpendTime;
        retval["_money"] = Money;
        retval["_energy"] = Energy;

        return retval;
    }

}

[Serializable]
public class FieldData
{
    private static readonly int TimeQuantization = 144;

    private int _realEnergy;
    public int Energy => Mathf.Max(0, _realEnergy);

    private float _realSpendTime;
    public float SpendTime { get; private set; }
    public int Money { get; set; }
    public int Day => Mathf.Max(0, Mathf.FloorToInt(SpendTime));

    public static FieldData Default
        => new FieldData(-1f / TimeQuantization, 1000, 100);

    [Newtonsoft.Json.JsonConstructor]
    public FieldData(float spendTime, int money, int energy)
    {
        SpendTime = spendTime;
        Money = money;

        _realSpendTime = spendTime;
        _realEnergy = energy;
    }

    public void AddTime(float amount, Action timeChangeEvent)
    {
        _realSpendTime += amount;

        if (CompareTime(_realSpendTime) == 0) return;

        SpendTime = Mathf.Max(SpendTime = -1f / TimeQuantization,
            Mathf.Round(_realSpendTime * TimeQuantization) / TimeQuantization);

        timeChangeEvent?.Invoke();

    }

    public void AddMoney(int amount) {
        Money += amount;
    }

    public void AddEnergy(int amount) {
        _realEnergy += amount;
    }

    public int CompareTime(float time)
    {
        return CompareTime(SpendTime, time);
    }

    public int CompareTime(float time1, float time2)
    {
        if (Mathf.Abs(time1 - time2) * TimeQuantization < 0.5f)
        {
            return 0;
        }
        if (time1 > time2) return 1;

        return -1;
    }

}