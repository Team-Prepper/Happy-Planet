using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem {

    public class GUIPopUp : MonoBehaviour, GUIWindow {

        public virtual void Open()
        {
            if (UIManager.Instance.NowPopUp == null)
            {
                Close();
                return;
            }

            RectTransform rect = gameObject.GetComponent<RectTransform>();

            rect.SetParent(UIManager.Instance.NowPopUp.transform);
        }

        public virtual void Close()
        {
            Destroy(gameObject);
        }
    }

}