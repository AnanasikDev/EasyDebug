using UnityEngine;
using System.Linq;
public static class EasyDebug
{
    public static void Log(params object[] objects)
    {
        Debug.Log(string.Join(" ", objects));
    }
}
