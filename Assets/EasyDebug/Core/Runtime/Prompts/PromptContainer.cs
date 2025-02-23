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

        public void UpdateTextPrompt(string key, string value, int priority)
        {
            if (_prompts.TryGetValue(key, out Prompt prompt))
            {
                ((TextPrompt)prompt).UpdateValue(value, priority);
                prompt.UpdateState();
            }
            else
            {
                var newPrompt = new TextPrompt(key, value, priority, _promptsHandler.transform);
                newPrompt.UpdateState();
                _prompts[key] = newPrompt;
                _sortedPrompts.Add(newPrompt);
                SortPrompts();
            }

            UpdatePromptPositions();
        }

        public void UpdateArrowPrompt(string key, Vector3 value, Color color, Vector3? localPosition = null)
        {
            if (_prompts.TryGetValue(key, out Prompt prompt))
            {
                ((ArrowPrompt)prompt).UpdateValue(value);
                prompt.UpdateState();
            }
            else
            {
                ArrowPrompt newPrompt;
                if (localPosition.HasValue)
                {
                    newPrompt = new ArrowPrompt(key, value, localPosition.Value, color, _promptsHandler.transform);
                }
                else
                {
                    newPrompt = new ArrowPrompt(key, value, color, _promptsHandler.transform);
                }
                newPrompt.UpdateState();
                _prompts[key] = newPrompt;
                _sortedPrompts.Add(newPrompt);
                SortPrompts();
            }

            UpdatePromptPositions();
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
