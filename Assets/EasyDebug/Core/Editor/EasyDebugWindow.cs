using UnityEngine;
using UnityEditor;
using EasyDebug;

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
        GUILayout.Label("Runtime Command Line");
        if (GUILayout.Button("Init Command Line"))
        {
            CommandLine.Create();
        }
        if (GUILayout.Button("Delete Command Line"))
        {
            CommandLine.Delete();
        }

        if (GUILayout.Button("Clear Unity Console"))
        {
            PipeConsole.ClearConsole();
        }
        GUILayout.EndVertical();
    }
}
