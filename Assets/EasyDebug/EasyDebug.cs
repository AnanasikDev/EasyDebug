using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Reflection;
using UnityEditor;

namespace EasyDebug
{
    public class Entity
    {
        public string value;
        private object[] objects;
        public Tag tag = EasyDebug.Tag.Info;

        public Action<string> logFunction
        {
            get
            {
                switch (tag)
                {
                    case EasyDebug.Tag.Info:    return UnityEngine.Debug.Log;
                    case EasyDebug.Tag.Debug:   return UnityEngine.Debug.Log;
                    case EasyDebug.Tag.Warning: return UnityEngine.Debug.LogWarning;
                    case EasyDebug.Tag.Error:   return UnityEngine.Debug.LogError;
                }
                return null;
            }
        }

        public Entity(object[] _objects)
        {
            this.objects = _objects;
        }
        public Entity(string _value)
        {
            this.value = _value;
        }

        public Entity Parse(string separator = null)
        {
            value = QDebug.defaultParser(objects, separator != null ? separator : QDebug.defaultSeparator);
            value = QDebug.formatFunction(this);
            return this;
        }
        public Entity Parse(Func<object[], string, string> parser, string separator = null)
        {
            value = parser(objects, separator != null ? separator : QDebug.defaultSeparator);
            value = QDebug.formatFunction(this);
            return this;
        }
        public Entity Parse(Parser parser, string separator = null)
        {
            value = QDebug.FindParser(parser)(objects, separator != null ? separator : QDebug.defaultSeparator);
            value = QDebug.formatFunction(this);
            return this;
        }

        public Entity Tag(Tag tag)
        {
            this.tag = tag;
            return this;
        }

        public bool Do()
        {
            if (!QDebug.tagsAllowed.HasFlag(tag)) return false;
            
            logFunction(value);
            return true;
        }

        public Entity Color(string color)
        {
            value = $"<color={color}>{value}</color>";
            return this;
        }
    }
    public static class QDebug
    {
        public static bool serialize = true;
        public static string defaultSeparator = " ";

        public static Tag tagsAllowed = Tag.Info | Tag.Warning | Tag.Error | Tag.Debug;

        public static Func<Entity, string> formatFunction = Format;

        public static string Format(Entity entity)
        {
            return entity.value;
        }

        /// <summary>
        /// Input values ; separator ; output string
        /// </summary>
        public static Func<object[], string, string> defaultParser = HarshParse;

        public static Func<object[], string, string> FindParser(Parser parser)
        {
            switch (parser)
            {
                case Parser.Harsh: return HarshParse;
                case Parser.Deep: return DeepParse;
            }
            return null;
        }

        public static string HarshParse(object[] objects, string separator = null)
        {
            return string.Join(separator != null ? separator : QDebug.defaultSeparator, objects);
        }
        public static string DeepParse(object[] objects, string separator = null)
        {
            return string.Join(separator != null ? separator : QDebug.defaultSeparator, objects.Select(x => x is IEnumerable i && !(x is string) ? string.Join(separator, i.CastToStrings()) : x.ToString()));
        }

        public static Entity Commit(params object[] values)
        {
            return new Entity(values);
        }
        public static void Dommit(params object[] values)
        {
            new Entity(values).Parse().Do();
        }

        public static void SetFloatDivider(string divider = ".")
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = divider;
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }
        public static void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
    }
    
    public enum Parser
    {
        Harsh,
        Deep
    }

    public enum Tag
    {
        Info = 1,
        Warning = 2,
        Error = 4,
        Debug = 8
    }
}

public static class Extensions
{
    public static IEnumerable<string> CastToStrings(this IEnumerable ienumerable)
    {
        foreach (var obj in ienumerable)
            yield return obj.ToString();
    }
    public static string Color(this string target, string color)
    {
        return $"<color={color}>{target}</color>";
    }
}

/*

Syntax
 
Debug.Log("Hello").Parse().Tag(tags.Debug).Do();
Debug.LogWarning("wow").Do();
Debug.Dog();
Debug.DogWarning("oowow");

Debug.Commit("Hello").Parse().Tag(tags.Debug).Do();
Debug.Dommit("eh");

 */