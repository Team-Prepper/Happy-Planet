#if UNITY_EDITOR
using System.CodeDom.Compiler;
using UnityEditor;
using UnityEngine;

namespace EHTool.LangKit {

    [CanEditMultipleObjects]
    [CustomEditor(typeof(EHText))]
    public class EHTextEditor : UnityEditor.UI.TextEditor {

        SerializedProperty _key;

        [MenuItem("GameObject/EHTool/EHText")]
        static void AddEHText()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                canvas = new GameObject("Cavas").AddComponent<Canvas>();
            }
            GameObject newText = new GameObject("EHText");
            newText.transform.SetParent(canvas.transform);
            newText.transform.localScale = Vector3.one;
            newText.transform.localPosition = Vector3.zero;
            newText.AddComponent<EHText>().SetText("EHText");
        }

        protected override void OnEnable()
        {
            _key = serializedObject.FindProperty("_key");
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_key);
            serializedObject.ApplyModifiedProperties();


            EHText t = target as EHText;
            if (GUILayout.Button("Å° Ãß°¡"))
            {
                t.AddKey();
            }

            base.OnInspectorGUI(); 

        }
    }
}
#endif