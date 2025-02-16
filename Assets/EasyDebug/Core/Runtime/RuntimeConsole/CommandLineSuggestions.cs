using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace EasyDebug.CommandLine
{
    public class CommandLineSuggestions : MonoBehaviour
    {
        public Button buttonPrefab;  // Assign ButtonTemplate from Inspector
        public Transform contentPanel;   // Assign "Content" from Inspector
        public TMP_InputField inputField;

        public Pool<Button> buttons = new(x => x.gameObject.activeSelf);

        void Start()
        {
            buttons.createFunc = () =>
            {
                Button btn = Instantiate(buttonPrefab, contentPanel);
                btn.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(btn.GetComponentInChildren<TextMeshProUGUI>().text));
                return btn;
            };

            // Example: Creating 10 buttons
            /*for (int i = 0; i < 10; i++)
            {
                CreateButton($"Button {i + 1}");
            }*/
        }

        void CreateButton(string buttonText)
        {
            var btn = buttons.TakeInactiveOrCreate();
            btn.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
            btn.gameObject.SetActive(true);
        }

        public void UpdateValues(string[] values)
        {
            buttons.objects.ForEach(x => x.gameObject.SetActive(false));
            foreach (string v in values)
            {
                CreateButton(v);
            }
        }

        void OnButtonClick(string buttonText)
        {
            var parsed = CommandLine.instance.engine.ParseInput(inputField.text);
            PipeConsole.Print(parsed.Serialize(), inputField.text);
            parsed.functionName = buttonText;
            PipeConsole.Print(parsed.Serialize(), inputField.text);
            inputField.text = parsed.Serialize(); 
            //Debug.Log($"{buttonText} clicked!");
        }

        public void UpdateDropdownOptions(string[] options)
        {
            Debug.Log("Dropdown updated");
            // Clear existing options
            //dropdown.ClearOptions();
            UpdateValues(options);

            // Add new options
            //dropdown.AddOptions(options);
            //dropdown.Show();
        }
    }
}