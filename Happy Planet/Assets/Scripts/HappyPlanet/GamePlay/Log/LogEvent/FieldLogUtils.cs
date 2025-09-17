using UnityEngine;

public static class FieldLogUtils {
    
    public static string CostumVector3ToString(Vector3 v3)
    {
        return string.Format("{0}/{1}/{2}", v3.x, v3.y, v3.z);
    }

    public static Vector3 CostumStringToVector3(string x, string y, string z)
    {
        return new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
    }

    public static ILogEvent GetEventFromString(string str)
    {

        string[] tocken = str.Split('/');

        switch (tocken[0])
        {
            case "01":
                return new CreateEvent(tocken);
            case "02":
                return new RemoveEvent(tocken);
            default:
                return new LevelUpEvent();

        }
    }

}