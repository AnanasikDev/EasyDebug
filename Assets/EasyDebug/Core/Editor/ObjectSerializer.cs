﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditorInternal;
using UnityEngine;

[Serializable]
internal class ObjectSerializer
{
    public static ObjectSerializer instance;
    public GameObject obj;

    public bool onlyScripts = true;
    public bool allAssemblies = false;
    public bool showStatic = false;
    public bool showProperties = true;
    public bool showFields = true;

    public List<AssemblyDefinitionAsset> includedAssemblyDefinitions = new List<AssemblyDefinitionAsset>();
    private HashSet<string> includedAssemblyNames = new HashSet<string>();

    public ObjectSerializer()
    {
        instance = this;
        RefreshAssemblyList();    }

    public void RefreshAssemblyList()
    {
        includedAssemblyNames.Clear();

        // Always include the default Unity C# Assembly
        includedAssemblyNames.Add("Assembly-CSharp");

        // Add user-defined assembly definitions
        foreach (var asmDef in includedAssemblyDefinitions)
        {
            if (asmDef != null)
            {
                includedAssemblyNames.Add(asmDef.name);
            }
        }
    }

    public string Serialize()
    {
        if (obj == null) return "No object selected.";

        StringBuilder sb = new StringBuilder();

        BindingFlags access = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        if (showStatic) access = access | BindingFlags.Static;

        Type type = onlyScripts ? typeof(MonoBehaviour) : typeof(Component);

        foreach (var script in obj.GetComponents(type))
        {
            if (!allAssemblies && !IsUserDefined(script.GetType())) continue;

            sb.AppendLine($"<color=#FFA500>===== {script.GetType().Name} =====</color>");
            if (showFields) foreach (var field in script.GetType().GetFields(access))
            {
                string typeName = FormatTypeName(field.FieldType);
                string fieldName = field.Name;
                string fieldValue = FormatValue(field.GetValue(script));

                string prefix = field.IsStatic ? "<color=#EEEEEF>static</color> " : "";

                sb.AppendLine(prefix + $"<color=#00FF00>{typeName}</color> <color=#87CEFA>{fieldName}</color><color=#EEEEEE>:</color> <color=#FFD700>{fieldValue}</color>");
            }

            if (showProperties) foreach (var prop in script.GetType().GetProperties(access))
            {
                if (prop.GetCustomAttribute<ObsoleteAttribute>() != null) continue;

                string typeName = FormatTypeName(prop.PropertyType);
                string fieldName = prop.Name;
                string fieldValue = FormatValue(prop.GetValue(script));

                string prefix = "<color=#EEEEEF>" + (prop.CanRead ? "r" : "" + (prop.CanWrite ? "w" : "")) + "</color> ";

                sb.AppendLine(prefix + $"<color=#CAFC01>{typeName}</color> <color=#87CEFA>{fieldName}</color><color=#EEEEEE>:</color> <color=#FFD700>{fieldValue}</color>");
            }
        }

        return sb.ToString();
    }

    private bool IsUserDefined(Type type)
    {
        return includedAssemblyNames.Contains(type.Assembly.GetName().Name);
    }

    private string FormatTypeName(Type type)
    {
        return type switch
        {
            _ when type == typeof(int) => "int",
            _ when type == typeof(float) => "float",
            _ when type == typeof(bool) => "bool",
            _ when type == typeof(string) => "string",
            _ when type == typeof(Vector3) => "Vector3",
            _ when type == typeof(Vector2) => "Vector2",
            _ when type == typeof(GameObject) => "GameObject",
            _ => type.Name
        };
    }

    private string FormatValue(object value)
    {
        return value switch
        {
            null => "null",
            bool b => b ? "true" : "false",
            float f => f.ToString("F3"),
            Vector3 v => $"({v.x:F2}, {v.y:F2}, {v.z:F2})",
            Vector2 v => $"({v.x:F2}, {v.y:F2})",
            _ => value.ToString()
        };
    }
}
