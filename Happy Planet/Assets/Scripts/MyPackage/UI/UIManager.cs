using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

class Heap<T>{
    List<T> list;
    public T Pop() {
        return list[0];
    }

    public void Add(T data, int value) { 
        
    }
}

public class UIManager : MonoSingleton<UIManager> {

    List<GUIFullScreen> uiStack;

    public GUIFullScreen NowPopUp { get; set; }
    public GUIInforBox InforBox { get; set; }
    
    public void EnrollmentGUI(GUIFullScreen newData)
    {
        if (NowPopUp == null)
        {
            NowPopUp = newData;
            return;

        }
        else
        {
            NowPopUp.gameObject.SetActive(false);
            uiStack.Add(NowPopUp);
            uiStack.Add(newData);

        }
        Pop();

    }

    public void Pop()
    {
        InforBox.transform.SetParent(null);

        NowPopUp = uiStack[uiStack.Count - 1];
        uiStack.RemoveAt(uiStack.Count - 1);
        NowPopUp.gameObject.SetActive(true);

        InforBox.transform.SetParent(NowPopUp.transform);

    }

    class GUIData {
        internal string name;
        internal string path;

        internal void Read(XmlNode node)
        {
            name = node.Attributes["name"].Value;
            path = node.Attributes["path"].Value;
        }
    }

    Dictionary<string, GUIData> _dic;

    protected override void OnCreate()
    {
        NowPopUp = null;
        uiStack = new List<GUIFullScreen>();

        _dic = new Dictionary<string, GUIData>();
        XmlDocument xmlDoc = AssetOpener.ReadXML("GUIInfor");

        XmlNodeList nodes = xmlDoc.SelectNodes("GUIInfor/GUIData");

        for (int i = 0; i < nodes.Count; i++)
        {
            GUIData guiData = new GUIData();
            guiData.Read(nodes[i]);

            _dic.Add(guiData.name, guiData);
        }
    }
    public static T OpenGUI<T>(string guiName)
    {
        string path = Instance._dic[guiName].path;
        T result = AssetOpener.ImportGameObject(path).GetComponent<T>();

        return result;
    }
}
