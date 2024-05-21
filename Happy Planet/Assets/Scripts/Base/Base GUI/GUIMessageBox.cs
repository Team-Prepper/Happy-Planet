using UnityEngine;
using System;
using LangSystem;

namespace UISystem {
    public class GUIMessageBox : GUIPopUp {

        [SerializeField] xText _textField;

        Action _buttonMethod;

        public void SetMessage(string key)
        {
            SetMessage(key, CloseMessageBox);
        }

        public void SetMessage(string key, Action buttonMethod)
        {
            SetOn();
            _textField.SetText(key);

            _buttonMethod = buttonMethod;
        }

        public void MessageBoxButton()
        {
            _buttonMethod.Invoke();

            if (_buttonMethod != CloseMessageBox) return;

            CloseMessageBox();
        }

        public void CloseMessageBox()
        {
            Close();
        }

    }
}