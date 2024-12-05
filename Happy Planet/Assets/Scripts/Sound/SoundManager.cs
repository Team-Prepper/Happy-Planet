using EHTool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoSingleton<SoundManager>
{
    IDictionary<string, string> _dict;
    IDictionary<string, AudioClip> _audioDict;

    AudioMixer _mixer;
    AudioSource[] _audio;
    int _idx;

    protected override void OnCreate()
    {
        IDictionaryConnector<string, string> reader =
            new JsonDictionaryConnector<string, string>();

        _dict = reader.ReadData("SoundInfor");
        _audioDict = new Dictionary<string, AudioClip>();

        _audio = new AudioSource[5];
        _mixer = Resources.Load("AudioMixer") as AudioMixer;

        for (int i = 0; i < _audio.Length; i++)
        {
            _audio[i] = gameObject.AddComponent<AudioSource>();
        }
        _idx = 0;
    }

    public AudioClip GetSound(string key) {

        if (!_audioDict.ContainsKey(key))
        {
            if (!_dict.ContainsKey(key)) return null;

            _audioDict.Add(key, AssetOpener.Import<AudioClip>(_dict[key]));
        }

        return _audioDict[key];
    }

    public void PlaySound(string v, string group="Master")
    {
        _audio[_idx].clip = GetSound(v);
        _audio[_idx].Play();
        _audio[_idx].outputAudioMixerGroup = _mixer.FindMatchingGroups(group)[0];
        _idx = (_idx + 1) % _audio.Length;
    }
}
