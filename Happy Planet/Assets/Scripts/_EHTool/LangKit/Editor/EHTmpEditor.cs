#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace EHTool.LangKit {


    [CustomEditor(typeof(EHTmp))]
    public class EHTmpEditor : TMPro.EditorUtilities.TMP_EditorPanelUI {
        SerializedProperty _key;

        [MenuItem("GameObject/EHTool/EHTmp")]
        static void AddEHText()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                canvas = new GameObject("Cavas").AddComponent<Canvas>();
            }
            GameObject newText = new GameObject("EHTmp");
            newText.transform.SetParent(canvas.transform);
            newText.transform.localScale = Vector3.one;
            newText.transform.localPosition = Vector3.zero;
            newText.AddComponent<EHTmp>().SetText("EHText");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _key = serializedObject.FindProperty("_key");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_key);
            serializedObject.ApplyModifiedProperties();

            EHTmp t = target as EHTmp;
            if (GUILayout.Button("Å° Ãß°¡"))
            {
                t.AddKey();
            }

            base.OnInspectorGUI();
        }
    }
}
#endif