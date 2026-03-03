using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public LayerMask sexPartnerMask;
    public float lerpSpeed = 1.0f; // the lerping speed for XZ movement
    public float yLerpSpeed = 1.0f; // the lerping speed for Y matching movement
    public float yOffset = 1.0f; // offset of the character from the top of touching objects
    public float targetRotationDamp;
    public float rotationLerpDamp; 
    public Quaternion rotationOffset; // offset the rotation after timing normal
    public bool lockRotation = false;
    public bool isMoving = true;
    [HideInInspector]public Vector2 moveXZ;
    public Transform handParentTransform;
    
    [Header("Startup Rotation")]
    public float initialRotationLerpSpeed = 10f;
    public float initialRotationAngleThreshold = 0.5f; // degrees difference when startup lerp finished
    private bool initialLerpDone = false;
    public Vector3 handParentInitialRotation;
    
    [Header("Rotation Clamps (degrees)")]
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
        // handle input change
        if (playerInput.currentControlScheme == "Gamepad")
        {
            isUsingGamepad = true;
        }
        else
        {
            isUsingGamepad = false;
            ConvertMovementInput(playerInput.movingForward, playerInput.movingBackward, playerInput.movingLeft, playerInput.movingRight);
        }

        if (isMoving)
        {
            Moving();
        }
        HandlingInput();
        AdjustPositionAndRotation();
    }

    public void SetHandParentTransform(Transform handParent)
    {
        handParentTransform = handParent;
    }

    void HandlingInput()
    {
        // using keyboard
        if (!isUsingGamepad)
        {
            moveXZ.x = -movementVector.y;
            moveXZ.y = movementVector.x;
        }
        // using gamepad
        else
        {
            moveXZ.x = playerInput.look.x;
            moveXZ.y = -playerInput.look.y;
        }
    }

    // handle movement of the character around
    void Moving()
    {
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
