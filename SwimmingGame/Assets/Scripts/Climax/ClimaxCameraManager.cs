using System.Collections.Generic;
using UnityEngine;


public class ClimaxCameraManager : MonoBehaviour
{
    public Unity.Cinemachine.CinemachineVirtualCamera climaxCamera; 
    public List<Transform> cameraPositions; // Reference points for camera positions
    public List<float> cameraHoldDurations; // Time to hold each camera position
    public List<float> cameraZooms;
    public float slowMoTimeScale;
    public float transitionSpeed;
    public float cameraZoomSpeed;
    public float panRange; // Range for random pan movement

    public bool isClimaxCompleted = false;
    private bool isCameaPosUpdated = false;
    private bool effectActive = false;
    private int currentPosIndex;
    private float effectTimer;
    private float holdTimer;
    private Transform camTransform;
    private float originalTimeScale;
    private Vector3 randomPanOffset;


    void Start()
    {
        originalTimeScale = Time.timeScale;
        camTransform = climaxCamera.transform;
        GenerateNewRandomOffset();
    }

    void Update()
    {
        if (isClimaxCompleted && !effectActive)
        {
            StartClimaxCompletion();
        }

        if (effectActive)
        {
            UpdateClimaxCompletionEffect();
        }
    }

    public void TriggerClimaxCompletion()
    {
        isClimaxCompleted = true;
    }

    private void StartClimaxCompletion()
    {
        effectActive = true;
        effectTimer = 0f;
        holdTimer = 0f;
        Time.timeScale = slowMoTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private void UpdateClimaxCompletionEffect()
    {
        effectTimer += Time.unscaledDeltaTime;
        holdTimer += Time.unscaledDeltaTime;

        if (currentPosIndex < cameraPositions.Count)
        {
            // update new camera position
            Transform targetPos = cameraPositions[currentPosIndex];
            if (!isCameaPosUpdated)
            {
                camTransform.position = targetPos.position;
                camTransform.rotation = targetPos.rotation;
                isCameaPosUpdated = true;
            }


            // Lerp position and rotation to random pan offset
            camTransform.position = Vector3.Lerp(camTransform.position, targetPos.position + randomPanOffset, transitionSpeed * Time.unscaledDeltaTime);
            camTransform.rotation = Quaternion.Slerp(camTransform.rotation, targetPos.rotation, transitionSpeed * Time.unscaledDeltaTime);

            // Zoom in
            Unity.Cinemachine.CinemachineCameraOffset offset = climaxCamera.GetComponent<Unity.Cinemachine.CinemachineCameraOffset>();
            offset.Offset.z = Mathf.Lerp(offset.Offset.z, cameraZooms[currentPosIndex], cameraZoomSpeed * Time.unscaledDeltaTime);

            // Switch to next position after holdTimer completes
            if (holdTimer >= cameraHoldDurations[currentPosIndex])
            {
                holdTimer = 0f;
                GenerateNewRandomOffset();
                currentPosIndex++;
                isCameaPosUpdated = false;
            }
        }

        if (currentPosIndex >= cameraPositions.Count)
        {
            EndClimaxCompletion();
        }
    }

    private void GenerateNewRandomOffset()
    {
        if (currentPosIndex < cameraPositions.Count)
        {
            // Generate a new random pan offset
            randomPanOffset = new Vector3(
                Random.Range(-panRange, panRange),
                Random.Range(-panRange, panRange),
                Random.Range(-panRange, panRange)
            );
        }
    }
    private void EndClimaxCompletion()
    {
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = 0.02f;
        effectActive = false;
    }
}
