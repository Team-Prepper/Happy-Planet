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

    string _gameManagerSavePath;
    string _logSavePath;

    [System.Serializable]
    public class GameManagerData {
        public float _spendTime = 0;
        public int _money = 0;
        public int _enegy = 0;

    }

    [System.Serializable]
    public class LogData {
        public List<Log> _logs = new List<Log>();
    }

    private void _AddLog(Log log, int cost)
    {
        _logs.Add(log);
        _lastLog = log;

        _logCancled = new List<Log>();
        _lastCancled = null;

        GameManager.Instance.AddMoney(-cost);

        _JsonWrite();
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

    public void AddUnit(IUnit data, int cost) {
        _AddLog(new Log(GameManager.Instance.SpendTime, _units.Count, cost, new CreateEvent(data)), cost);

        data.SetId(_units.Count);
        _units.Add(data);

    }

    public void LevelUp(int id, int cost)
    {
        _AddLog(new Log(GameManager.Instance.SpendTime, id, cost, new LevelUpEvent()), cost);
    }

    public void RemoveUnit(IUnit data, int id, int cost)
    {
        _AddLog(new Log(GameManager.Instance.SpendTime, id, cost, new RemoveEvent(data)), cost);

        if (id < _units.Count)
        {
            _units[id] = null;
            return;
        }

        _units.RemoveAt(_units.Count - 1);

        while (_units[_units.Count - 1] == null)
        {
            _units.RemoveAt(_units.Count - 1);

        }
    }

    protected override void OnCreate()
    {
        _units = new List<IUnit>();
        _logs = new List<Log>();
        _logCancled = new List<Log>();
#if UNITY_EDITOR
        string BasePath = Application.dataPath + "/Resources";
#else
        string BasePath = Application.persistentDataPath;
#endif

        _gameManagerSavePath = BasePath + "/GameManagerData";
        _logSavePath = BasePath + "/LogData";
    }

    private void Start()
    {
        StartCoroutine(_RoutineDataSave());
    }

    public void TimeChangeEvent(float nowTime)
    {
        while (_lastLog != null && _lastLog.Value.OccurrenceTime > nowTime)
        {
            _PopLog();
        }

        while (_lastCancled != null && _lastCancled.Value.OccurrenceTime <= nowTime) {

            _lastCancled.Value.Redo();
            _logs.Add(_lastCancled.Value);
            _lastLog = _lastCancled.Value;

            _logCancled.RemoveAt(_logCancled.Count - 1);

            if (_logCancled.Count > 0)
            {
                _lastCancled = _logCancled[_logCancled.Count - 1];
                continue;
            }

            _lastCancled = null;
        }

    }

    IEnumerator _RoutineDataSave()
    {
        while (true) {
            yield return new WaitForSecondsRealtime(_saveRoutine);
            _JsonGMWrite();
        }

    }

    public void MapGenerate()
    {
        _units = new List<IUnit>();
        _logs = new List<Log>();

        _JsonLoad();

        foreach (Log log in _logs)
        {
            if (log.OccurrenceTime > GameManager.Instance.SpendTime) {
                _logCancled.Insert(0, log);
                continue;
            }
            log.Action();
        }

        foreach (Log log in _logCancled) {
            _logs.Remove(log);
        }
        if (_logs.Count > 0)
            _lastLog = _logs[_logs.Count - 1];
        if (_logCancled.Count > 0)
            _lastCancled = _logCancled[_logCancled.Count - 1];
    }

    void _JsonLoad()
    {
        if (!File.Exists(_gameManagerSavePath + ".json") || !File.Exists(_logSavePath + ".json")) {
            _JsonWrite();
            return;
        }

        GameManagerData gmData = JsonUtility.FromJson<GameManagerData>(File.ReadAllText(_gameManagerSavePath + ".json"));
        LogData data = JsonUtility.FromJson<LogData>(File.ReadAllText(_logSavePath + ".json"));

        GameManager.Instance.SetInitial(gmData._spendTime, gmData._money, gmData._enegy);
        _logs = data._logs;
        
    }

    void _JsonWrite() {
        _JsonGMWrite();
        _JsonLogWrite();
    }

    void _JsonGMWrite() {

        GameManagerData data = new GameManagerData();
        data._spendTime = GameManager.Instance.SpendTime;
        data._money = GameManager.Instance.Money;
        data._enegy = GameManager.Instance.Energy;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_gameManagerSavePath + ".json", json);

    }

    void _JsonLogWrite() {

        LogData data = new LogData();

        foreach (Log log in _logs) { 
            data._logs.Add(log);
        }

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(_logSavePath + ".json", json);
    }

    void _XMLLoad() {

        GameManager.Instance.SetInitial(PlayerPrefs.GetFloat("SpendTime", 0), PlayerPrefs.GetInt("Money", 0), PlayerPrefs.GetInt("Pollution", 0));

        XmlDocument xmlDoc = AssetOpener.ReadXML(_logSavePath);

        if (xmlDoc == null) return;

        XmlNodeList nodes = xmlDoc.SelectNodes("List/Element");

        for (int i = 0; i < nodes.Count; i++)
        {
            float time = float.Parse(nodes[i].Attributes["Time"].Value);
            int id = int.Parse(nodes[i].Attributes["Id"].Value);
            int money = int.Parse(nodes[i].Attributes["Money"].Value);
            string eventStr = nodes[i].Attributes["Event"].Value;

            _logs.Add(new Log(time, id, money, eventStr));

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
        Document.Save(_logSavePath + ".xml");
    }

}
