using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ArgumentParser
{
    public static char FloatSeparator = '.';

    public static object ParseArgument(Type type, string str)
    {
        if (type == typeof(int)) return int.Parse(str);
        if (type == typeof(float)) return float.Parse(str.Replace(FloatSeparator, '.'), CultureInfo.InvariantCulture);
        if (type == typeof(double)) return double.Parse(str.Replace(FloatSeparator, '.'), CultureInfo.InvariantCulture);
        if (type == typeof(uint)) return uint.Parse(str);
        if (type == typeof(long)) return long.Parse(str);
        if (type == typeof(short)) return short.Parse(str);
        if (type == typeof(byte)) return byte.Parse(str);
        if (type == typeof(sbyte)) return sbyte.Parse(str);
        if (type == typeof(bool)) return bool.Parse(str);
        if (type == typeof(string)) return str;
        if (type == typeof(char) && str.Length == 1) return str[0];

        if (type == typeof(Vector2)) return ParseVector2(str);
        if (type == typeof(Vector3)) return ParseVector3(str);
        if (type == typeof(Vector4)) return ParseVector4(str);
        if (type == typeof(Quaternion)) return ParseQuaternion(str);
        if (type == typeof(Vector2Int)) return ParseVector2Int(str);
        if (type == typeof(Vector3Int)) return ParseVector3Int(str);

        throw new ArgumentException($"Unsupported type: {type}");
    }

    public static T ParseArgument<T>(string str) => (T)ParseArgument(typeof(T), str);

    private static Vector2 ParseVector2(string str)
    {
        var v = ExtractFloats(str, 2);
        return new Vector2(v[0], v[1]);
    }

    private static Vector3 ParseVector3(string str)
    {
        var v = ExtractFloats(str, 3);
        return new Vector3(v[0], v[1], v[2]);
    }

    private static Vector4 ParseVector4(string str)
    {
        var v = ExtractFloats(str, 4);
        return new Vector4(v[0], v[1], v[2], v[3]);
    }

    private static Quaternion ParseQuaternion(string str)
    {
        var v = ExtractFloats(str, 4);
        return new Quaternion(v[0], v[1], v[2], v[3]);
    }

    private static Vector2Int ParseVector2Int(string str)
    {
        var v = ExtractInts(str, 2);
        return new Vector2Int(v[0], v[1]);
    }

    private static Vector3Int ParseVector3Int(string str)
    {
        var v = ExtractInts(str, 3);
        return new Vector3Int(v[0], v[1], v[2]);
    }

    private static float[] ExtractFloats(string str, int count)
    {
        var matches = Regex.Matches(str, @"-?\d+[" + FloatSeparator + @"]?\d*");
        if (matches.Count != count) throw new FormatException($"Invalid format for {count}-component vector.");
        float[] values = new float[count];
        for (int i = 0; i < count; i++)
            values[i] = float.Parse(matches[i].Value.Replace(FloatSeparator, '.'), CultureInfo.InvariantCulture);
        return values;
    }

    private static int[] ExtractInts(string str, int count)
    {
        var matches = Regex.Matches(str, @"-?\d+");
        if (matches.Count != count) throw new FormatException($"Invalid format for {count}-component integer vector.");
        int[] values = new int[count];
        for (int i = 0; i < count; i++)
            values[i] = int.Parse(matches[i].Value, CultureInfo.InvariantCulture);
        return values;
    }
}
