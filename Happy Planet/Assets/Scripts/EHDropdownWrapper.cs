using EHTool.LangKit;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class EHDropdownWrapper : MonoBehaviour, IObserver<IEHLangManager> {

    [SerializeField] private string[] _options;
    [SerializeField] private Dropdown _dropdown;

    public Dropdown.DropdownEvent onValueChanged 
        => _dropdown.onValueChanged;

    public int value {
        get {
            return _dropdown.value;
        }
        set {
            _dropdown.value = value;
        }
    }

#nullable enable
    IDisposable? _disposable;

    private void OnEnable() {
        _disposable = LangManager.Instance.Subscribe(this);
    }

    private void OnDisable()
    {
        _disposable?.Dispose();
    }

    public void OnCompleted()
    {

    }

    public void OnError(Exception error)
    {

    }

    public void SetDropdownOption(string[] options) {
        _options = options;
        OnNext(LangManager.Instance);
    }

    public void OnNext(IEHLangManager value)
    {
        Dropdown.DropdownEvent tmp = _dropdown.onValueChanged;

        _dropdown.onValueChanged = new Dropdown.DropdownEvent();

        int idx = _dropdown.value;
        _dropdown.ClearOptions();

        List<Dropdown.OptionData> optionData = new List<Dropdown.OptionData>();

        for (int i = 0; i < _options.Length; i++)
        {
            optionData.Add(new Dropdown.OptionData(
                value.GetStringByKey(_options[i]), null));
        }

        _dropdown.AddOptions(optionData);
        _dropdown.value = idx;
        _dropdown.onValueChanged = tmp;

    }
}