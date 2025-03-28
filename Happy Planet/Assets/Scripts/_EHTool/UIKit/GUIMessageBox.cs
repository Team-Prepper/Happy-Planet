using UnityEngine;
using System;

namespace EHTool.UIKit
{
    public abstract class GUIMessageBox : GUIPopUp
    {

        Action _buttonMethod;

        public void SetMessage(string key)
        {
            SetOn();
            SetMessage(key, CloseMessageBox);
        }

        public override void SetOn()
        {
            base.SetOn();
            transform.SetAsLastSibling();
        }

        public override void Close()
        {
            
        }

        public void CloseMessageBox()
        {
            UIManager.Instance.NowDisplay?.ClosePopUp(this);
            SetOff();
        }

        public void SetMessage(string key, Action buttonMethod)
        {
            SetOn();
            ShowMessage(key);

            _buttonMethod = buttonMethod;
        }

        protected abstract void ShowMessage(string key);

        public void MessageBoxButton()
        {
            _buttonMethod?.Invoke();

            if (_buttonMethod != CloseMessageBox) return;

            CloseMessageBox();
        }
    }
}