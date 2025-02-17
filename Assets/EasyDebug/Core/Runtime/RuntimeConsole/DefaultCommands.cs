using TMPro;
using UnityEngine;

namespace EasyDebug.CommandLine
{
    public class DefaultCommands : MonoBehaviour
    {
        private Canvas canvas;
        private TextMeshProUGUI fpsText = null;

        private void Start()
        {
            canvas = FindAnyObjectByType<Canvas>();
        }


        [Command("show_fps", ConsoleCommandType.Global)]
        public void ShowFps()
        {
            if (fpsText != null || canvas == null) return;

            var obj = new GameObject("FPS_Text");
            obj.transform.SetParent(canvas.transform, false);

            fpsText = obj.AddComponent<TextMeshProUGUI>();
            fpsText.text = "FPS: 0";

            fpsText.fontSize = 14f;
            fpsText.color = Color.black;

            fpsText.alignment = TextAlignmentOptions.TopRight;

            var rectTransform = fpsText.rectTransform;
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(-10, -10);
        }

        [Command("hide_fps", ConsoleCommandType.Global)]
        public void HideFps()
        {
            if (fpsText == null) return;
            Destroy(fpsText.gameObject);
        }


        private void Update()
        {
            if (fpsText != null)
            {
                fpsText.text = $"FPS: {(1.0f / Time.deltaTime)}";
            }
        }
    }
}
