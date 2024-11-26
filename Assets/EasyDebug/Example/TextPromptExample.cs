using UnityEngine;

public class TextPromptExample : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextPromptManager.UpdateText(gameObject, "health", "100", 1);   
        TextPromptManager.UpdateText(gameObject, "name", "Player 1", 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
