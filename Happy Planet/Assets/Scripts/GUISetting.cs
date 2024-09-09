using EHTool.LangKit;
using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUISetting : GUIPopUp
{
    [System.Serializable]
    struct Option {
        public string name;
        public string value;
    }

    int _nowLangIdx = 0;

    [SerializeField] Option[] _langOpt;
    [SerializeField] Dropdown _langDropdown;

    private void Start()
    {
        DropdownSetting();

        for (int i = 0; i < _langOpt.Length; i++)
        {
            if (LangManager.Instance.NowLang.CompareTo(_langOpt[i].value) == 0) {
                _nowLangIdx = i;
                break;
            }
        }

        _langDropdown.value = _nowLangIdx;
        _langDropdown.onValueChanged.AddListener(LangSet);
    }

    public void LangSet(int idx) {
        if (_nowLangIdx == idx) return;

        LangManager.Instance.ChangeLang(_langOpt[_langDropdown.value].value);
        DropdownSetting();

        _nowLangIdx = idx;
        _langDropdown.value = idx;

    }

    void DropdownSetting()
    {
        _langDropdown.ClearOptions();

        List<Dropdown.OptionData> optionData = new List<Dropdown.OptionData>();

        for (int i = 0; i < _langOpt.Length; i++)
        {
            optionData.Add(new Dropdown.OptionData(LangManager.Instance.GetStringByKey(_langOpt[i].name), null));
        }
        _langDropdown.AddOptions(optionData);

    }

}
