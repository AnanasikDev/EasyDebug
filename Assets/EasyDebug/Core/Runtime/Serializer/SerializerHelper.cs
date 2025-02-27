using System;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace EasyDebug.Serializer
{
    internal static class SerializerHelper
    {
        public static string SerializeComponent(object component, BindingFlags access, ObjectSerializer serializer, int depthi = 0, string separator = "\n", Func<string, string> formatUnit = null)
        {
            StringBuilder sb = new StringBuilder();
            Type type = component.GetType();
            if (component is Type t) type = t; // for static classes component is a static class, i.e. type

            if (serializer.showFields)
            {
                SerializeFields(sb, type, component, access, serializer, depthi, separator, formatUnit);
            }

            if (type.GetProperties().Length > 0)
            {
                sb.Append(separator);
                if (serializer.showProperties)
                {
                    SerializeProperties(sb, type, component, access, serializer, depthi, separator, formatUnit);
                }
            }

            return sb.ToString();
        }

        private static void SerializeFields(StringBuilder sb, Type type, object obj, BindingFlags access, ObjectSerializer serializer, int depthi = 0, string separator = "\n", Func<string, string> formatUnit = null)
        {
            FieldInfo[] fields = type.GetFields(access);
            int length = fields.Length;
            int i = 0;
            foreach (var field in fields)
            {
                i++;
                string typeName = serializer.FormatTypeName(field.FieldType);
                string fieldName = field.Name;
                string fieldValue = serializer.FormatValue(field.GetValue(obj), depthi);
                sb.Append(SerializeUnit(typeName, fieldName, fieldValue, field.IsStatic, true, true, true, formatUnit) + (i == length ? "" : separator));
            }
        }

        private static void SerializeProperties(StringBuilder sb, Type type, object obj, BindingFlags access, ObjectSerializer serializer, int depthi = 0, string separator = "\n", Func<string, string> formatUnit = null)
        {
            PropertyInfo[] props = type.GetProperties(access);
            int length = props.Length;
            int i = 0;
            foreach (var prop in props)
            {
                i++;
                if (prop.GetCustomAttribute<ObsoleteAttribute>() != null) continue;

                string typeName = serializer.FormatTypeName(prop.PropertyType);
                string name = prop.Name;
                bool isStatic = prop.CanRead ? prop.GetGetMethod()?.IsStatic ?? false : prop.CanWrite ? prop.GetSetMethod()?.IsStatic ?? false : false;
                string value = serializer.FormatValue(prop.GetValue(obj), depthi);

                sb.Append(SerializeUnit(typeName, name, value, isStatic, false, prop.CanRead, prop.CanWrite, formatUnit) + (i == length ? "" : separator));
            }
        }

        public static string FormatHeader(ObjectSerializer serializer, string name, bool dynamicWidth)
        {
            int width = dynamicWidth ? (int)(serializer.windowWidth / 16.0f) - name.Length / 4 : 8;
            return "\n" + name.Encapsulate("=".Repeat(width) + " ", true).Colorify(ThemeManager.currentTheme.scriptColor) + "\n";
        }

        public static string SerializeUnit(string typeName, string name, string value, bool isStatic, bool isField, bool read = true, bool write = true, Func<string, string> formatUnit = null)
        {
            var theme = ThemeManager.currentTheme;

            string staticPrefix = (isStatic ? "static " : "").Colorify(theme.prefixColor);

            string rwPrefix = isField ? "" : ("r".Colorify(read ? theme.prefixColor : Color.clear) + "w".Colorify(write ? theme.prefixColor : Color.clear) + " ");

            string result = staticPrefix + rwPrefix + typeName.Colorify(isField ? theme.fieldTypeColor : theme.propertyTypeColor) + " " + name.Colorify(theme.nameColor) + ":".Colorify(theme.prefixColor) + " " + value.Colorify(theme.valueColor);

            if (formatUnit == null) return result;
            return formatUnit(result);
        }

    }
}
