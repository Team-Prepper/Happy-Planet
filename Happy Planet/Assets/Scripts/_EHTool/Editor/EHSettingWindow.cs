using UnityEngine;
using UnityEditor;

namespace EHTool {
    public class MyWindow : EditorWindow {

        string myString = "Hello World";
        bool groupEnabled;
        bool myBool = true;
        float myFloat = 1.23f;
 
        //Window 메뉴에 "My Window" 항목을 추가한다.
        [MenuItem("Window/EHTool")]
        static void Init()
        {
            EditorWindow window = GetWindow(typeof(MyWindow));
            window.Show();
        }
        
        void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField("Text Field", myString);
            
            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle("Toggle", myBool);
            myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            EditorGUILayout.EndToggleGroup();
        }
    }
}