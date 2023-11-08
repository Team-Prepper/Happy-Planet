using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem {
    public class GUIFullScreen : MonoBehaviour, GUIWindow {

        public int priority = 0;
        public LayerMask selectionMask;

        public virtual void Open()
        {
            gameObject.SetActive(true);

            RectTransform rect = gameObject.GetComponent<RectTransform>();

            rect.SetParent(GameObject.Find("Canvas").transform);
            rect.anchoredPosition = Vector3.zero;
            rect.localScale = Vector3.one;

            UIManager.Instance.EnrollmentGUI(this);
            gameObject.GetComponent<RectTransform>().sizeDelta = Vector3.zero;
        }

        public virtual void Close()
        {
            UIManager.Instance.Pop();
            Destroy(gameObject);
        }

    }
}