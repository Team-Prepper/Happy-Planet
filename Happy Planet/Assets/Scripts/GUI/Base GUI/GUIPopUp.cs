using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem {

    public class GUIPopUp : MonoBehaviour, GUIWindow {

        private void Awake()
        {
            Open();
        }

        public virtual void Open()
        {
            if (UIManager.Instance.NowPopUp == null)
            {
                Close();
                return;
            }

            RectTransform rect = gameObject.GetComponent<RectTransform>();

            rect.SetParent(UIManager.Instance.NowPopUp.transform);
            rect.offsetMin = Vector3.zero;
            rect.offsetMax = Vector3.zero;
        }

        public virtual void Close()
        {
            Destroy(gameObject);
        }
    }

}