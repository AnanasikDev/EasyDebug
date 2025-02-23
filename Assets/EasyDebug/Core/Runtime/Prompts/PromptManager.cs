using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyDebug.Prompts
{
    public static class PromptManager
    {
        public static float TextSize = 1.3f;
        public static float PromptDistance = 0.4f;

#if UNITY_EDITOR
        private static bool _showAll = true; // for editor default is true
#else
        private static bool _showAll = false; // for release default is false
#endif

        public static bool ShowAll
        {
            get
            {
                return _showAll;
            }
            set
            {
                if (_showAll != value)
                {
                    foreach (var container in PromptContainers.Values)
                    {
                        container.GetAllPrompts().ForEach(p => p.ToggleState(value));
                    }
                }
                _showAll = value;
            }
        }

        public static TextPromptTransformMode transformMode = TextPromptTransformMode.FaceCamera;
        public static string ShowOnlyWithName = null;
        public static Vector3 StartLocalOffset = new Vector3(0, 1.5f, 0);

        private static readonly Dictionary<GameObject, PromptContainer> PromptContainers = new();

        private static PromptContainer GetContainer(GameObject gameobject)
        {
            if (!PromptContainers.TryGetValue(gameobject, out var container))
            {
                container = new PromptContainer(gameobject);
                PromptContainers[gameobject] = container;
            }
            return container;
        }

        /// <summary>
        /// Updates or creates a text prompt above a gameobject.
        /// </summary>
        /// <param name="gameobject">The target GameObject.</param>
        /// <param name="key">The key for the text prompt.</param>
        /// <param name="value">The text to display.</param>
        /// <param name="priority">Priority for stacking the text prompt.</param>
        public static void UpdateTextPrompt(GameObject gameobject, string key, string value, int priority = 0)
        {
            GetContainer(gameobject).UpdateTextPrompt(key, value, priority);
        }

        public static void UpdateArrowPrompt(GameObject gameobject, string key, Vector3 value, Vector3 position, Color color)
        {
            GetContainer(gameobject).UpdateArrowPrompt(key, value, position, color);
        }

        public static void UpdateBoxPrompt(GameObject gameobject, string key, Vector3 position, float size, Color color)
        {
            GetContainer(gameobject).UpdateBoxPrompt(key, position, size, color);
        }

        public static void UpdateSpherePrompt(GameObject gameobject, string key, Vector3 position, float radius, Color color)
        {
            GetContainer(gameobject).UpdateSpherePrompt(key, position, radius, color);
        }

        public static void DestroyAllPrompts(GameObject gameobject)
        {
            if (PromptContainers.TryGetValue(gameobject, out PromptContainer container))
            {
                PromptContainers.Remove(gameobject);
                container.Destroy();
            }
        }

        /// <summary>
        /// Returns an array containing all gameobjects which have prompt containers attached to them, even if they are empty, unless includeEmpty is set to false.
        /// </summary>
        /// <returns></returns>
        public static GameObject[] GetAllGameobjects(bool includeEmpty = true)
        {
            return PromptContainers.Keys.ToArray();
        }
    }
}
