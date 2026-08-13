using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform cameraRoot;
    public float keyboardMouseMouseSensitivity = 100f;  
    public float keyboardMouseLookSensitivity = 100f;  
    public float gamepadMouseSensitivity = 100f;  
    public float gamepadLookSensitivity = 100f;      
    public float rotationSmoothTime = 0.1f;
    public bool cameraLocked;

    private float xRotation = 0f;  // Current x-axis rotation
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Vector3 rotationVelocity;
    private PlayerInput playerInput;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Locks the cursor to the center of the screen
        playerInput = FindObjectOfType<PlayerInput>();
    }

    void Update()
    {
        if (!cameraLocked)
        {
            HandleMouseLook();
        }
    }

    void HandleMouseLook()
    {
        float mouseX = 0f;
        float mouseY = 0f;
        float lookX = 0f;
        float lookY = 0f;
        // Get mouse input
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            mouseX = playerInput.rotation.x * keyboardMouseMouseSensitivity * Time.fixedDeltaTime;
            mouseY = -playerInput.rotation.y * keyboardMouseMouseSensitivity * Time.fixedDeltaTime;
        }
        else if (playerInput.currentControlScheme == "Gamepad")
        {
            mouseX = playerInput.rotation.x * gamepadMouseSensitivity * Time.fixedDeltaTime;
            mouseY = -playerInput.rotation.y * gamepadMouseSensitivity * Time.fixedDeltaTime;
        }

        // Get look input
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            lookX = playerInput.look.x * keyboardMouseLookSensitivity * Time.fixedDeltaTime;
            lookY = -playerInput.look.y * keyboardMouseLookSensitivity * Time.fixedDeltaTime;
        }
        else if (playerInput.currentControlScheme == "Gamepad")
        {
            lookX = playerInput.look.x * gamepadLookSensitivity * Time.fixedDeltaTime;
            lookY = -playerInput.look.y * gamepadLookSensitivity * Time.fixedDeltaTime;
        }

        // Combine mouse input and look input
        targetRotation.y += mouseX + lookX;  // Horizontal rotation (combined)
        xRotation -= (mouseY + lookY);       // Vertical rotation (combined)
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);  // Clamp vertical rotation to prevent flipping

        // Set the target rotation for the cameraRoot
        targetRotation.x = xRotation;

        // Smoothly interpolate the camera's rotation for a smooth look
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, rotationSmoothTime);

        // Apply the smoothed rotation to the cameraRoot
        cameraRoot.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
    }
}

