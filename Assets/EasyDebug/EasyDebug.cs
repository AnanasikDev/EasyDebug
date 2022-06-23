using UnityEngine;
using System.Collections.Generic;
public static class EasyDebug
{
    public static void Log(params object[] objects)
    {
        Debug.Log(string.Join(" ", objects));
    }
    public static void LogSep(string separator, params object[] objects)
    {
        Debug.Log(string.Join(separator, objects));
    }
    public static void LogCollection<T>(IEnumerable<T> collection)
    {
        Debug.Log(string.Join(" ", collection));
    }

    public static void LogWarning(params object[] objects)
    {
        Debug.LogWarning(string.Join(" ", objects));
    }
    public static void LogWarningSep(string separator, params object[] objects)
    {
        Debug.LogWarning(string.Join(separator, objects));
    }
    public static void LogWarningCollection<T>(IEnumerable<T> collection)
    {
        Debug.LogWarning(string.Join(" ", collection));
    }

    public static void LogError(params object[] objects)
    {
        Debug.LogError(string.Join(" ", objects));
    }
    public static void LogErrorSep(string separator, params object[] objects)
    {
        Debug.LogError(string.Join(separator, objects));
    }
    public static void LogErrorCollection<T>(IEnumerable<T> collection)
    {
        Debug.LogError(string.Join(" ", collection));
    }
}
