using EHTool.DBKit;
using System;
using System.Collections.Generic;

public class LogFile
{
    private IDatabaseConnector<Log> _logDBConnector;
    private IList<Log> _logs = new List<Log>();

    private int _validLogCount { get; set; }
    private int _logCursor;

    public bool IsLoaded { get; private set; } = false;

    public Log? Top{
        get {
            if (_logCursor >= _validLogCount)
                return null;
            return _logs[_logCursor];
        }
    }

    public Log? TopUnder {
        get {
            if (_logCursor < 1)
            {
                return null;
            }
            return _logs[_logCursor - 1];
        }
    }

    public void SetDBConnector(IDatabaseConnector<Log> logDataConnector) {
        _logDBConnector = logDataConnector;
        IsLoaded = false;
    }

    public void LoadLog(Action callback, Action<string> fallback) {

        if (IsLoaded) {
            callback?.Invoke();
            return;
        }

        _logDBConnector.GetAllRecord((data) => {
            IsLoaded = true;
            _logs = data;
            
            _logCursor = 0;
            _validLogCount = _logs.Count;
            callback?.Invoke();

        }, (msg) => {
            fallback?.Invoke(msg);
        });

    }

    public void CreateDB() {
        _logDBConnector.UpdateRecordAt(new Log(-1, -1, 0, ""), -1);
    }

    public void AddLog(Log newLog) {
        if (_logCursor < _logs.Count) _logs[_logCursor] = newLog;
        else _logs.Add(newLog);

        _validLogCount = ++_logCursor;
        _logDBConnector.UpdateRecordAt(newLog, _validLogCount - 1);
        
    }

    public void Do(IField targetField) {
        _logs[_logCursor].Action(targetField);
        _logCursor++;
    }

    public void Undo(IField targetField) {
        _logs[--_logCursor].Undo(targetField);
    }

    public void Redo(IField targetField) {
        _logs[_logCursor].Redo(targetField);
        _logCursor++;
    }
    
}