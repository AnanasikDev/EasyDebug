using System.Linq;
using System.Text;
using UnityEngine;

namespace EasyDebug
{
    public static class StringExtensions
    {
        public static string Colorify(this string str, UnityEngine.Color color)
        {
            if (str.Trim() == string.Empty) return str;

            string hex = ColorUtility.ToHtmlStringRGB(color);
            return $"<color=#{hex}>{str}</color>";
        }

        public static string Encapsulate(this string str, string cap, bool mirror = false)
        {
            return cap + str + (mirror ? string.Join("", cap.Reverse()) : cap);
        }

        public static string Encapsulate(this string str, string leftCap, string rightCap, string filler = "")
        {
            return leftCap + filler + str + filler + rightCap;
        }

        public static string Repeat(this string str, int n)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                sb.Append(str);
            }
            return sb.ToString();
        }
    }
}