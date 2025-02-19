using System.Linq;
using TMPro;
using UnityEngine;

namespace EasyDebug.CommandLine
{
    public class CommandLine : MonoBehaviour
    {
        internal const float height = 30;

        internal Canvas canvas;
        internal GameObject handler;
        public static CommandLine instance;
        public bool isActive { get; private set; } = false;

        [SerializeField] private TMP_InputField inputField;

        public CommandLineEngine engine = new CommandLineEngine();
        public CommandLineSuggestions suggestions;

        public Status status
        {
            get
            {
                var parsed = engine.ParseInput(inputField.text);
                return engine.GetQueryStatus(parsed, inputField.text);
            }
        }

        private void Start()
        {
            if (instance == null) instance = this;
            Init();
        }

        public static void Create()
        {
            if (instance == null)
            {
                CommandLine prefab = Resources.Load<CommandLine>("RuntimeConsole");
                Debug.Log("Resource loaded as " + prefab);
                instance = Instantiate(prefab);
            }
            if (Application.isPlaying)
            {
                instance.Init();
            }
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
            inputField.onFocusSelectAll = false;

            engine.Init();
            suggestions.Init();
            OnInputChanged();
        }

        public void Update()
        {
            if (!inputField.isFocused && Input.GetKeyDown(KeyCode.Slash))
            {
                Toggle();
            }

            if (inputField.text != string.Empty && Input.GetKeyDown(KeyCode.Return))
            {
                Submit();
            }
        }

        public void OnInputChanged()
        {
            if (status == Status.EnteringObjectName)
            {
                suggestions.UpdateValues(engine.SuggestObjects(engine.ParseInput(inputField.text)).ToArray());
            }
            else if (status == Status.EnteringFunctionName)
            {
                suggestions.UpdateValues(engine.SuggestFunctions(engine.ParseInput(inputField.text)).ToArray());
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
            isActive = true;
            gameObject.SetActive(true);
            inputField.Select();
        }

        public void Hide()
        {
            isActive = false;
            gameObject.SetActive(false);
        }

        public enum Status
        {
            EnteringObjectName,
            EnteringFunctionName,
            EnteringArguments
        }
    }
}
