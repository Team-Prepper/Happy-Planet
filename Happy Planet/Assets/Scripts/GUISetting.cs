using EHTool.LangKit;
using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUISetting : GUIPopUp
{

    int _nowLangIdx = 0;

    [SerializeField] string[] _langName;
    [SerializeField] Dropdown _langDropdown;

    private void Start()
    {
        DropdownSetting();

        for (int i = 0; i < _langName.Length; i++)
        {
            if (LangManager.Instance.NowLang.CompareTo(_langName[i]) == 0) {
                _nowLangIdx = i;
                break;
            }
        }

        _langDropdown.value = _nowLangIdx;
        _langDropdown.onValueChanged.AddListener(LangSet);
    }

    public void LangSet(int idx) {
        if (_nowLangIdx == idx) return;

        LangManager.Instance.ChangeLang(_langName[_langDropdown.value]);
        DropdownSetting();

        _nowLangIdx = idx;
        _langDropdown.value = idx;

    }

    void DropdownSetting()
    {
        _langDropdown.ClearOptions();

        List<Dropdown.OptionData> optionData = new List<Dropdown.OptionData>();

        for (int i = 0; i < _langName.Length; i++)
        {
            optionData.Add(new Dropdown.OptionData(LangManager.Instance.GetStringByKey(_langName[i]), null));
        }
        _langDropdown.AddOptions(optionData);

    }

}
