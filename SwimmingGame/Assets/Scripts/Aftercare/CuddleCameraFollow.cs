using UnityEngine;

public class CuddleCameraFollow : MonoBehaviour
{
    public TouchController touchController;
    public float smoothTime;
    private Vector3 lastMovementVector;
    private Vector3 targetPosition;
    private Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        ResetFollow();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Calculate the displacement of the camera movement vector since the last frame
        Vector3 currentMovementVector = touchController.cameraMovementVector;
        Vector3 displacement = currentMovementVector - lastMovementVector;
        lastMovementVector = currentMovementVector;

        // Ignore y axis movement
        displacement.y = 0;
        targetPosition = new Vector3(targetPosition.x + displacement.x, targetPosition.y, targetPosition.z + displacement.z);
        velocity.y = 0;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, Mathf.Infinity, Time.deltaTime);
    }

    void ResetFollow()
    {
        lastMovementVector = Vector3.zero;
        targetPosition = transform.position;
        velocity = Vector3.zero;
    }

}
