using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDebug.Prompts
{
    public class PromptContainer
    {
        /// <summary>
        /// Gameobject to which container is attached to
        /// </summary>
        private GameObject _gameobject;
        private GameObject _promptsHandler;
        private Dictionary<string, Prompt> _prompts;
        private List<Prompt> _sortedPrompts;

        public PromptContainer(GameObject gameobject)
        {
            _sortedPrompts = new List<Prompt>();
            _prompts = new Dictionary<string, Prompt>();

            _gameobject = gameobject;
            _promptsHandler = new GameObject("TextPrompts");
            _promptsHandler.transform.SetParent(_gameobject.transform);
        }

        private void UpdatePrompt(string key, Func<Prompt, Prompt> createFunc)
        {
            if (_prompts.TryGetValue(key, out Prompt prompt))
            {
                createFunc(prompt);
                prompt.UpdateState();
            }
            else
            {
                Prompt newPrompt = createFunc(null);
                newPrompt.UpdateState();
                _prompts[key] = newPrompt;
                _sortedPrompts.Add(newPrompt);
                SortPrompts();
            }

            UpdatePromptPositions();
        }

        public void UpdateTextPrompt(string key, string value, int priority)
        {
            UpdatePrompt(key, (Prompt p) =>
            {
                if (p == null)
                {
                    return new TextPrompt(key, value, priority, _promptsHandler.transform);
                }
                else
                {
                    ((TextPrompt)p).UpdateValue(value, priority);
                    return p;
                }
            });
        }

        public void UpdateArrowPrompt(string key, Vector3 direction, Color color, Vector3? localPosition = null)
        {
            UpdatePrompt(key, (Prompt p) =>
            {
                if (p == null)
                {
                    if (localPosition.HasValue)
                    {
                        return new ArrowPrompt(key, direction, localPosition.Value, color, _promptsHandler.transform);
                    }
                    else
                    {
                        return new ArrowPrompt(key, direction, color, _promptsHandler.transform);
                    }
                }
                else
                {
                    ((ArrowPrompt)p).UpdateValue(direction);
                    return p;
                }
            });
        }

        public void UpdateBoxPrompt(string key, Vector3 position, float size, Color color, bool parentRelative = true)
        {
            UpdatePrompt(key, (Prompt p) =>
            {
                if (p == null)
                {
                    return new BoxPrompt(key, position, size, color, _promptsHandler.transform, parentRelative);
                }
                else
                {
                    ((BoxPrompt)p).UpdateValue(position, size, color, parentRelative);
                    return p;
                }
            });
        }

        private void SortPrompts()
        {
            _sortedPrompts.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        private void UpdatePromptPositions()
        {
            int p = 0;
            foreach (Prompt prompt in _sortedPrompts)
            {
                if (prompt.type == PromptType.Text)
                {
                    prompt.SetLocalPosition(PromptManager.StartLocalOffset + Vector3.up * p * PromptManager.PromptDistance);
                    p++;
                }
            }
        }

        public List<Prompt> GetAllPrompts()
        {
            return _sortedPrompts;
        }

        public GameObject GetGameobject()
        {
            return _promptsHandler;
        }
    }
}
