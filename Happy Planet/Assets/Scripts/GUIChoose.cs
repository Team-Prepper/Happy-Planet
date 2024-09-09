using EHTool.LangKit;
using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIChoose : GUIPopUp {

    [SerializeField] EHText _header;
    [SerializeField] EHText _content;
    [SerializeField] EHText[] _btnName;

    CallbackMethod[] _callbacks;

    public void Set(string header, string contents, string[] btnName, CallbackMethod[] callbacks)
    {
        _header.SetText(header);
        _content.SetText(contents);

        _callbacks = callbacks;
        

        for (int i = 0; i < btnName.Length && i < _btnName.Length; i++)
        {
            _btnName[i].SetText(btnName[i]);
        }
    }

    public void ButtonDo(int idx)
    {
        Close();
        _callbacks[idx]?.Invoke();
    }

}
