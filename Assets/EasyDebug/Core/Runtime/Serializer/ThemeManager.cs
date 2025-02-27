using System.Collections.Generic;
using UnityEngine;

namespace EasyDebug
{
    public static class ThemeManager
    {
        public static Theme currentTheme { get { return themes[currentThemeIndex]; } }
        public static int currentThemeIndex { get; private set; } = 0;

        public static List<Theme> themes;

        public static void SafeInit()
        {
            if (themes == null || themes.Count == 0)
                themes = GetThemes();
        }

        public static bool SetTheme(string name)
        {
            var temp = GetTheme(name);
            if (temp == null) return false;
            currentThemeIndex = GetThemeIndex(temp);
            return true;
        }

        public static bool SetTheme(int index)
        {
            var temp = GetTheme(index);
            if (temp == null) return false;
            currentThemeIndex = index;
            return true;
        }

        public static Theme GetTheme(string name)
        {
            foreach (var theme in themes) if (theme.Name == name) return theme;
            return themes == null || themes.Count == 0 ? null : themes[0];
        }

        public static Theme GetTheme(int index)
        {
            return themes == null || themes.Count == 0 ? null : (index < 0 || index > themes.Count - 1 ? themes[0] : themes[index]);
        }

        public static int GetThemeIndex(Theme theme)
        {
            for (int i = 0; i < themes.Count; i++)
            {
                if (themes[i] == theme) return i;
            }
            return 0;
        }

        public static int GetThemeIndex(string name)
        {
            for (int i = 0; i < themes.Count; i++)
            {
                if (themes[i].Name == name) return i;
            }
            return 0;
        }

        public static List<Theme> GetThemes()
        {
            return new List<Theme>
            {
new Theme("Default",
    fieldTypeColor: new Color(0.3f, 0.9f, 0.6f),
    propertyTypeColor: new Color(0.64f, 0.6f, 0.99f),
    valueColor: new Color(0.9f, 0.7f, 0.3f),
    nameColor: new Color(0.9f, 0.9f, 0.91f),
    prefixColor: new Color(0.95f, 0.95f, 1.0f),
    scriptColor: new Color(1.0f, 0.4f, 0.1f),
    dictKeyColor: new Color(1.0f, 0.9f, 0.8f)),

new Theme("Neon Eclipse",
    fieldTypeColor: new Color(0.0f, 0.9f, 1.0f),
    propertyTypeColor: new Color(1.0f, 0.3f, 0.9f),
    valueColor: new Color(1.0f, 1.0f, 0.0f),
    nameColor: new Color(0.6f, 1.0f, 0.0f),
    prefixColor: new Color(0.81f, 0.04f, 1.0f),
    scriptColor: new Color(1.0f, 0.2f, 0.4f),
    dictKeyColor: new Color(1.0f, 0.5f, 0.2f)),

new Theme("Cyber Matrix",
    fieldTypeColor: new Color(0.2f, 1.0f, 0.8f),
    propertyTypeColor: new Color(0.3f, 0.8f, 0.05f),
    valueColor: new Color(0.7f, 1.0f, 0.85f),
    nameColor: new Color(0.85f, 1.0f, 0.6f),
    prefixColor: new Color(0.7f, 0.87f, 0.15f),
    scriptColor: new Color(0.9f, 0.3f, 0.3f),
    dictKeyColor: new Color(0.5f, 0.9f, 1.0f)),

new Theme("Crimson Forge",
    fieldTypeColor: new Color(1.0f, 0.2f, 0.2f),
    propertyTypeColor: new Color(0.9f, 0.3f, 0.1f),
    valueColor: new Color(1.0f, 0.7f, 0.3f),
    nameColor: new Color(1.0f, 0.85f, 0.85f),
    prefixColor: new Color(1.0f, 0.5f, 0.3f),
    scriptColor: new Color(1.0f, 0.4f, 0.2f),
    dictKeyColor: new Color(1.0f, 0.4f, 0.6f))
            };
        }
    }
}