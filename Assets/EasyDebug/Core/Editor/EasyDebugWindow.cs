using EasyDebug.Prompts;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EasyDebug
{
    public class EasyDebugWindow : EditorWindow
    {
        private int tab = 0;
        private string[] tabs = new string[] { "General", "CommandLine", "Prompts", "PipeConsole", "Serializer" };
        private static Color bgdefault;
        private static string version = "3.0.1 alpha";
        private Vector2 scroll;

        private bool _alwaysUpdate = true;
        private bool alwaysUpdate 
        { 
            get 
            { 
                return _alwaysUpdate; 
            } 
            set 
            { 
                if (value != _alwaysUpdate)
                {
                    _alwaysUpdate = value;
                    if (_alwaysUpdate) OnEnable();
                    else OnDisable();
                }
            } 
        }

        private ObjectSerializer serializer = new();

        [MenuItem("Tools/EasyDebug")]
        public static void ShowWindow()
        {
            bgdefault = GUI.backgroundColor;
            EditorWindow.GetWindow<EasyDebugWindow>();
        }

        private void DrawTab_General()
        {
            // main body

            GUILayout.Space(20);
            GUILayout.Label(version + "v | Developed by Ananaseek");

            alwaysUpdate = GUILayout.Toggle(alwaysUpdate, "Always update");
        }

        private void DrawTab_CommandLine()
        {
            GUILayout.Label("Runtime Command Line");
            if (GUILayout.Button("Init Command Line"))
            {
                CommandLine.CommandLine.Create();
            }
            if (GUILayout.Button("Delete Command Line"))
            {
                CommandLine.CommandLine.Delete();
            }
            if (GUILayout.Button("Restart Command Line"))
            {
                CommandLine.CommandLine.Delete();
                CommandLine.CommandLine.Create();
            }

            if (GUILayout.Button("Clear Unity Console"))
            {
                PipeConsole.ClearConsole();
            }

            GUILayout.Space(25);
            GUILayout.Label("All found commands:");
            for (int i = 0; i < CommandLine.CommandLine.instance?.engine?.commands.Count; i++)
            {
                GUILayout.Label($"{i}: {CommandLine.CommandLine.instance.engine.commands[i].Serialize()}");
            }
        }

        private void DrawTab_Prompts()
        {
            GUILayout.Label("Runtime gameobject prompts manager");
            TextPromptManager.ShowAll = GUILayout.Toggle(TextPromptManager.ShowAll, "Show all");

            GUILayout.BeginHorizontal();

            GUI.backgroundColor = new Color(1f, 0.65f, 0.68f);

            if (GUILayout.Button("Clear selected"))
            {
                foreach (Object obj in Selection.objects)
                {
                    if (obj is GameObject)
                        TextPromptManager.DestroyAllPrompts(obj as GameObject);
                }
            }

            if (GUILayout.Button("Clear all"))
            {
                foreach (Object obj in TextPromptManager.GetAllGameobjects())
                {
                    if (obj is GameObject)
                        TextPromptManager.DestroyAllPrompts(obj as GameObject);
                }
            }

            GUI.backgroundColor = bgdefault;

            GUILayout.EndHorizontal();
            TextPromptManager.transformMode = (TextPromptTransformMode)EditorGUILayout.EnumPopup(TextPromptManager.transformMode);
        }

        private void DrawTab_PipeConsole()
        {

        }

        private void DrawTab_ObjectSerializer()
        {
            scroll = GUILayout.BeginScrollView(scroll);

            serializer.obj = (GameObject)EditorGUILayout.ObjectField("", serializer.obj, typeof(GameObject), true);

            serializer.allAssemblies = GUILayout.Toggle(serializer.allAssemblies, "All assemblies");
            serializer.onlyScripts = GUILayout.Toggle(serializer.onlyScripts, "Only scripts");

            if (!serializer.allAssemblies)
            {
                GUILayout.Label("Included Assembly Definitions:", EditorStyles.boldLabel);

                // Display the default "Assembly-CSharp" as a non-editable entry
                EditorGUILayout.LabelField("• Assembly-CSharp (default)", EditorStyles.miniLabel);

                if (GUILayout.Button("Add Assembly Definition"))
                {
                    serializer.includedAssemblyDefinitions.Add(null);
                }

                for (int i = 0; i < serializer.includedAssemblyDefinitions.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    serializer.includedAssemblyDefinitions[i] = (AssemblyDefinitionAsset)EditorGUILayout.ObjectField(
                        serializer.includedAssemblyDefinitions[i], typeof(AssemblyDefinitionAsset), false
                    );
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        serializer.includedAssemblyDefinitions.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Refresh Assemblies"))
                {
                    serializer.RefreshAssemblyList();
                }
            }

            if (serializer.obj != null)
            {
                GUILayout.Label(serializer.Serialize(), new GUIStyle { richText = true });
            }

            GUILayout.EndScrollView();
        }


        private void OnEnable()
        {
            EditorApplication.update += ForceRepaint;
        }

        private void OnDisable()
        {
            EditorApplication.update -= ForceRepaint;
        }

        private void ForceRepaint()
        {
            Repaint(); // Forces the editor window to update every frame
        }


        private void OnGUI()
        {
            tab = GUILayout.Toolbar(tab, tabs);
            switch (tab)
            {
                case 0:
                    DrawTab_General();
                    break;
                case 1:
                    DrawTab_CommandLine();
                    break;
                case 2:
                    DrawTab_Prompts();
                    break;
                case 3:
                    DrawTab_PipeConsole();
                    break;
                case 4:
                    DrawTab_ObjectSerializer();
                    break;
            }
        }
    }
}
