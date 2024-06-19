using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EHTool.LangKit {
    [CustomEditor(typeof(EHText))]
    public class EHTextEditor : Editor {

        public override void OnInspectorGUI()
        {
            if (GUI.changed)
                EditorUtility.SetDirty(target);

            base.OnInspectorGUI();
        }
    }
}