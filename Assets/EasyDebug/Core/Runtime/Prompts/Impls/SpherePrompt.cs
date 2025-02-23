using UnityEngine;

namespace EasyDebug.Prompts
{
    public class SpherePrompt : Prompt
    {
        public Vector3 position;
        public float radius;
        public Color color;

        /// <summary>
        /// Whether position is relative to the parent object or global
        /// </summary>
        public bool parentRelative = true;

        public SpherePrompt(string key, Vector3 localPosition, float radius, Color color, Transform parent, bool parentRelative = true)
        {
            type = PromptType.Sphere;
            Key = key;
            this.radius = radius;
            this.position = localPosition;
            this.color = color;
            this.parentRelative = parentRelative;
            _transform = parent;
            allPrompts.Add(this);
            PromptUpdater.onUpdateEvent += Update;
        }

        public void UpdateValue(Vector3 postition, float radius, Color color, bool parentRelative = true)
        {
            this.position = postition;
            this.radius = radius;
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
            RuntimeGizmos.DrawWireSphere((parentRelative ? _transform.position : Vector3.zero) + position, radius, color, Time.deltaTime, 24);
        }
    }
}