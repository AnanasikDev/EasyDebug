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
        if (staticClassNames != null) return;

        staticClassNames = new List<string>();
        staticClassLookup = new Dictionary<string, Type>();

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsClass && type.IsAbstract && type.IsSealed)
            {
                staticClassNames.Add(type.FullName);
                staticClassLookup[type.FullName] = type;
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
