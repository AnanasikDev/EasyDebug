using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace EasyDebug
{
    public class Entity
    {
        public string value;
        private object[] objects;

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
            value = EDebug.ParseFunction(objects, separator != null ? separator : EDebug.defaultSeparator);
            return this;
        }
        public Entity Parse(Func<object[], string, string> parser, string separator = null)
        {
            value = parser(objects, separator != null ? separator : EDebug.defaultSeparator);
            return this;
        }
        public Entity Parse(Parser parser, string separator = null)
        {
            value = EDebug.FindParser(parser)(objects, separator != null ? separator : EDebug.defaultSeparator);
            return this;
        }

        public void Do()
        {
            UnityEngine.Debug.Log(value);
        }
    }
    public static class EDebug
    {
        public static bool serialize = true;
        public static string defaultSeparator = " ";

        /// <summary>
        /// Input values ; separator ; output string
        /// </summary>
        public static Func<object[], string, string> ParseFunction = HarshParse;

        public static Func<object[], string, string> FindParser(Parser parser)
        {
            switch (parser)
            {
                case Parser.Harsh: return HarshParse;
                case Parser.Deep:  return DeepParse;
            }
            return null;
        }

        public static string HarshParse(object[] objects, string separator = null)
        {
            return string.Join(separator != null ? separator : EDebug.defaultSeparator, objects);
        }
        public static string DeepParse(object[] objects, string separator = null)
        {
            return string.Join(separator != null ? separator : EDebug.defaultSeparator, objects.Select(x => x is IEnumerable i && !(x is string) ? string.Join(separator, i.CastToStrings()) : x.ToString()));
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
    }
    
    public enum Parser
    {
        Harsh,
        Deep
    }
}

public static class Extensions
{
    public static IEnumerable<string> CastToStrings(this IEnumerable ienumerable)
    {
        foreach (var obj in ienumerable)
            yield return obj.ToString();
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