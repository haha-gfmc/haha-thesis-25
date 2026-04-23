using Cinemachine;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class SpringController : SexSpring
{
    [Header("Game Objects")]
    public Camera playerCamera;
    public CinemachineVirtualCamera virtualCamera;
    public Animator organAnimator;

    [Header("Camera Parameters")]
    public float defaultCameraLerpSpeed; // speed for the character to align with the camera direction
    public float quickCameraLerpSpeed;
    public float defaultCameraDistance;
    public float ZoomOutCameraDistance;
    public float ZoomInCameraDistance;
    public float shakeAmplitude = 0.2f; 
    public float shakeFrequency=3f;
    public NoiseSettings cameraNoiseSettings;
    private float ogShakeAmplitude, ogShakeFrequency;
    private NoiseSettings ogCameraNoiseSettings;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    public bool lockCamera;

    public bool canMove = true; // Flag to enable/disable movement

    private PlayerInput playerInput;
    private bool isAligningWithCamera = false;
    private bool isShaking;

    private Cinemachine3rdPersonFollow thirdPersonFollow;
    private Vector2 movementVector;
    private Vector3 initialCameraPosition;
    private int cameraDistance; // 0 zoom in, 1 default, 2 zoom out
    private float targetCameraDistance;
    private float cameraLerpSpeed;

    private EventInstance chargingInstance;
    private bool justExhaled;

    void Start()
    {
        SpringStart();

        playerInput = FindObjectOfType<PlayerInput>();
        thirdPersonFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        chargingInstance = RuntimeManager.CreateInstance("event:/Sex/Charge");

        cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        ogShakeAmplitude=cinemachineBasicMultiChannelPerlin.m_AmplitudeGain;
        ogShakeFrequency=cinemachineBasicMultiChannelPerlin.m_FrequencyGain;
        ogCameraNoiseSettings=cinemachineBasicMultiChannelPerlin.m_NoiseProfile;
    }

    private void Update()
    {
        if (!canMove) return; // Disable movement logic if canMove is false

        if (playerInput.movingForward && !playerInput.prevMovingForward)
        {
            StartInhaling();
            // Enable screenshake and quick camera lerp speed
            isShaking = true;
            cameraLerpSpeed = quickCameraLerpSpeed;
            chargingInstance.start();
        }

        if (!playerInput.movingForward && playerInput.prevMovingForward)
        {
            StartExhaling();
            // Disable screenshake and return to default camera lerp speed
            isShaking = false;
            cameraLerpSpeed = defaultCameraLerpSpeed;

            chargingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Sound.PlayOneShotVolume("event:/Sex/Thrust", 1f, "force", 1f);
        }

        inhaleTimeModifier = playerInput.movingForwardValue;

        SpringUpdate();

        if (isInhaling)
        {
            Rumble.AddRumble("Inhaling", Mathf.Clamp(inhaleTime / maxInhaleTime, 0f, 1f));
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return; // Disable movement logic if canMove is false

        if (!playerInput.aiming && !lockCamera)
        {
            AlignWithCamera();
        }
        else
        {
            isAligningWithCamera = false;
            cameraDistance = 2;
        }

        if (playerInput.currentControlScheme == "Gamepad")
        {
            HandleTurning(new Vector2(playerInput.look.x, -playerInput.look.y), lockCamera);
        }
        else
        {
            ConvertMovementInput(playerInput.movingForward, playerInput.movingBackward, playerInput.movingLeft, playerInput.movingRight);
            HandleTurning(movementVector, lockCamera);
        }

        if (isShaking)
        {
            InhaleCameraFeedback(shakeAmplitude,shakeFrequency,cameraNoiseSettings);
            if (isAligningWithCamera) // If not zooming out
            {
                cameraDistance = 0;
            }
        }
        else
        {
            InhaleCameraFeedback(ogShakeAmplitude,ogShakeFrequency,ogCameraNoiseSettings);
            if (isAligningWithCamera) // If not zooming out
            {
                cameraDistance = 1;
            }
        }
        if (organAnimator != null)
        {
            organAnimator.SetBool("Inhaling", isInhaling);
            organAnimator.SetBool("Exhaling", isExhaling);
            if (isInhaling && justExhaled)
            {
                organAnimator.SetFloat("Blend", Random.Range(0f, 1f));
                justExhaled = false;
            }
        }
        LerpCameraDistance();
        SpringFixedUpdate();
        if (!justExhaled){
            justExhaled = isExhaling;
        }

    }

    void ConvertMovementInput(bool movingForward, bool movingBackward, bool movingLeft, bool movingRight)
    {
        if (!canMove) return; // Disable movement logic if canMove is false

        // Reset movementVector before accumulating input
        movementVector.x = 0f;
        movementVector.y = 0f;

        // Determine movement direction based on input
        if (movingForward) { movementVector.x += 1; }
        if (movingBackward) { movementVector.x -= 1; }
        if (movingLeft) { movementVector.y += 1; }
        if (movingRight) { movementVector.y -= 1; }

        // Normalize the vector only if it has a non-zero length
        if (movementVector.magnitude > 1f)
        {
            movementVector.Normalize();
        }
    }

    // Align character with camera direction
    void AlignWithCamera()
    {
        if (!canMove) return; // Disable movement logic if canMove is false

        Vector3 cameraForward = playerCamera.transform.forward;
        targetRotation = Quaternion.LookRotation(cameraForward); // Set target rotation based on camera direction
        isAligningWithCamera = true; // Start aligning
        characterRb.MoveRotation(Quaternion.Lerp(characterRb.rotation, targetRotation, cameraLerpSpeed * Time.fixedDeltaTime));
        characterRb.angularVelocity = Vector3.zero; // Clear all angular acceleration
    }

    void ZoomOutCamera()
    {
        if (!canMove) return; // Disable movement logic if canMove is false

        isAligningWithCamera = false;

        // Smoothly restore the original camera distance
        if (thirdPersonFollow != null)
        {
            thirdPersonFollow.CameraDistance = Mathf.Lerp(thirdPersonFollow.CameraDistance, ZoomOutCameraDistance, cameraLerpSpeed * Time.fixedDeltaTime);
        }
    }

    void InhaleCameraFeedback(float amplitude, float frequency, NoiseSettings noiseSettings)
    {
        // Shake and zoom in camera
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = amplitude;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = frequency;
        cinemachineBasicMultiChannelPerlin.m_NoiseProfile=noiseSettings;
    }

    void LerpCameraDistance()
    {
        if (!canMove) return; // Disable movement logic if canMove is false

        switch (cameraDistance)
        {
            case 0:
                targetCameraDistance = ZoomInCameraDistance;
                break;
            case 1:
                targetCameraDistance = defaultCameraDistance;
                break;
            case 2:
                targetCameraDistance = ZoomOutCameraDistance;
                break;
        }
        thirdPersonFollow.CameraDistance = Mathf.Lerp(thirdPersonFollow.CameraDistance, targetCameraDistance, cameraLerpSpeed * Time.fixedDeltaTime);
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    void OnDestroy()
    {
        chargingInstance.release();
    }
}