using UnityEngine;
using UnityEditor;

public class EasyDebugWindow : EditorWindow
{
    [MenuItem("Tools/EasyDebug")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<EasyDebugWindow>();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Easy Debug Window");
        GUILayout.Space(15);
        GUILayout.Label("Runtime Console");
        if (GUILayout.Button("Init Runtime Console"))
        {
            RuntimeConsole.Create();
        }
        if (GUILayout.Button("Delete Runtime Console"))
        {
            RuntimeConsole.Delete();
        }
        GUILayout.EndVertical();
    }
}
