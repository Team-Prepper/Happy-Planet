using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem {
    public interface GUIWindow {
        // Start is called before the first frame update

        public void Open()
        {

        }

        public void Close()
        {

        }

        public virtual void OpenWindow(string key)
        {
            UIManager.Instance.OpenGUI<GUIWindow>(key);
        }

    }

}