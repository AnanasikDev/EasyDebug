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
                    new Color(0.3f, 0.9f, 0.6f),
                    new Color(0.64f, 0.6f, 0.99f),
                    new Color(0.9f, 0.7f, 0.3f),
                    new Color(0.9f, 0.9f, 0.91f),
                    new Color(0.95f, 0.95f, 1.0f),
                    new Color(1.0f, 0.4f, 0.1f)),

                new Theme("Neon Eclipse",
                    new Color(0.0f, 0.9f, 0.7f),
                    new Color(1.0f, 0.3f, 0.9f),
                    new Color(1.0f, 0.7f, 0.3f),
                    new Color(0.9f, 0.9f, 1.0f),
                    new Color(0.6f, 0.3f, 1.0f),
                    new Color(1.0f, 0.2f, 0.3f)),

                new Theme("Cyber Matrix",
                    new Color(0.2f, 1.0f, 0.5f),
                    new Color(0.1f, 0.8f, 0.1f),
                    new Color(0.7f, 1.0f, 0.7f),
                    new Color(0.85f, 1.0f, 0.85f),
                    new Color(0.2f, 0.9f, 0.2f),
                    new Color(0.9f, 0.3f, 0.3f)),

                new Theme("Crimson Forge",
                    new Color(1.0f, 0.2f, 0.2f),
                    new Color(0.9f, 0.3f, 0.1f),
                    new Color(1.0f, 0.7f, 0.3f),
                    new Color(1.0f, 0.85f, 0.85f),
                    new Color(1.0f, 0.5f, 0.3f),
                    new Color(1.0f, 0.4f, 0.2f))
            };
        }
    }
}