using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIChoose : GUIPopUp {

    [SerializeField] Text _header;
    [SerializeField] Text _content;
    [SerializeField] Text[] _btnName;

    CallbackMethod[] _callbacks;

    public void Set(string header, string contents, string[] btnName, CallbackMethod[] callbacks)
    {
        _header.text = header;
        _content.text = contents;

        _callbacks = callbacks;
        

        for (int i = 0; i < btnName.Length && i < _btnName.Length; i++)
        {
            _btnName[i].text = btnName[i];
        }
    }

    public void ButtonDo(int idx)
    {
        _callbacks[idx]?.Invoke();
        Close();
    }

}
