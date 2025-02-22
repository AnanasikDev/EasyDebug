using EasyDebug;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;
using Codice.Client.Common;

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
    public bool serializable_forceNewLine = true;
    public bool collection_forceNewLine = true;
    public int collection_maxLimit = 30;
    public bool unfoldSerializable = true;

    private int maxDepth = 3;

    public float windowWidth;
    private bool dynamicWidth = true;

    public List<AssemblyDefinitionAsset> includedAssemblyDefinitions = new List<AssemblyDefinitionAsset>();
    private HashSet<string> includedAssemblyNames = new HashSet<string>();

    public ObjectSerializer()
    {
        instance = this;
        RefreshAssemblyList();    
    }

    public BindingFlags GetBindingFlags()
    {
        BindingFlags access = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        if (showStatic) access = access | BindingFlags.Static;

        return access;
    }

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

    public string SerializeUnit(string typeName, string name, string value, bool isStatic, bool isField, bool read = true, bool write = true)
    {
        var theme = ThemeManager.currentTheme;

        string staticPrefix = (isStatic ? "static " : "").Colorify(theme.prefixColor);

        string rwPrefix = isField ? "" : ("r".Colorify(read ? theme.prefixColor : Color.clear) + "w".Colorify(write ? theme.prefixColor : Color.clear) + " ");

        return staticPrefix + rwPrefix + typeName.Colorify(isField ? theme.fieldTypeColor : theme.propertyTypeColor) + " " + name.Colorify(theme.nameColor) + ":".Colorify(theme.prefixColor) + " " + value.Colorify(theme.valueColor);
    }

    public string Serialize()
    {
        if (obj == null) return "No object selected.";

        StringBuilder sb = new StringBuilder();

        BindingFlags access = GetBindingFlags();

        Type type = onlyScripts ? typeof(MonoBehaviour) : typeof(Component);

        foreach (var script in obj.GetComponents(type))
        {
            if (!allAssemblies && !IsUserDefined(script.GetType())) continue;
            int width = dynamicWidth ? (int)(windowWidth / 18) : 8;

            sb.AppendLine(script.GetType().Name.Encapsulate("=".Repeat(width) + " ", true).Colorify(ThemeManager.currentTheme.scriptColor) + "\n");
            if (showFields) foreach (var field in script.GetType().GetFields(access))
            {
                string typeName = FormatTypeName(field.FieldType);
                string fieldName = field.Name;
                string fieldValue = FormatValue(field.GetValue(script));

                sb.AppendLine(SerializeUnit(typeName, fieldName, fieldValue, field.IsStatic, true));
            }

            if (showProperties) foreach (var prop in script.GetType().GetProperties(access))
            {
                if (prop.GetCustomAttribute<ObsoleteAttribute>() != null) continue;

                string typeName = FormatTypeName(prop.PropertyType);
                string name = prop.Name;
                string value = FormatValue(prop.GetValue(script));
                bool isStatic = prop.CanRead ? (prop.GetGetMethod()?.IsStatic ?? false) : (prop.CanWrite ? prop.GetSetMethod()?.IsStatic ?? false : false);

                sb.AppendLine(SerializeUnit(typeName, name, value, isStatic, false, prop.CanRead, prop.CanWrite));
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
        if (type.IsArray)
            return FormatTypeName(type.GetElementType()) + "[]";

        if (type.IsGenericType)
        {
            Type genericType = type.GetGenericTypeDefinition();
            Type[] genericArgs = type.GetGenericArguments();

            if (genericType == typeof(List<>))
                return $"List<{FormatTypeName(genericArgs[0])}>";
            if (genericType == typeof(Dictionary<,>))
                return $"Dictionary<{FormatTypeName(genericArgs[0])}, {FormatTypeName(genericArgs[1])}>";
        }

        return type switch
        {
            _ when type == typeof(int) => "int",
            _ when type == typeof(float) => "float",
            _ when type == typeof(bool) => "bool",
            _ when type == typeof(string) => "string",
            _ when type == typeof(byte) => "byte",
            _ when type == typeof(sbyte) => "sbyte",
            _ when type == typeof(short) => "short",
            _ when type == typeof(ushort) => "ushort",
            _ when type == typeof(long) => "long",
            _ when type == typeof(ulong) => "ulong",
            _ when type == typeof(double) => "double",
            _ when type == typeof(decimal) => "decimal",
            _ when type == typeof(uint) => "uint",
            _ when type == typeof(Vector3) => "Vector3",
            _ when type == typeof(Vector2) => "Vector2",
            _ when type == typeof(Vector4) => "Vector4",
            _ when type == typeof(Quaternion) => "Quaternion",
            _ when type == typeof(Color) => "Color",
            _ when type == typeof(GameObject) => "GameObject",
            _ when type == typeof(Transform) => "Transform",
            _ when type == typeof(Rect) => "Rect",
            _ => type.Name
        };
    }

    private string FormatValue(object value, int depthi = 0)
    {
        depthi++;
        if (depthi > maxDepth) return "MAXLIMIT";

        if (value == null)
            return "null";

        Type type = value.GetType();

        if (type == typeof(string))
        {
            return value.ToString();
        }

        // Handle arrays
        if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
        {
            var collection = (IEnumerable)value;
            return $"({GetLength(collection)} elements)[" + (collection_forceNewLine ? "\n" : "") + string.Join((collection_forceNewLine ? ",\n" : ", "), FormatEnumerable(collection)) + (collection_forceNewLine ? "\n" : "") + "]";
        }

        // Handle generic dictionaries
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var dict = (IDictionary)value;
            List<string> entries = new();
            int i = 0;
            foreach (DictionaryEntry entry in dict)
            {
                if (i >= collection_maxLimit)
                {
                    entries.Add($"... ({collection_maxLimit} shown, {dict.Count - collection_maxLimit} hidden)");
                    break;
                }
                i++;
                entries.Add($"{FormatValue(entry.Key)}: {FormatValue(entry.Value)}");
            }
            return $"({GetLength(dict)} elements){{" + (collection_forceNewLine ? "\n" : "") + string.Join((collection_forceNewLine ? ",\n" : ", "), entries) + (collection_forceNewLine ? "\n" : "") + "}";
        }

        // Handle custom serializable classes
        if (type.IsClass && type.GetCustomAttribute(typeof(SerializableAttribute)) != null)
        {
            return SerializeObject(value);
        }

        // Handle standard types
        return value switch
        {
            bool b => b ? "true" : "false",
            float f => f.ToString("F3"),
            double d => d.ToString("F3"),
            Vector3 v => $"({v.x:F2}, {v.y:F2}, {v.z:F2})",
            Vector2 v => $"({v.x:F2}, {v.y:F2})",
            Vector4 v => $"({v.x:F2}, {v.y:F2}, {v.z:F2}, {v.w:F2})",
            Quaternion q => $"({q.x:F2}, {q.y:F2}, {q.z:F2}, {q.w:F2})",
            Color c => PipeConsole.Commit("■").Parse().Colorify(c).value + $"RGBA({c.r:F2}, {c.g:F2}, {c.b:F2}, {c.a:F2})",
            _ => value.ToString()
        };
    }

    // Helper function to format collections (arrays/lists)
    private IEnumerable<string> FormatEnumerable(IEnumerable collection)
    {
        int i = 0;
        foreach (var item in collection)
        {
            i++;
            if (i > collection_maxLimit)
            {
                yield return $"... ({collection_maxLimit} shown, {GetLength(collection) - collection_maxLimit} hidden)";
                yield break;
            }
            yield return FormatValue(item);
        }
    }

    // Serialize custom serializable objects
    private string SerializeObject(object obj)
    {
        if (!unfoldSerializable) return obj.ToString();

        Type type = obj.GetType();
        List<string> vals = new();

        string separator = serializable_forceNewLine ? ",\n" : ", ";

        BindingFlags access = GetBindingFlags();

        foreach (FieldInfo field in type.GetFields(access))
        {
            object fieldValue = field.GetValue(obj);
            vals.Add(SerializeUnit(FormatTypeName(field.FieldType), field.Name, FormatValue(fieldValue), false, true));
        }

        foreach (PropertyInfo prop in type.GetProperties(access))
        {
            object propValue = prop.GetValue(obj);
            vals.Add(SerializeUnit(FormatTypeName(prop.PropertyType), prop.Name, FormatValue(propValue), true, true, prop.CanRead, prop.CanWrite));
        }

        return type.Name + (serializable_forceNewLine ? "\n{\n" : "{") + string.Join(separator, vals) + (serializable_forceNewLine ? "\n}" : "}");
    }

    private int GetLength(IEnumerable collection)
    {
        return collection.Cast<object>().ToArray().Length;
    }
}
