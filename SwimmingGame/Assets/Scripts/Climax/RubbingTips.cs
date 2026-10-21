using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbingTips : MonoBehaviour
{
    public GameObject[] object1;  
    public GameObject[] object2;  
    public float moveForce = 10f;  // Force applied for movement
    public float dragFactor = 0.95f;  // Drag factor to slow down objects
    public bool dontUseIndividualDirection;

    private PlayerInput playerInput;
    private Vector2 movementVector;
    private bool isUsingGamepad;
    private Vector3[] object1InitialVector;
    private Vector3[] object2InitialVector;

    private void Start()
    {
        object1InitialVector = new Vector3[object1.Length];
        object2InitialVector = new Vector3[object2.Length];
        playerInput = FindObjectOfType<PlayerInput>();
        for (int i = 0; i < object1.Length; i++)
            {object1InitialVector[i] = object1[i].transform.forward;}
        for (int i = 0; i < object2.Length; i++)
            {object2InitialVector[i] = object2[i].transform.forward;}
    }

    void FixedUpdate()
    {
        // handle input change
        if (playerInput.currentControlScheme == "Gamepad")
        {
            isUsingGamepad = true;
            for (int i = 0; i < object1.Length; i++)
            {
                ApplyMovement(object1[i], object1InitialVector[i], playerInput.look.x, playerInput.look.y);
            }
            for (int i = 0; i < object2.Length; i++)
            {
                ApplyMovement(object2[i], object2InitialVector[i], playerInput.rotation.x, -playerInput.rotation.y);
            }

        }
        else
        {
            isUsingGamepad = false;
            
            ConvertMovementInput(playerInput.movingForward, playerInput.movingBackward, playerInput.movingLeft, playerInput.movingRight);
            for (int i = 0; i < object1.Length; i++)
            {
                ApplyMovement(object1[i], object1InitialVector[i], playerInput.look.x, playerInput.look.y);
            }
            for (int i = 0; i < object2.Length; i++)
            {
                ApplyMovement(object2[i], object2InitialVector[i], -movementVector.y, movementVector.x);
            }
        }

    }

    void ConvertMovementInput(bool movingForward, bool movingBackward, bool movingLeft, bool movingRight)
    {
        // Reset movementVector before accumulating input
        movementVector.x = 0f;
        movementVector.y = 0f;

        // Determine movement direction based on input
        if (movingForward) { movementVector.x += 1; }
        if (movingBackward) { movementVector.x += -1; }
        if (movingLeft) { movementVector.y += 1; }
        if (movingRight) { movementVector.y += -1; }

        // Normalize the vector only if it has a non-zero length
        if (movementVector.magnitude > 1f)
        {
            movementVector.Normalize();
        }

        //Debug.Log(movementVector);
    }
    // Apply force-based movement based on input
    void ApplyMovement(GameObject gameObject, Vector3 direction, float inputX, float inputY)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (dontUseIndividualDirection)
        {
            // Calculate the movement direction in XZ plane
            Vector3 movementDirection = new Vector3(inputX, 0, inputY).normalized;
            // Apply force to the rigidbody based on input
            rb.AddForce(movementDirection * moveForce);
        }
        else
        {
            // Calculate the movement direction in XZ plane
            Vector3 movementDirection = new Vector3(inputX * direction.x, 0, inputY * direction.z).normalized;
            // Apply force to the rigidbody based on input
            rb.AddForce(movementDirection * moveForce);
        }




        // Apply drag to gradually slow down the object
        rb.linearVelocity *= dragFactor;
    }
}
