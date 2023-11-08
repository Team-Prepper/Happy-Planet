using TMPro;
using UnityEngine;
using ObserberPattern;


namespace LangSystem {

    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("UI/xTMP", 100)]
    [ExecuteAlways]
    public class xTmp : TextMeshProUGUI, IObserver, IStringListener {

        [SerializeField] private string m_Key = string.Empty;

        protected override void OnValidate()
        {
            SetText(m_Key);
            base.OnValidate();
        }

        protected override void OnEnable()
        {
            StringManager.Instance.AddObserver(this);
            SetText(m_Key);
            base.OnEnable();
        }

        override protected void OnDisable()
        {
            StringManager.Instance.RemoveObserver(this);
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            StringManager.Instance.RemoveObserver(this);
            base.OnDestroy();
        }

        public void OnLangChanged()
        {
            SetText(m_Key);
        }

        public void SetText(string key)
        {

            m_Key = key;

            text = StringManager.Instance.GetStringByKey(key);

            if (text.Equals(string.Empty))
            {
                text = key;
            }

        }

        public void Notified() {
            SetText(m_Key);
        }


    }
}
