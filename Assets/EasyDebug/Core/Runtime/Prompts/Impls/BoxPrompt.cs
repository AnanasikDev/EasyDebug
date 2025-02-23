using UnityEngine;

namespace EasyDebug.Prompts
{
    public class BoxPrompt : Prompt
    {
        public Vector3 position;
        public float size;
        public Color color;

        public bool show = true;

        public BoxPrompt(string key, Vector3 localPosition, float size, Color color)
        {
            type = PromptType.Box;
            Key = key;
            this.size = size;
            this.position = localPosition;
            this.color = color;
            allPrompts.Add(this);
            PromptUpdater.onUpdateEvent += Update;
        }

        public void UpdateValue(Vector3 position, float size, Color color)
        {
            this.position = position;
            this.size = size;
            this.color = color;
        }

        public override void SetLocalPosition(Vector3 localPosition)
        {
        }

        public override void ToggleState(bool state)
        {
            show = state;
        }

        public override void ForceDestroy()
        {
            PromptUpdater.onUpdateEvent -= Update;
        }

        public override void Update()
        {
            if (!show) return;

            RuntimeGizmos.DrawWireCube(position, size, color, Time.deltaTime);
        }
    }
}