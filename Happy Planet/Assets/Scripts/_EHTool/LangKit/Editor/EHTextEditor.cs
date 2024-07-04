#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

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
#endif