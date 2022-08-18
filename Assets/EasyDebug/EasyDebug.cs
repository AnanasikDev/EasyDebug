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
        public Dag tag = Dag.Info;

        public Action<string, UnityEngine.Object> logFunction
        {
            get
            {
                switch (tag)
                {
                    case Dag.Info:    return UnityEngine.Debug.Log;
                    case Dag.Debug:   return UnityEngine.Debug.Log;
                    case Dag.Warning: return UnityEngine.Debug.LogWarning;
                    case Dag.Error:   return UnityEngine.Debug.LogError;
                }
                return null;
            }
        }

        public Entity(object[] _objects)
        {
            this.objects = _objects;
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

        public Entity Tag(Dag tag)
        {
            this.tag = tag;
            return this;
        }

        public bool Do(UnityEngine.Object target = null)
        {
            if (!QDebug.tagsAllowed.HasFlag(tag)) return false;

            logFunction(value, target);
            return true;
        }
    }
}

public static class EasyDebugExtensions
{
    public static IEnumerable<string> CastToStrings(this IEnumerable ienumerable)
    {
        foreach (var obj in ienumerable)
            yield return obj.ToString();
    }
    /// <summary>
    /// Wraps string with <color> tag
    /// </summary>
    public static string Color(this string target, string color)
    {
        return $"<color={color}>{target}</color>";
    }

    /// <summary>
    /// Wraps Entity's value with <color> tag
    /// </summary>
    public static EasyDebug.Entity Color(this EasyDebug.Entity entity, string color)
    {
        entity.value = $"<color={color}>{entity.value}</color>";
        return entity;
    }
}

public static class QDebug
{
    public static bool serialize = true;
    public static string defaultSeparator = " ";

    public static Dag tagsAllowed = Dag.Info | Dag.Warning | Dag.Error | Dag.Debug;

    /// <summary>
    /// Function delegate that controls the way of global
    /// output formatting. Can be overriten by user.
    /// </summary>
    public static Func<EasyDebug.Entity, string> formatFunction = Format;

    /// <summary>
    /// Controls global output formatting.
    /// Returns Entity.value by default.
    /// Fullfills the default value of 'formatFunction', that actually controls the formatting.
    /// </summary>
    /// <param name="entity">The Entity to format</param>
    /// <returns>Single formatted string</returns>
    public static string Format(EasyDebug.Entity entity)
    {
        return entity.value;
    }

    /// <summary>
    /// Input values ; separator ; output string
    /// </summary>
    public static Func<object[], string, string> defaultParser = HarshParse;

    /// <summary>
    /// Provides parser function by the type
    /// </summary>
    /// <param name="parser">Parser algorithm type</param>
    /// <returns>Parser algorithm delegate</returns>
    public static Func<object[], string, string> FindParser(Parser parser)
    {
        switch (parser)
        {
            case Parser.Harsh: return HarshParse;
            case Parser.Deep: return DeepParse;
        }
        return null;
    }
    public static void SetParser(Parser parser)
    {
        defaultParser = FindParser(parser);
    }

    /// <summary>
    /// Parses input objects into a single string using The Harsh Algorithm (THA).
    /// Converts each object into a string via .ToString(); separates with separator
    /// if possible or defaultSeparator if l:separator is null.
    /// </summary>
    /// <param name="objects">objects to combine</param>
    /// <param name="separator">separates parsed objects</param>
    /// <returns>Single string</returns>
    public static string HarshParse(object[] objects, string separator = null)
    {
        return string.Join(separator != null ? separator : QDebug.defaultSeparator, objects);
    }

    /// <summary>
    /// Parses input objects into a single string using The Deep Algorithm (TDA).
    /// Converts objects to strings via .ToString() but keeping IEnumerables serialized properly.
    /// </summary>
    /// <param name="objects">objects to combine</param>
    /// <param name="separator">separates parsed objects. Does NOT separate IEnumerables' content.</param>
    /// <returns>Single string</returns>
    public static string DeepParse(object[] objects, string separator = null)
    {
        return string.Join(separator != null ? separator : QDebug.defaultSeparator, objects.Select(x => x is IEnumerable i && !(x is string) ? string.Join(separator, i.CastToStrings()) : x.ToString()));
    }

    /// <summary>
    /// Commits a new Entity that handles printing.
    /// </summary>
    /// <param name="values">Objects to commit</param>
    /// <returns>Entity to pipe</returns>
    public static EasyDebug.Entity Commit(params object[] values)
    {
        return new EasyDebug.Entity(values);
    }
    /// <summary>
    /// Represents function chain 'Commit(values).Parse().Do()'
    /// The short form of simple chain. Doesn't pipe.
    /// </summary>
    /// <param name="values">Objects to commit</param>
    public static void Dommit(params object[] values)
    {
        Commit(values).Parse().Do();
    }

    /// <summary>
    /// Sets default float numbers' string representation to
    /// dividing with dot (.) instead of comma (,). Absorbs any string.
    /// </summary>
    /// <param name="divider">The float divider string</param>
    public static void SetFloatDivider(string divider = ".")
    {
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = divider;
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
    }

    /// <summary>
    /// Clears Unity console.
    /// </summary>
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

public enum Dag
{
    Info = 1,
    Warning = 2,
    Error = 4,
    Debug = 8
}