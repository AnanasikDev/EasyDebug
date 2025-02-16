using UnityEngine;

namespace EasyDebug.Example
{
    internal class CameraController : MonoBehaviour
    {
        [SerializeField] float mouseSensitivityVertical = 1.0f;
        [SerializeField] float mouseSensitivityHorizontal = 1.0f;

        private Vector3 globalEulerAngles;
        private bool followMouse = true;

        private void Start()
        {
            //Cursor.lockState = CursorLockMode.Locked;

            // Initialize rotation angles from the current rotation
            globalEulerAngles = transform.eulerAngles;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                followMouse = !followMouse;
            }

            float hor = Input.GetAxis("Horizontal");
            float ver = Input.GetAxis("Vertical");

            if (followMouse )
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityHorizontal;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityVertical;
                
                // Update global rotation angles
                globalEulerAngles.x -= mouseY; // Subtract mouse Y for "inverted" vertical control
                globalEulerAngles.y += mouseX; // Add mouse X for horizontal control

                // Clamp vertical rotation to avoid flipping
                globalEulerAngles.x = Mathf.Clamp(globalEulerAngles.x, -90f, 90f);

                // Apply the updated rotation using global angles
                transform.rotation = Quaternion.Euler(globalEulerAngles);
            }

            // Move the camera based on its current direction
            transform.Translate(Vector3.forward * ver + Vector3.right * hor, Space.Self);
        }
    }
}
