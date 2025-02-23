using UnityEngine;

namespace EasyDebug.Prompts
{
    public class BoxPrompt : Prompt
    {
        public Vector3 position;
        public float size;
        public Color color;

        /// <summary>
        /// Whether position is relative to the parent object or global
        /// </summary>
        public bool parentRelative = true;

        public BoxPrompt(string key, Vector3 localPosition, float size, Color color, Transform parent, bool parentRelative = true)
        {
            type = PromptType.Box;
            Key = key;
            this.size = size;
            this.position = localPosition;
            this.color = color;
            this.parentRelative = parentRelative;
            _transform = parent;
            allPrompts.Add(this);
            PromptUpdater.onUpdateEvent += Update;
        }

        public void UpdateValue(Vector3 localPosition, float size, Color color, bool parentRelative = true)
        {
            this.position = localPosition;
            this.size = size;
            this.color = color;
            this.parentRelative = parentRelative;
        }

        public override void SetLocalPosition(Vector3 localPosition)
        {
        }

        public override void ToggleState(bool state)
        {
            _transform.gameObject.SetActive(state);
        }

        public override void ForceDestroy()
        {
            PromptUpdater.onUpdateEvent -= Update;
        }

        public override void Update()
        {
            RuntimeGizmos.DrawWireCube((parentRelative ? _transform.position : Vector3.zero) + position, size, color, Time.deltaTime);
        }
    }
}