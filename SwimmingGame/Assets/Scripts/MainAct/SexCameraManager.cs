using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class SexCameraManager : MonoBehaviour
{
    public CameraSet[] cameraGroups; // array of sets of cameras
    public int currentGroupIndex = 0; // index to control which group is active
    public GameObject targetObject; // The game object to monitor (e.g., Player)
    public ThirdPersonCamera thirdPersonCamera;
    public GameObject bodyCollider;

    private int spacePressCount = 0; // count of space presses
    private bool isObjectInsideTrigger = true; // assume the object starts inside the trigger
    public SpringController springController;

    void Start()
    {
        if (bodyCollider != null){
            bodyCollider.SetActive(false);
        }
        // initialize cameras for the current group
        InitializeCameras();
    }

    void Update()
    {

    }

    // initializes cameras for the current group
    void InitializeCameras()
    {
        if (currentGroupIndex < cameraGroups.Length)
        {
            CameraSet currentSet = cameraGroups[currentGroupIndex];

            // Start with fixedPOV active by default
            currentSet.mainCamera.gameObject.SetActive(true);
            currentSet.fixedPOV.gameObject.SetActive(true);
            currentSet.followCamera.gameObject.SetActive(false);
        }
        thirdPersonCamera.cameraLocked = true;
    }

    // switch cameras based on the count of space presses
    void SwitchCameras()
    {
        if (currentGroupIndex < cameraGroups.Length)
        {
            CameraSet currentSet = cameraGroups[currentGroupIndex];

            if (spacePressCount == 1)
            {
                currentSet.fixedPOV.gameObject.SetActive(true);
                currentSet.followCamera.gameObject.SetActive(false);
            }
            else if (spacePressCount == 2)
            {
                currentSet.fixedPOV.gameObject.SetActive(false);
                currentSet.followCamera.gameObject.SetActive(true);
            }
            else if (spacePressCount >= 3)
            {
                currentSet.followCamera.gameObject.SetActive(false);
                spacePressCount = 0; // reset the counter
            }
        }
    }

    public void SetCameraGroup(int index)
    {
        if (index >= 0 && index < cameraGroups.Length)
        {
            currentGroupIndex = index;
            spacePressCount = 0; // reset the space press count
            InitializeCameras(); // initialize cameras for the new group
        }
    }


    // Trigger detection when an object exits the volume
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == targetObject) // check if the exited object is the targetObject
        {
            isObjectInsideTrigger = false;
            CameraSet currentSet = cameraGroups[currentGroupIndex];
            currentSet.fixedPOV.gameObject.SetActive(false);
            thirdPersonCamera.cameraLocked = false;
            springController.lockCamera = false;
            if (bodyCollider != null)
            {
                bodyCollider.SetActive(true);
            }

        }
    }
}