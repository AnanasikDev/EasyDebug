using UnityEngine;

public class TextPromptFaceCamera : MonoBehaviour
{
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_mainCamera != null)
        {
            transform.forward = _mainCamera.transform.forward;
        }
    }
}
