using UnityEngine;
using EasyDebug.Prompts;

public class TextPromptExample : MonoBehaviour
{
    int h = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PromptManager.UpdateTextPrompt(gameObject, "health", (h++).ToString(), 1);   
        PromptManager.UpdateTextPrompt(gameObject, "name", "Player 1", 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PromptManager.UpdateTextPrompt(gameObject, "health", (h++).ToString(), 1);
            PromptManager.UpdateTextPrompt(gameObject, "name", "Player 1", 2);
        }
    }
}
