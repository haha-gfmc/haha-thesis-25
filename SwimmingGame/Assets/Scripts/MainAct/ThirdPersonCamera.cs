using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform cameraRoot;
    public float sensitivity = 100f;  // sensitivity for looking around
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
        // Get mouse input
        float mouseX = playerInput.rotation.x * sensitivity * Time.fixedDeltaTime;
        float mouseY = -playerInput.rotation.y * sensitivity * Time.fixedDeltaTime;

        // Get look input
        float lookX = playerInput.look.x * sensitivity * Time.fixedDeltaTime;
        float lookY = -playerInput.look.y * sensitivity * Time.fixedDeltaTime;

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

