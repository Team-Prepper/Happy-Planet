
using EasyH.Unity.UI;
using EasyH.Tool.LangKit;
using UnityEngine;
using System;

public class GUIChoose : GUIPopUp {

    [SerializeField] private EHText _header;
    [SerializeField] private EHText _content;
    [SerializeField] private EHText[] _btnName;

    private Action[] _callbacks;

    public void Set(string header, string contents, string[] btnName, Action[] callbacks)
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
