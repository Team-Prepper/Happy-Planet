using UnityEngine;
using EHTool.DBKit;
using System.Collections.Generic;

[System.Serializable]
public struct Log : IDictionaryable<Log> {

    public float OccurrenceTime { get; private set; }
    public int TargetId { get; private set; }
    public int Cost { get; private set; }
    public string EventStr { get; private set; }
    
    internal Log(float time, int id, int cost, ILogEvent even)
    {
        OccurrenceTime = Mathf.Max(0, time);
        TargetId = id;
        Cost = cost;
        EventStr = even.ToString();
    }

    [Newtonsoft.Json.JsonConstructor]
    internal Log(float time, int id, int cost, string eventStr)
    {
        OccurrenceTime = Mathf.Max(0, time);
        TargetId = id;
        Cost = cost;
        EventStr = eventStr;
    }

    public void Action(IField target)
    {
        GetEvent().Action(target, OccurrenceTime, TargetId, true);
    }

    public void Undo(IField target)
    {
        GetEvent().Undo(target, OccurrenceTime, TargetId);
        target.AddMoney(Cost);
    }

    public void Redo(IField target)
    {
        GetEvent().Action(target, OccurrenceTime, TargetId);
        target.AddMoney(-Cost);

    }

    public ILogEvent GetEvent()
    {
        return FieldLogUtils.GetEventFromString(EventStr);
    }

    public IDictionary<string, object> ToDictionary()
    {
        IDictionary<string, object> retval = new Dictionary<string, object>();

        retval["OccurrenceTime"] = OccurrenceTime;
        retval["TargetId"] = TargetId;
        retval["Cost"] = Cost;
        retval["EventStr"] = EventStr;

        return retval;
    }

    public override string ToString()
    {
        return string.Format("{0}, {1}, {2}, {3}", OccurrenceTime, TargetId, Cost, EventStr);
    }

    public void SetValueFromDictionary(IDictionary<string, object> value)
    {
        OccurrenceTime = float.Parse(value["OccurrenceTime"].ToString());
        TargetId = int.Parse(value["TargetId"].ToString());
        Cost = int.Parse(value["Cost"].ToString());
        EventStr = value["EventStr"].ToString();
    }

}