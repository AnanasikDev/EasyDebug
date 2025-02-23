using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            _promptsHandler = new GameObject("[debug] promptsHandler");
            _promptsHandler.transform.SetParent(_gameobject.transform);
            _promptsHandler.transform.localPosition = Vector3.zero;
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

        public void UpdateArrowPrompt(string key, Vector3 direction, Vector3 position, Color color)
        {
            UpdatePrompt(key, (Prompt p) =>
            {
                if (p == null)
                {
                    return new ArrowPrompt(key, direction, position, color);
                }
                else
                {
                    ((ArrowPrompt)p).UpdateValue(direction, position);
                    return p;
                }
            });
        }

        public void UpdateBoxPrompt(string key, Vector3 position, float size, Color color)
        {
            UpdatePrompt(key, (Prompt p) =>
            {
                if (p == null)
                {
                    return new BoxPrompt(key, position, size, color);
                }
                else
                {
                    ((BoxPrompt)p).UpdateValue(position, size, color);
                    return p;
                }
            });
        }

        public void UpdateSpherePrompt(string key, Vector3 position, float radius, Color color)
        {
            UpdatePrompt(key, (Prompt p) =>
            {
                if (p == null)
                {
                    return new SpherePrompt(key, position, radius, color);
                }
                else
                {
                    ((SpherePrompt)p).UpdateValue(position, radius, color);
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

        public GameObject GetHandlerGameobject()
        {
            return _promptsHandler;
        }

        public void Destroy()
        {
            foreach (Prompt p in _prompts.Values)
                p.ForceDestroy();
            _prompts.Clear();
            GameObject.Destroy(GetHandlerGameobject());
        }
    }
}
