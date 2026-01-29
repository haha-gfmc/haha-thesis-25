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
    public Vector2 moveXZ;

    public float rotationDampFactor = 1.0f; // public variable to control how much to dampen rotation

    private PlayerInput playerInput;
    private Vector3 targetPosition;
    public Quaternion initialRotation;
    private Vector2 movementVector;

    private bool isUsingGamepad;

    public BoxCollider boundingBox; // bounding box for movement
    private Vector3 initialPosition; 

    private void Awake()
    {
        initialRotation = Quaternion.identity;
        Debug.Log("Awake - Transform rotation: " + transform.rotation.eulerAngles + ", InitialRotation: " + initialRotation.eulerAngles);
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

    // adjust the character's Y position and rotation based on the sex partner below
    void AdjustPositionAndRotation()
    {
        RaycastHit hit;

        // casting a ray downward to find the highest object
        if (Physics.Raycast(new Vector3(transform.position.x, 100f, transform.position.z), Vector3.down, out hit, Mathf.Infinity, sexPartnerMask))
        {
            // set Y position to the top of the hit object
            Vector3 positionTarget = new Vector3(transform.position.x, hit.point.y + yOffset, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, positionTarget, yLerpSpeed * Time.fixedDeltaTime);
            if (!lockRotation)
            {
                // Calculate the rotation offset based on the initial rotation
                Quaternion targetRotation = Quaternion.FromToRotation(Vector3.right, hit.normal) * initialRotation * rotationOffset;
                targetRotation = Quaternion.Lerp(Quaternion.identity, targetRotation, targetRotationDamp);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationDampFactor * Time.fixedDeltaTime);
            }
        }
    }
}
