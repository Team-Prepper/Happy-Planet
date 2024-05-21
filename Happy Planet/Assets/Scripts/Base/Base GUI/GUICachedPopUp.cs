using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem {

    public class GUICachedPopUp : GUIWindow, IGUIPopUp {

        public override void Open()
        {
            base.Open();

            PopUpAction();
        }

        protected void PopUpAction()
        {
            if (UIManager.Instance.NowDisplay == null)
            {
                return;
            }

            UIManager.Instance.NowDisplay.AddPopUp(this);

        }

        public override void Close()
        {
            UIManager.Instance.NowDisplay.PopPopUp();
            base.Close();
        }
    }

}