using EHTool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    IDictionary<string, string> _dict;
    IDictionary<string, AudioClip> _audioDict;

    AudioSource _audio;

    protected override void OnCreate()
    {
        IDictionaryConnector<string, string> reader =
            new JsonDictionaryConnector<string, string>();

        _dict = reader.ReadData("SoundInfor");
        _audioDict = new Dictionary<string, AudioClip>();

        _audio = gameObject.AddComponent<AudioSource>();
    }

    public AudioClip GetSound(string key) {

        if (!_audioDict.ContainsKey(key))
        {
            if (!_dict.ContainsKey(key)) return null;

            _audioDict.Add(key, AssetOpener.Import<AudioClip>(_dict[key]));
        }

        return _audioDict[key];
    }

    public void PlaySound(string v)
    {
        _audio.clip = GetSound(v);
        _audio.Play();
    }
}
