using EHTool.UIKit;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

public class FirestoreTest : GUIFullScreen {


    string message = "{\"0\":{\"TargetId\":2,\"EventStr\":\"01/unit_oak/0.1627661/0.08914978/-3.495076/0.04650461/0.02547148/-0.9985934\",\"Cost\":50,\"OccurrenceTime\":-0.013888888992369175}}";
    public override void Open()
    {
        GetAllRecordCallback(message);
    }
    public void GetAllRecordCallback(string value)
    {

        Debug.Log(value);

        Dictionary<string, object> snapshot = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);

        List<object> jsonList = snapshot.Values.ToList();
        List<DataManager.Log> data = new List<DataManager.Log>();

        foreach (object json in jsonList)
        {
            DataManager.Log temp = default;

            temp.SetValueFromDictionary(JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString()));

            data.Add(temp);

        }

    }

    public void Button1()
    {
    }

    public void Button2()
    {
    }

}
