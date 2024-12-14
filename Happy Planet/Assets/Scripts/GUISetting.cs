using EHTool.LangKit;
using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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


    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _musicMasterSlider;

    private void Start()
    {
        _DropdownSetting();

        for (int i = 0; i < _langOpt.Length; i++)
        {
            if (LangManager.Instance.NowLang.Equals(_langOpt[i].value)) {
                _nowLangIdx = i;
                break;
            }
        }

        _langDropdown.value = _nowLangIdx;
        _langDropdown.onValueChanged.AddListener(LangSet);

        _musicMasterSlider.onValueChanged.AddListener(SetMasterVolume);

        _audioMixer.GetFloat("Master", out float volume);
        _musicMasterSlider.value = Mathf.Pow(10, volume / 20);
    }

    public void LangSet(int idx) {
        if (_nowLangIdx == idx) return;

        LangManager.Instance.ChangeLang(_langOpt[_langDropdown.value].value);
        _DropdownSetting();

        _nowLangIdx = idx;
        _langDropdown.value = idx;

    }

    void _DropdownSetting()
    {
        _langDropdown.ClearOptions();

        List<Dropdown.OptionData> optionData = new List<Dropdown.OptionData>();

        for (int i = 0; i < _langOpt.Length; i++)
        {
            optionData.Add(new Dropdown.OptionData(LangManager.Instance.GetStringByKey(_langOpt[i].name), null));
        }
        _langDropdown.AddOptions(optionData);

    }

    public void SetMasterVolume(float volume)
    {
        if (volume <= _musicMasterSlider.minValue)
        {
            _audioMixer.SetFloat("Master", -80);
            return;

        }

        _audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

}
