using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class DataManager : MonoSingleton<DataManager>
{

    [SerializeField] float _saveRoutine = 1f;

    IList<Log> _logs;
    IList<Unit> _units;
    string _path;

    [System.Serializable]
    public class SaveData {
        public float _spendTime = 0;
        public int _money = 0;
        public List<Log> _logs = new List<Log>();
    }

    public void AddUnit(Unit data) {
        _logs.Add(new Log(GameManager.Instance.SpendTime, _units.Count,
            new CreateEvent(data.transform.position, data.transform.up, data.GetInfor().UnitCode)));

        _units.Add(data);
        SaveFurnitureXML();
    }

    public void LevelUp(int id) {
        _logs.Add(new Log(GameManager.Instance.SpendTime, id, new LevelUpEvent()));
        SaveFurnitureXML();
    }

    public void RemoveUnit(Unit data, int id) {
        _logs.Add(new Log(GameManager.Instance.SpendTime, id,
            new RemoveEvent(data.transform.position, data.transform.up, data.GetInfor().UnitCode)));
        _units[id] = null;
        SaveFurnitureXML();
    }

    private void Start()
    {
        StartCoroutine(_RoutineDataSave());
    }

    IEnumerator _RoutineDataSave()
    {
        while (true) {
            yield return new WaitForSecondsRealtime(_saveRoutine);
            SaveFurnitureXML();
        }

    }

    public void SaveFurnitureXML()
    {
        _JsonWrite();
        //XMLWrite(_path);
    }

    public void MapGenerate()
    {
        _units = new List<Unit>();
        _logs = new List<Log>();

        _JsonLoad();

        float spendTime = 0;

        foreach (Log log in _logs)
        {
            if (spendTime > GameManager.Instance.SpendTime) break;

            log.GetEvent().Action(log.OccurrenceTime, log.TargetId);
        }
    }

    protected override void OnCreate()
    {
        _units = new List<Unit>();
        _logs = new List<Log>();

        _path = Application.dataPath + "/Resources/UnitData";
    }
    void _JsonLoad()
    {
        //GameManager.Instance.SetInitial(PlayerPrefs.GetFloat("SpendTime", 0), PlayerPrefs.GetInt("Money", 0));

        SaveData data = new SaveData();

        if (!File.Exists(_path + ".json")) {
            _JsonWrite();
            return;
        }
        string loadJson = File.ReadAllText(_path + ".json");
        data = JsonUtility.FromJson<SaveData>(loadJson);

        GameManager.Instance.SetInitial(data._spendTime, data._money);
        _logs = data._logs;
        
    }
    void _JsonWrite() {
        Debug.Log("Write");
        SaveData data = new SaveData();

        foreach (Log log in _logs) { 
            data._logs.Add(log);
        }
        data._spendTime = GameManager.Instance.SpendTime;
        data._money = GameManager.Instance.Money;

        string json = JsonUtility.ToJson(data, true);
        //JsonUtility.FromJson<Dictionary<object, object>>(json);

        File.WriteAllText(_path + ".json", json);
    }

    void _XMLLoad() {

        GameManager.Instance.SetInitial(PlayerPrefs.GetFloat("SpendTime", 0), PlayerPrefs.GetInt("Money", 0));

        XmlDocument xmlDoc = AssetOpener.ReadXML(_path);

        if (xmlDoc == null) return;

        XmlNodeList nodes = xmlDoc.SelectNodes("List/Element");

        for (int i = 0; i < nodes.Count; i++)
        {
            float time = float.Parse(nodes[i].Attributes["Time"].Value);
            int id = int.Parse(nodes[i].Attributes["Id"].Value);
            string eventStr = nodes[i].Attributes["Event"].Value;

            _logs.Add(new Log(time, id, eventStr));

        }

    }

    private void _XMLWrite()
    {

        XmlDocument Document = new XmlDocument();
        XmlElement FList = Document.CreateElement("List");
        Document.AppendChild(FList);

        foreach (Log log in _logs)
        {
            XmlElement FElement = Document.CreateElement("Element");

            FElement.SetAttribute("Time", log.OccurrenceTime.ToString());
            FElement.SetAttribute("Id", log.TargetId.ToString());
            FElement.SetAttribute("Event", log.EventStr);

            FList.AppendChild(FElement);
        }
        Document.Save(_path + ".xml");
    }

}
