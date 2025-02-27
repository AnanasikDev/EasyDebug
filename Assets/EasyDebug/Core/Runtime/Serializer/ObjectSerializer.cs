using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace EasyDebug.Serializer
{
    public class ObjectSerializer
    {
        public static ObjectSerializer instance;
        public GameObject obj;
        public Type staticType;

        public bool onlyScripts = true;
        public bool allAssemblies = false;

        public bool showStatic = false;
        public bool showProperties = true;
        public bool showFields = true;
        /// <summary>
        /// If false only members declared by the user are shown.
        /// </summary>
        public bool declaredOnly = true;

        public bool unfoldSerializable = true;
        public bool serializable_forceNewLine = true;
        public int maxInnerDepth = 3;

        public bool unfoldCollections = true;
        public bool collection_forceNewLine = true;
        public int collection_maxLimit = 30;

        public float windowWidth;
        private bool dynamicWidth = true;

        public List<Type> deepSerializationBlacklist = new List<Type>()
        {
            typeof(TMP_FontAsset),
            typeof(AnimationTriggers)
        };

        private static readonly Dictionary<Type, string> typeAliases = new()
        {
            { typeof(int), "int" },
            { typeof(float), "float" },
            { typeof(bool), "bool" },
            { typeof(string), "string" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(uint), "uint" },
            { typeof(Vector3), "Vector3" },
            { typeof(Vector2), "Vector2" },
            { typeof(Vector4), "Vector4" },
            { typeof(Quaternion), "Quaternion" },
            { typeof(Color), "Color" },
            { typeof(GameObject), "GameObject" },
            { typeof(Transform), "Transform" },
            { typeof(Rect), "Rect" }
        };

        public List<AssemblyDefinitionAsset> includedAssemblyDefinitions = new List<AssemblyDefinitionAsset>();
        public HashSet<string> includedAssemblyNames = new HashSet<string>();

        public ObjectSerializer()
        {
            instance = this;
            RefreshAssemblyList();
        }

        public BindingFlags GetBindingFlags()
        {
            BindingFlags access = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            if (showStatic) access |= BindingFlags.Static;
            if (declaredOnly) access |= BindingFlags.DeclaredOnly;

            return access;
        }

        public BindingFlags GetStaticBindingFlags()
        {
            BindingFlags access = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            if (declaredOnly) access |= BindingFlags.DeclaredOnly;

            return access;
        }

        public void RefreshAssemblyList()
        {
            includedAssemblyNames.Clear();

            // Always include the default Unity C# Assembly
            includedAssemblyNames.Add("Assembly-CSharp");
            includedAssemblyNames.Add("Assembly-CSharp-Editor");

            // Add user-defined assembly definitions
            foreach (var asmDef in includedAssemblyDefinitions)
            {
                if (asmDef != null)
                {
                    includedAssemblyNames.Add(asmDef.name);
                }
            }
        }

        public bool IsTypeUserDefined(Type type)
        {
            return IsAssemblyUserDefined(type.Assembly.GetName().Name);
        }
        public bool IsAssemblyUserDefined(string name)
        {
            return includedAssemblyNames.Contains(name);
        }


        public string SerializeGameobject()
        {
            if (obj == null) return "No object selected.";

            StringBuilder sb = new StringBuilder();
            BindingFlags access = GetBindingFlags();
            Type type = onlyScripts ? typeof(MonoBehaviour) : typeof(Component);

            foreach (var script in obj.GetComponents(type))
            {
                if (!allAssemblies && !IsTypeUserDefined(script.GetType())) continue;

                sb.AppendLine(SerializerHelper.FormatHeader(this, script.GetType().Name, dynamicWidth));
                sb.AppendLine(SerializerHelper.SerializeComponent(script, access, this));
            }

            return sb.ToString();
        }

        public string SerializeStaticType()
        {
            if (staticType == null) return "No type selected.";
            if (!allAssemblies && !IsTypeUserDefined(staticType)) return "-";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(SerializerHelper.FormatHeader(this, staticType.Name, dynamicWidth));
            sb.AppendLine(SerializerHelper.SerializeComponent(staticType, GetStaticBindingFlags(), this));
            return sb.ToString();
        }

        public string SerializeInnerObject(object obj, int depthi = 0)
        {
            if (obj == null) return "null";

            depthi++;
            if (depthi > maxInnerDepth) return obj.GetType().ToString() + " MAXLIMIT";

            if (!unfoldSerializable) return obj.ToString();

            Type type = obj.GetType();
            if (deepSerializationBlacklist.Contains(type)) return obj.ToString();

            List<string> vals = new();

            string separator = serializable_forceNewLine ? "\n" : ", ";
            string tab = "  ".Repeat(depthi);
            string tab1 = "  ".Repeat(depthi - 1);

            BindingFlags access = GetBindingFlags();

            if (serializable_forceNewLine)
            {
                return type.Name + "\n" + tab1 + "{\n" + SerializerHelper.SerializeComponent(obj, access, this, depthi, separator, (string unit) => tab + unit) + "\n" + tab1 + "}";
            }
            else
            {
                return type.Name + "{" + SerializerHelper.SerializeComponent(obj, access, this, depthi, separator) + "}";
            }
        }

        public string FormatTypeName(Type type)
        {
            if (typeAliases.TryGetValue(type, out string alias))
                return alias;

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

            return type.Name;
        }

        public string FormatValue(object value, int depthi = 0)
        {
            if (value == null) return "null";
            if (value is Type) return $"Type({value})";
            if (value is string str) return str;

            Type type = value.GetType();

            if (TryFormatCollection(out string col, value, depthi))
                return col;

            // Handle custom serializable classes
            if (type.IsClass && !(value.GetType().IsAbstract && value.GetType().IsSealed) && type.GetCustomAttribute(typeof(SerializableAttribute)) != null)
            {
                return SerializeInnerObject(value, depthi);
            }

            return value.ToString();
        }

        // Helper function to format collections (arrays/lists)
        private bool TryFormatCollection(out string result, object value, int depthi = 0)
        {
            result = "";
            Type type = value.GetType();
            StringBuilder sb = new StringBuilder();

            // Handle arrays
            if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                var collection = (IEnumerable)value;
                int length = GetLength(collection);
                sb.Append($"({length} elements)[]");
                if (!unfoldCollections)
                {
                    result = sb.ToString();
                    return true;
                }

                sb.Append("[" + (collection_forceNewLine ? "\n" : ""));
                int i = 0;
                foreach (var item in collection)
                {
                    i++;
                    if (i > collection_maxLimit)
                    {
                        sb.Append($"... {collection_maxLimit} shown, {length - collection_maxLimit} left ...");
                        break;
                    }
                    if (i == length)
                    {
                        sb.Append(FormatValue(item, depthi));
                    }
                    else
                    {
                        sb.Append(FormatValue(item, depthi) + ", " + (collection_forceNewLine ? "\n" : ""));
                    }
                }
                sb.AppendLine((collection_forceNewLine ? "\n" : "") + "]");
                result = sb.ToString();
                return true;
            }

            // Handle generic dictionaries
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var dict = (IDictionary)value;
                int length = GetLength(dict);
                sb.Append($"({length} pairs)" + "{}");
                if (!unfoldCollections)
                {
                    result = sb.ToString();
                    return true;
                }

                sb.Append("{" + (collection_forceNewLine ? "\n" : ""));
                int i = 0;
                foreach (DictionaryEntry entry in dict)
                {
                    i++;
                    if (i > collection_maxLimit)
                    {
                        sb.Append($"... {collection_maxLimit} shown, {length - collection_maxLimit} left ...");
                        break;
                    }
                    if (i == length)
                    {
                        sb.Append($"{FormatValue(entry.Key, depthi)}" + ":" + $"{FormatValue(entry.Value, depthi)}");
                    }
                    else
                    {
                        sb.Append($"{FormatValue(entry.Key, depthi)}: {FormatValue(entry.Value, depthi)}, " + (collection_forceNewLine ? "\n" : ""));
                    }
                }
                sb.AppendLine((collection_forceNewLine ? "\n" : "") + "}");
                result = sb.ToString();
                return true;
            }
            return false;
        }

        private int GetLength(IEnumerable collection)
        {
            return collection.Cast<object>().ToArray().Length;
        }
    }
}