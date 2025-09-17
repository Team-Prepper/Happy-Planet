using System;
using System.Collections.Generic;
using EasyH.Tool.DBKit;

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