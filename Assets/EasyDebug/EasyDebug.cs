using UnityEngine;
using System.Linq;
public static class EasyDebug
{
    public static void Log(params object[] objects)
    {
        Debug.Log(string.Join(" ", objects));
    }
    public static void Log(string separator, params object[] objects)
    {
        Debug.Log(string.Join(separator, objects));
    }

    public static void LogWarning(params object[] objects)
    {
        Debug.LogWarning(string.Join(" ", objects));
    }
    public static void LogWarning(string separator, params object[] objects)
    {
        Debug.LogWarning(string.Join(separator, objects));
    }

    public static void LogError(params object[] objects)
    {
        Debug.LogError(string.Join(" ", objects));
    }
    public static void LogError(string separator, params object[] objects)
    {
        Debug.LogError(string.Join(separator, objects));
    }
}
