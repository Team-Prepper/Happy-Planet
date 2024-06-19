using UnityEngine;
using UnityEngine.UI;

namespace EHTool.LangKit {

    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("UI/Legacy/EHText", 100)]

    public class EHText : Text, IObserver, IEHText {
        // Start is called before the first frame update
        [SerializeField] private string m_Key = string.Empty;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetText(m_Key);
            base.OnValidate();
        }
#endif
        protected override void OnEnable()
        {
            LangManager.Instance.AddObserver(this);
            SetText(m_Key);
            base.OnEnable();
        }
        override protected void OnDisable()
        {
            LangManager.Instance.RemoveObserver(this);
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            LangManager.Instance.RemoveObserver(this);
            base.OnDestroy();
        }

        public void OnLangChanged()
        {
            SetText(m_Key);
        }

        public void SetText(string key)
        {

            m_Key = key;

            text = LangManager.Instance.GetStringByKey(key);

            if (text.Equals(string.Empty))
            {
                text = key;
            }

        }

        public void Notified()
        {
            SetText(m_Key);
        }
    }
}
