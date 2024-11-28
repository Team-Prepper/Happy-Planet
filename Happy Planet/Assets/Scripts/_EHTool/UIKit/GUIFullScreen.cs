using UnityEngine;

namespace EHTool.UIKit {
    public class GUIFullScreen : GUIWindow, IGUIFullScreen {
        
        private IQueue<IGUIPopUp> _popUpStack;
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
            _popUpStack = new StablePriorityQueue<IGUIPopUp>();

            UIManager.Instance.OpenFullScreen(this);
        }

        public override void SetOn()
        {
            base.SetOn();
            _nowPopUp?.SetOn();
            _nowPanel?.SetOn();
        }
        public override void SetOff()
        {
            base.SetOff();

            _nowPopUp?.SetOff();
            _nowPanel?.SetOff();
        }

        public void AddPopUp(IGUIPopUp popUp)
        {
            if (_nowPopUp != null)
            {
                _popUpStack.Enqueue(_nowPopUp);
            }

            _popUpStack.Enqueue(popUp);

            IGUIPopUp tmp = _popUpStack.Dequeue();

            if (tmp == _nowPopUp)
            {
                popUp?.SetOff();
                return;
            }

            _nowPopUp?.SetOff();
            _nowPopUp = popUp;
            _nowPopUp.SetOn();

        }

        public void ClosePopUp(IGUIPopUp popUp) {

            if (_nowPopUp != popUp)
            {
                _popUpStack.Remove(popUp);
                return;
            }

            if (_popUpStack.Count == 0)
            {
                _nowPopUp = null;
                return;
            }

            _nowPopUp = _popUpStack.Dequeue();
            _nowPopUp.SetOn();

        }

        public void AddPanel(IGUIPanel panel)
        {
            _nowPanel?.Close();
            _nowPanel = panel;
        }

        public void ClosePanel()
        {
            _nowPanel = null;
        }

        public override void Close()
        {
            UIManager.Instance.CloseFullScreen(this);

            foreach (IGUIPopUp popup in _popUpStack) { 
                popup.Close();
            }
            _nowPopUp?.Close();

            base.Close();
        }

    }
}