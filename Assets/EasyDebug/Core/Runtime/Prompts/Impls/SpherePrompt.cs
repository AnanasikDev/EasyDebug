using UnityEngine;

namespace EasyDebug.Prompts
{
    public class SpherePrompt : Prompt
    {
        public Vector3 position;
        public float radius;
        public Color color;

        public bool show = true;

        public SpherePrompt(string key, Vector3 localPosition, float radius, Color color)
        {
            type = PromptType.Sphere;
            Key = key;
            this.radius = radius;
            this.position = localPosition;
            this.color = color;
            allPrompts.Add(this);
            PromptUpdater.onUpdateEvent += Update;
        }

        public void UpdateValue(Vector3 postition, float radius, Color color)
        {
            this.position = postition;
            this.radius = radius;
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

            RuntimeGizmos.DrawWireSphere(position, radius, color, Time.deltaTime, 24);
        }
    }
}