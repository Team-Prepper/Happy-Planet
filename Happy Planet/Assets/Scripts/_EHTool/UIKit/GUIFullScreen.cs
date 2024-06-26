using System.Collections.Generic;
using UnityEngine;

namespace EHTool.UIKit {
    public class GUIFullScreen : GUIWindow, IGUIFullScreen {

        private IList<IGUIPopUp> _popupUI;
        protected IGUIPopUp _nowPopUp;
        protected IGUIPanel _nowPanel;

        bool _isSetting = false;

        virtual protected void Start()
        {
            if (_isSetting) return;

            Open();
        }

        public override void Open()
        {

            _isSetting = true;

            base.Open();
            gameObject.GetComponent<RectTransform>().sizeDelta = Vector3.zero;
            _popupUI = new List<IGUIPopUp>();

            UIManager.Instance.OpenFullScreen(this);
        }

        public override void SetOn()
        {
            base.SetOn();
            if (_nowPopUp != null)
                _nowPopUp.SetOn();
            if (_nowPanel != null)
                _nowPanel.SetOn();
        }
        public override void SetOff()
        {
            base.SetOff();

            if (_nowPopUp != null)
                _nowPopUp.SetOff();
            if (_nowPanel != null)
                _nowPanel.SetOff();
        }

        public void AddPopUp(IGUIPopUp popUp)
        {
            if (_nowPopUp != null)
            {
                _popupUI.Add(_nowPopUp);
                _nowPopUp.SetOff();
            }
            _nowPopUp = popUp;

        }

        public void PopPopUp() {
            if (_popupUI.Count == 0) {
                _nowPopUp = null;
                return;
            }

            _nowPopUp = _popupUI[_popupUI.Count - 1];
            _nowPopUp.SetOn();
            _popupUI.RemoveAt(_popupUI.Count - 1);
        }
        public void AddPanel(IGUIPanel panel)
        {
            if (_nowPanel != null) {
                _nowPanel.Close();
            }
            _nowPanel = panel;
        }

        public void ClosePanel()
        {
            _nowPanel = null;
        }

        public override void Close()
        {
            UIManager.Instance.CloseFullScreen(this);

            while (_popupUI.Count > 0)
            {
                _nowPopUp.Close();
            }

            base.Close();
        }

    }
}