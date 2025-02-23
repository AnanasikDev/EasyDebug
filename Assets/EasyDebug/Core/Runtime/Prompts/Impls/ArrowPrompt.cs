using UnityEngine;

namespace EasyDebug.Prompts
{
    public class ArrowPrompt : Prompt
    {
        public Vector3 vector;
        public Vector3 localPosition;
        public Color color = Color.white;
        public bool show = true;

        public ArrowPrompt(string key, Vector3 vector, Color color, Transform parent)
        {
            type = PromptType.Arrow;
            Key = key;
            _transform = parent;

            this.color = color;
            this.vector = vector;

            allPrompts.Add(this);
            PromptUpdater.onUpdateEvent += Update;
        }

        public ArrowPrompt(string key, Vector3 vector, Vector3 localPosition, Color color, Transform parent)
        {
            type = PromptType.Arrow;
            Key = key;
            _transform = parent;

            this.color = color;
            this.vector = vector;
            this.localPosition = localPosition;

            allPrompts.Add(this);
            PromptUpdater.onUpdateEvent += Update;
        }

        public void UpdateValue(Vector3 vector)
        {
            this.vector = vector;
        }

        public void UpdateValue(Vector3 vector, Vector3 localPosition)
        {
            this.vector = vector;
            this.localPosition = localPosition;
        }

        public override void UpdateState()
        {
            ToggleState(PromptManager.ShowAll);
        }

        public override void SetLocalPosition(Vector3 localPosition)
        {
            this.localPosition = localPosition;
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
            RuntimeGizmos.DrawArrow(_transform.position + localPosition, vector, duration: Time.deltaTime, color: color);
        }
    }
}