using TMPro;
using UnityEngine;

namespace EasyDebug
{
    public class CommandLine : MonoBehaviour
    {
        private float height = 30;

        private Canvas canvas;
        private GameObject handler;
        public static CommandLine instance;
        public bool isActive { get; private set; } = false;

        [SerializeField] private TMP_InputField inputField;
        CommandLineEngine engine = new CommandLineEngine();

        public static void Create()
        {
            if (instance == null)
            {
                CommandLine prefab = Resources.Load<CommandLine>("RuntimeConsole");
                Debug.Log("Resource loaded as " + prefab);
                instance = Instantiate(prefab);
            }
            instance.Init();
        }

        public static void Delete()
        {
            if (instance != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(instance.gameObject);
#else
                Destroy(instance.gameObject);
#endif
            }
            var foundInstance = FindFirstObjectByType<CommandLine>();
            if (foundInstance != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(foundInstance.gameObject);
#else
                Destroy(foundInstance.gameObject);
#endif
            }
        }

        private void Init()
        {
            canvas = GameObject.FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                canvas = new GameObject("Canvas").AddComponent<Canvas>();
            }
            instance.gameObject.transform.SetParent(canvas.transform);

            var rectTransform = GetComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);
            rectTransform.offsetMin = new Vector2(0, -height);
            rectTransform.offsetMax = Vector3.zero;

            engine.Init();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Slash))
            {
                Toggle();
            }
        }

        public void Submit()
        {
            engine.Execute(inputField.text);
            Clear();
        }

        public void Clear()
        {
            inputField.text = string.Empty;
        }

        public void Toggle()
        {
            isActive = !isActive;
            if (isActive) Show();
            else Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            inputField.Select();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
