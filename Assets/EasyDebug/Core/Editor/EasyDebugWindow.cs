using EasyDebug.Prompts;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

namespace EasyDebug
{
    public class EasyDebugWindow : EditorWindow
    {
        private int _tab = 0;
        private int tab
        {
            get
            {
                return _tab;
            }
            set
            {
                if (_tab != value)
                {
                    onTabChangedEvent?.Invoke();
                }
                _tab = value;
            }
        }
        public event System.Action onTabChangedEvent;
        private string[] tabs = new string[] { "General", "CommandLine", "Prompts", "PipeConsole", "Serializer" };
        private static Color bgdefault;
        private static string version = "3.1.0";
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
            EditorWindow.GetWindow<EasyDebugWindow>().SafeInit();
        }

        private void OnValidate()
        {
            SafeInit();
        }

        /// <summary>
        /// Init safe from multi-calls
        /// </summary>
        public void SafeInit()
        {
            ThemeManager.SafeInit();
            ThemeManager.SetTheme(EditorPrefs.GetInt("themeIndex"));


            TextPromptManager.ShowAll = EditorPrefs.GetBool("prompts_showAll");
            serializer.allAssemblies = EditorPrefs.GetBool("serializer_allAssemblies");
            serializer.onlyScripts = EditorPrefs.GetBool("serializer_onlyScripts");
            serializer.showStatic = EditorPrefs.GetBool("serializer_showStatic");
            serializer.showFields = EditorPrefs.GetBool("serializer_showFields");
            serializer.collection_forceNewLine = EditorPrefs.GetBool("serializer_collection_forceNewLine");
            serializer.unfoldSerializable = EditorPrefs.GetBool("serializer_unfoldSerializable");
            serializer.serializable_forceNewLine = EditorPrefs.GetBool("serializer_serializable_forceNewLine");
            serializer.collection_maxLimit = EditorPrefs.GetInt("serializer_collection_maxLimit");
        }

        private void OnEnable()
        {
            EditorApplication.update += ForceRepaint;
            SafeInit();
        }

        private void OnDisable()
        {
            EditorApplication.update -= ForceRepaint;
        }

        private void ForceRepaint()
        {
            Repaint(); // Forces the editor window to update every frame
        }

        private void OnTabChanged()
        {
            scroll = Vector2.zero;
        }

        private void DrawTab_General()
        {
            // main body

            GUILayout.Space(20);
            GUILayout.Label(version + "v | Developed by Ananaseek");

            alwaysUpdate = GUILayout.Toggle(alwaysUpdate, "Always update");

            EditorGUILayout.LabelField("Select Theme");

            // Dropdown to select theme
            string[] themeNames = ThemeManager.themes.ConvertAll(t => t.Name).ToArray();
            int newIndex = EditorGUILayout.Popup("Theme", ThemeManager.currentThemeIndex, themeNames);

            if (newIndex != ThemeManager.currentThemeIndex)
            {
                ThemeManager.SetTheme(newIndex);
                EditorPrefs.SetInt("themeIndex", newIndex);
            }

            EditorGUILayout.Space();
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

            scroll = GUILayout.BeginScrollView(scroll);
            for (int i = 0; i < CommandLine.CommandLine.instance?.engine?.commands.Count; i++)
            {
                GUILayout.Label($"{i}: {CommandLine.CommandLine.instance.engine.commands[i].Serialize()}");
            }
            GUILayout.EndScrollView();
        }

        private void DrawTab_Prompts()
        {
            GUILayout.Label("Runtime gameobject prompts manager");
            TextPromptManager.ShowAll = GUILayout.Toggle(TextPromptManager.ShowAll, "Show all");
            EditorPrefs.SetBool("prompts_showAll", TextPromptManager.ShowAll);

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
            serializer.obj = (GameObject)EditorGUILayout.ObjectField("", serializer.obj, typeof(GameObject), true);

            serializer.allAssemblies = GUILayout.Toggle(serializer.allAssemblies, "All assemblies");
            serializer.onlyScripts = GUILayout.Toggle(serializer.onlyScripts, "Only scripts");
            serializer.showStatic = GUILayout.Toggle(serializer.showStatic, "Show static");
            serializer.showFields = GUILayout.Toggle(serializer.showFields, "Show fields");
            serializer.showProperties = GUILayout.Toggle(serializer.showProperties, "Show properties");
            serializer.collection_forceNewLine = GUILayout.Toggle(serializer.collection_forceNewLine, "Collection force new line");

            serializer.unfoldSerializable = GUILayout.Toggle(serializer.unfoldSerializable, "UnfoldSerializable");
            if (serializer.unfoldSerializable)
                serializer.serializable_forceNewLine = GUILayout.Toggle(serializer.serializable_forceNewLine, "Serializable force new line");
            
            serializer.collection_maxLimit = EditorGUILayout.IntField("Collection max limit", serializer.collection_maxLimit);

            EditorPrefs.SetBool("serializer_allAssemblies", serializer.allAssemblies);
            EditorPrefs.SetBool("serializer_onlyScripts", serializer.onlyScripts);
            EditorPrefs.SetBool("serializer_showStatic", serializer.showStatic);
            EditorPrefs.SetBool("serializer_showFields", serializer.showFields);
            EditorPrefs.SetBool("serializer_collection_forceNewLine", serializer.collection_forceNewLine);
            EditorPrefs.SetBool("serializer_unfoldSerializable", serializer.unfoldSerializable);
            EditorPrefs.SetBool("serializer_serializable_forceNewLine", serializer.serializable_forceNewLine);
            EditorPrefs.SetInt("serializer_collection_maxLimit", serializer.collection_maxLimit);

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

            scroll = GUILayout.BeginScrollView(scroll);

            if (serializer.obj != null)
            {
                try
                {
                    GUILayout.Label(serializer.Serialize(), new GUIStyle { richText = true });
                }
                catch (System.Exception e)
                {
                    GUILayout.Label("CRITICAL ERROR: " + e.Message + "\n" + e.StackTrace.Substring(0, 1000));
                }
            }

            GUILayout.EndScrollView();
        }

        private void OnGUI()
        {
            if (serializer != null) serializer.windowWidth = position.width;
            
            int newtab = GUILayout.Toolbar(tab, tabs);
            if (newtab != tab)
            {
                OnTabChanged();
            }
            tab = newtab;
            switch (newtab)
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
