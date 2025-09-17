
using EasyH.Unity.UI;
using EasyH.Tool.LangKit;
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

    [SerializeField] Option[] _langOpt;
    [SerializeField] EHDropdownWrapper _langDropdown;

    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _musicMasterSlider;

    private void Start()
    {
        string[] options = new string[_langOpt.Length];

        int idx = 0;
        for (int i = 0; i < _langOpt.Length; i++)
        {
            if (LangManager.Instance.NowLang.Equals(_langOpt[i].value)) {
                idx = i;
            }
            options[i] = _langOpt[i].name;
        }

        _langDropdown.SetDropdownOption(options);
        _langDropdown.value = idx;
        _langDropdown.onValueChanged.AddListener(LangSet);

        _musicMasterSlider.onValueChanged.AddListener(SetMasterVolume);

        _audioMixer.GetFloat("Master", out float volume);
        _musicMasterSlider.value = Mathf.Pow(10, volume / 20);
    }

    public void LangSet(int idx) {
        LangManager.Instance.ChangeLang(_langOpt[_langDropdown.value].value);
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
