using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static DataManager;

public partial class DataManager : MonoSingleton<DataManager>
{

    [SerializeField] float _saveRoutine = 1f;

    IList<Log> _logs;
    IList<Log> _logCancled;
    IList<IUnit> _units;

    Log? _lastLog;
    Log? _lastCancled;

    string _path;
    [System.Serializable]
    public class SaveData {
        public float _spendTime = 0;
        public int _money = 0;
        public List<Log> _logs = new List<Log>();
    }

    private void _AddLog(Log log)
    {
        _logs.Add(log);
        _lastLog = log;

        _logCancled = new List<Log>();
        _lastCancled = null;
    }

    private void _PopLog()
    {
        _lastLog.Value.Undo();
        _lastCancled = _lastLog;
        _logCancled.Add(_lastLog.Value);

        _logs.RemoveAt(_logs.Count - 1);
        if (_logs.Count > 0)
        {
            _lastLog = _logs[_logs.Count - 1];
            return;
        }
        _lastLog = null;
    }

    public void AddUnit(IUnit data) {
        _AddLog(new Log(GameManager.Instance.SpendTime, _units.Count,
            new CreateEvent(data.Pos, data.Dir, data.GetInfor().UnitCode)));

        data.SetId(_units.Count);
        _units.Add(data);
        SaveFurniture();
    }

    public void LevelUp(int id) {
        _AddLog(new Log(GameManager.Instance.SpendTime, id, new LevelUpEvent()));
        SaveFurniture();
    }

    public void RemoveUnit(IUnit data, int id) {
        _AddLog(new Log(GameManager.Instance.SpendTime, id,
            new RemoveEvent(data.Pos, data.Dir, data.GetInfor().UnitCode)));
        _units[id] = null;
        SaveFurniture();
    }
    protected override void OnCreate()
    {
        _units = new List<IUnit>();
        _logs = new List<Log>();
        _logCancled = new List<Log>();

        _path = Application.dataPath + "/Resources/UnitData";
    }

    private void Start()
    {
        StartCoroutine(_RoutineDataSave());
    }

    void Update()
    {
        if (_lastLog != null && _lastLog.Value.OccurrenceTime > GameManager.Instance.SpendTime)
        {
            _PopLog();
        }

        if (_lastCancled == null) return;

        if (_lastCancled.Value.OccurrenceTime > GameManager.Instance.SpendTime) return;

        _lastCancled.Value.Action();
        _logs.Add(_lastCancled.Value);
        _lastLog = _lastCancled.Value;

        _logCancled.RemoveAt(_logCancled.Count - 1);

        if (_logCancled.Count > 0)
        {
            _lastCancled = _logCancled[_logCancled.Count - 1];
            return;
        }

        _lastCancled = null;

    }

    IEnumerator _RoutineDataSave()
    {
        while (true) {
            yield return new WaitForSecondsRealtime(_saveRoutine);
            SaveFurniture();
        }

    }

    public void SaveFurniture()
    {
        _JsonWrite();
        //XMLWrite(_path);
    }

    public void MapGenerate()
    {
        _units = new List<IUnit>();
        _logs = new List<Log>();

        _JsonLoad();

        float spendTime = 0;

        foreach (Log log in _logs)
        {
            if (spendTime > GameManager.Instance.SpendTime) break;

            log.Action();
        }
        Debug.Log(_logs.Count);
        if (_logs.Count > 0)
            _lastLog = _logs[_logs.Count - 1];
    }

    void _JsonLoad()
    {

        SaveData data = new SaveData();

        if (!File.Exists(_path + ".json")) {
            GameManager.Instance.AddMoney(1000);
            _JsonWrite();
            return;
        }
        string loadJson = File.ReadAllText(_path + ".json");
        data = JsonUtility.FromJson<SaveData>(loadJson);

        GameManager.Instance.SetInitial(data._spendTime, data._money);
        _logs = data._logs;
        
    }
    void _JsonWrite() {
        //Debug.Log("Write");
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
