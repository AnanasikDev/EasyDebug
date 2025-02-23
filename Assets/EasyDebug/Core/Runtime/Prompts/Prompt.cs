using System.Collections.Generic;
using UnityEngine;

namespace EasyDebug.Prompts
{
    public abstract class Prompt
    {
        public static readonly List<Prompt> allPrompts = new List<Prompt>();

        public PromptType type;

        public string Key { get; protected set; }
        public int Priority { get; protected set; }
        public Transform _transform;

        public abstract void ForceDestroy();

        public virtual void ToggleState(bool state)
        {
            _transform.gameObject.SetActive(state);
        }

        public virtual void SetLocalPosition(Vector3 localPosition)
        {
            _transform.localPosition = localPosition;
        }

        public virtual void UpdateState() { ToggleState(PromptManager.ShowAll); }

        public abstract void Update();
    }

    public enum PromptType
    {
        Text,
        Arrow
    }
}