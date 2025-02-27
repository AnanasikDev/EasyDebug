using EasyDebug.Serializer;
using System;
using System.Collections.Generic;
using System.Reflection;

public static class StaticClassSelector
{
    public static List<string> staticClassNames;
    public static Dictionary<string, Type> staticClassLookup;
    public static int selectedIndex = 0;

    public static void Init()
    {
        staticClassNames = new List<string>();
        staticClassLookup = new Dictionary<string, Type>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!ObjectSerializer.instance.IsAssemblyUserDefined(assembly.GetName().Name)) continue;
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass && type.IsAbstract && type.IsSealed)
                {
                    //UnityEngine.Debug.Log(type.FullName);
                    staticClassNames.Add(type.FullName);
                    staticClassLookup[type.FullName] = type;
                }
            }
        }
    }

    public static Type GetType(string typeName)
    {
        return staticClassLookup.TryGetValue(typeName, out Type type) ? type : null;
    }

    public static Type GetType(int index)
    {
        return staticClassLookup[staticClassNames[index]];
    }
}
