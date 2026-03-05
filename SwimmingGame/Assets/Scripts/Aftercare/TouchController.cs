using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TouchController : MonoBehaviour
{
    [Header("Hand Position Settings")]
    public Transform handParentTransform;
    public bool enableHandMovement = true;
    public float moveSpeed = 5f;
    public LayerMask sexPartnerMask;
    public float lerpSpeed = 1.0f; // the lerping speed for XZ movement
    public float yLerpSpeed = 1.0f; // the lerping speed for Y matching movement
    public float yOffset = 1.0f; // offset of the character from the top of touching objects

    [HideInInspector]public Vector2 moveXZ;

    [Header("Right Stick Caress Settings")]
    public bool enableCaress = false;
    public float caressSpeed = 5f;
    public float caressLerpSpeed = 1.0f;
    private Vector3 clampCircleCenter;
    public float boundingCircleRadius = 1.0f;
    [Header("Startup Rotation")]
    public float initialRotationLerpSpeed = 10f;
    public float initialRotationAngleThreshold = 0.5f; // degrees difference when startup lerp finished
    private bool initialLerpDone = false;
    public Vector3 handParentInitialRotation;
    
    [Header("Rotation Clamps (degrees)")]
    public float targetRotationDamp;
    public float rotationLerpDamp; 
    public Quaternion rotationOffset; // offset the rotation after timing normal
    public bool lockRotation = false;

    public bool enableRotationClamp = false;
    public Vector3 minEulerClamp = new Vector3(-90f, -180f, -90f);
    public Vector3 maxEulerClamp = new Vector3(90f, 180f, 90f);

    public float rotationDampFactor = 1.0f; // public variable to control how much to dampen rotation

    private PlayerInput playerInput;
    private Vector3 targetPosition;
    [HideInInspector]public Quaternion initialRotation;
    private Vector2 movementVector;

    private bool isUsingGamepad;
    [HideInInspector] public BoxCollider boundingBox; // bounding box for movement
    private Vector3 initialPosition; 

    // clamp reference: captures orientation at end of startup lerp
    private Quaternion clampReferenceRotation = Quaternion.identity;
    private bool clampReferenceSet = false;
    public Vector3 handMoveVector;

    private void Awake()
    {

        if (handParentTransform != null)
        {
            //handParentTransform.rotation = Quaternion.Euler(handParentInitialRotation);
        }
        initialRotation = transform.rotation;
    }

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        targetPosition = transform.position;
        initialPosition = transform.position;
    }



    private void FixedUpdate()
    {
        handMoveVector = Vector3.zero; 
        if (initialLerpDone && !enableCaress)
        {
            StartCoroutine(EnableCaressCoroutine());
        }
        if (enableHandMovement)
        {
            Moving();
        }
        if (enableCaress)
        {
            Caressing();
        }
        AdjustPositionAndRotation();
    }

    public void SetHandParentTransform(Transform handParent)
    {
        handParentTransform = handParent;
    }

    private IEnumerator EnableCaressCoroutine()
    {
        clampCircleCenter = transform.position; 
        yield return new WaitForSeconds(.5f);
        enableCaress = true;
    }

    // handle movement of the character around
    void Moving()
    {
        // handle input change
        if (playerInput.currentControlScheme == "Gamepad")
        {
            isUsingGamepad = true;
            moveXZ.x = playerInput.look.x;
            moveXZ.y = -playerInput.look.y;
        }
        else
        {
            isUsingGamepad = false;
            ConvertMovementInput(playerInput.movingForward, playerInput.movingBackward, playerInput.movingLeft, playerInput.movingRight);
            moveXZ.x = movementVector.x;
            moveXZ.y = -movementVector.y;
        }
        // calculate the target position based on input
        Vector3 move = new Vector3(moveXZ.x, 0, moveXZ.y) * moveSpeed * Time.fixedDeltaTime;
        targetPosition = transform.position + move;

        // Constrain the target position within the bounding box
        if (boundingBox != null)
        {
            targetPosition = ClampToBoundingBox(targetPosition);
        }

        // lerp the character to the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        handMoveVector = move;
    }

    void Caressing()
    {
        float moveX = isUsingGamepad ? playerInput.rotation.x : playerInput.look.x;
        float moveY = isUsingGamepad ? -playerInput.rotation.y : -playerInput.look.y;
        // calculate the target position based on input
        Vector3 move = new Vector3(moveX, 0, moveY) * caressSpeed * Time.fixedDeltaTime;
        targetPosition = ClampToBoundingCircle(transform.position + move);
        // lerp the character to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, caressLerpSpeed * Time.fixedDeltaTime);
    }

    Vector3 ClampToBoundingCircle(Vector3 target)
    {
        if (handMoveVector.magnitude > 0.01f)
        {
            Debug.Log("Reset Caress Center");
            clampCircleCenter = transform.position; 
        }
        else
        {
            Debug.Log("Same Caress Center");
        }
        // Clamp the Hand's position to the bounding circle around the clampCircleCenter
        Vector3 direction = new Vector3(target.x - clampCircleCenter.x, 0, target.z - clampCircleCenter.z);
        direction = direction.normalized;
        if (direction.magnitude > boundingCircleRadius)
        {
            direction = direction.normalized * boundingCircleRadius;
        }
        return new Vector3(clampCircleCenter.x + direction.x, target.y, clampCircleCenter.z + direction.z);
    }

    Vector3 ClampToBoundingBox(Vector3 target)
    {
        // Transform the target position into the local space of the bounding box
        Vector3 localTarget = boundingBox.transform.InverseTransformPoint(target);

        // Get the local bounds of the BoxCollider
        Vector3 localMin = boundingBox.center - boundingBox.size * 0.5f;
        Vector3 localMax = boundingBox.center + boundingBox.size * 0.5f;

        // Clamp the local position to the bounds of the box
        localTarget.x = Mathf.Clamp(localTarget.x, localMin.x, localMax.x);
        localTarget.y = Mathf.Clamp(localTarget.y, localMin.y, localMax.y);
        localTarget.z = Mathf.Clamp(localTarget.z, localMin.z, localMax.z);

        // Transform the clamped local position back into world space
        return boundingBox.transform.TransformPoint(localTarget);
    }

    void ConvertMovementInput(bool movingForward, bool movingBackward, bool movingLeft, bool movingRight)
    {
        // Reset movementVector before accumulating input
        movementVector.x = 0f;
        movementVector.y = 0f;

        // Determine movement direction based on input
        if (movingForward) { movementVector.y -= 1; }
        if (movingBackward) { movementVector.y += 1; }
        if (movingLeft) { movementVector.x -= 1; }
        if (movingRight) { movementVector.x += 1; }

        // Normalize the vector only if it has a non-zero length
        if (movementVector.magnitude > 1f)
        {
            movementVector.Normalize();
        }
    }

    // adjust the hand's Y position and rotation based on the sex partner mesh below
    void AdjustPositionAndRotation()
    {
        RaycastHit hit;

        // casting a ray downward to find the highest object
        if (Physics.Raycast(new Vector3(transform.position.x, 100f, transform.position.z), Vector3.down, out hit, Mathf.Infinity, sexPartnerMask))
        {
            // set Y position to the top of the hit object
            float targetY = hit.point.y + yOffset;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, targetY, yLerpSpeed * Time.fixedDeltaTime), transform.position.z);
            if (!lockRotation)
            {
                // During startup: quickly lerp to the rotation offset relative to the initial rotation
                if (!initialLerpDone)
                {
                    Quaternion startupTarget = initialRotation * rotationOffset;
                    transform.rotation = Quaternion.Lerp(transform.rotation, startupTarget, initialRotationLerpSpeed * Time.fixedDeltaTime);
                    if (Quaternion.Angle(transform.rotation, startupTarget) < initialRotationAngleThreshold)
                    {
                        initialLerpDone = true;
                        transform.rotation = startupTarget; // snap to exact target when close
                        // once initial lerp is finished we capture this orientation as the clamp baseline
                        if (enableRotationClamp && !clampReferenceSet)
                        {
                            clampReferenceRotation = transform.rotation;
                            clampReferenceSet = true;
                        }
                    }
                    // if clamps enabled, clamp the current rotation once we've reached it
                    if (initialLerpDone && enableRotationClamp)
                    {
                        transform.rotation = ClampRotation(transform.rotation);
                    }
                }
                else
                {
                    // After startup: orient to surface normal + offsets, then smooth toward it
                    Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * initialRotation * rotationOffset;
                    targetRotation = Quaternion.Lerp(Quaternion.identity, targetRotation, targetRotationDamp);
                    Quaternion newRot = Quaternion.Lerp(transform.rotation, targetRotation, rotationDampFactor * Time.fixedDeltaTime);
                    if (enableRotationClamp)
                    {
                        newRot = ClampRotation(newRot);
                    }
                    transform.rotation = newRot;
                }
            }
            // apply clamp after all lerping to guarantee final orientation stays within bounds
            if (enableRotationClamp)
            {
                transform.rotation = ClampRotation(transform.rotation);
            }
        }
    }

    // Clamp a quaternion's euler angles (degrees).
    private Quaternion ClampRotation(Quaternion q)
    {
        if (clampReferenceSet)
        {
            // compute rotation relative to baseline
            Quaternion relative = Quaternion.Inverse(clampReferenceRotation) * q;
            Vector3 e = relative.eulerAngles;
            e.x = NormalizeAngle(e.x);
            e.y = NormalizeAngle(e.y);
            e.z = NormalizeAngle(e.z);

            e.x = Mathf.Clamp(e.x, minEulerClamp.x, maxEulerClamp.x);
            e.y = Mathf.Clamp(e.y, minEulerClamp.y, maxEulerClamp.y);
            e.z = Mathf.Clamp(e.z, minEulerClamp.z, maxEulerClamp.z);

            Quaternion clampedRel = Quaternion.Euler(e);
            return clampReferenceRotation * clampedRel;
        }
        else
        {
            Vector3 e = q.eulerAngles;
            e.x = NormalizeAngle(e.x);
            e.y = NormalizeAngle(e.y);
            e.z = NormalizeAngle(e.z);

            e.x = Mathf.Clamp(e.x, minEulerClamp.x, maxEulerClamp.x);
            e.y = Mathf.Clamp(e.y, minEulerClamp.y, maxEulerClamp.y);
            e.z = Mathf.Clamp(e.z, minEulerClamp.z, maxEulerClamp.z);

            return Quaternion.Euler(e);
        }
    }
    // normalize is only useful if there is an euler angle wrap-around from 360 to 0
    // guess its not really useful in our case but ill leave it here anyway
    private float NormalizeAngle(float angle)
    {
        angle = (angle + 180f) % 360f;
        if (angle < 0f) angle += 360f;
        return angle - 180f;
    }
}
