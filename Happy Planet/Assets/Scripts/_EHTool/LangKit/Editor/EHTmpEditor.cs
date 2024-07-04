#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace EHTool.LangKit {
    [CustomEditor(typeof(EHTmp))]
    public class EHTmpEditor : Editor {

        public override void OnInspectorGUI()
        {
            if (GUI.changed)
                EditorUtility.SetDirty(target);

            base.OnInspectorGUI();
        }
    }
}
#endif