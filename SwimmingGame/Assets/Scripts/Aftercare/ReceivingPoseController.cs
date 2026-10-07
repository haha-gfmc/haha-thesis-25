using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceivingPoseController : MonoBehaviour
{
    public enum PoseDirection
    {
        Neutral,
        Up,
        Down,
        Left,
        Right
    }

    public enum TransitionStyle
    {
        StopMotion,
        Tweened
    }


    [Serializable]
    public class DirectionalPose
    {
        public PoseDirection direction;

        [Header("Animation")]
        public string transitionState;
        public string fullState;

        [Header("Camera")]
        public GameObject fullCamera;
    }


    [Header("References")]
    public PlayerInput playerInput;
    public Animator animator;


    [Header("Neutral")]
    public string neutralState = "rig|MCNeutral";
    public GameObject neutralCamera;


    [Header("Directional Poses")]
    public List<DirectionalPose> poses = new List<DirectionalPose>();


    [Header("Animation")]
    public TransitionStyle transitionStyle = TransitionStyle.StopMotion;

    public float transitionHoldTime = 0.1f;

    public float neutralHoldTime = 0.1f;

    public float tweenCrossFadeTime = 0.1f;


    [Header("Gamepad Input")]
    public float stickThreshold = 0.3f;
    public float directionHysteresis = 0.1f;


    private PoseDirection currentPose = PoseDirection.Neutral;
    private PoseDirection desiredPose = PoseDirection.Neutral;

    private bool isTransitioning = false;


    private void Awake()
    {
        if (playerInput == null)
        {
            playerInput = FindObjectOfType<PlayerInput>();
        }
    }


    private void Start()
    {
        PlayPose(neutralState);
        ShowCamera(neutralCamera);

        currentPose = PoseDirection.Neutral;
        desiredPose = PoseDirection.Neutral;
    }


    private void Update()
    {
        desiredPose = ReadDesiredPose();

        if (!isTransitioning &&
            desiredPose != currentPose)
        {
            StartCoroutine(TransitionCoroutine());
        }
    }

    // Reads the desired pose based on the current input method (keyboard or gamepad).
    private PoseDirection ReadDesiredPose()
    {
        if (playerInput == null)
        {
            return PoseDirection.Neutral;
        }
        if (playerInput.currentControlScheme == "Gamepad")
        {
            return ReadAnalogInput(playerInput.look);
        }
        return ReadKeyboardInput();
    }

    // Reads the desired pose based on keyboard input.
    private PoseDirection ReadKeyboardInput()
    {
        bool up = playerInput.movingForward;
        bool down = playerInput.movingBackward;
        bool left = playerInput.movingLeft;
        bool right = playerInput.movingRight;

        // No movement key held.
        if (!up && !down && !left && !right)
        {
            return PoseDirection.Neutral;
        }

        // Opposite keys cancel each other out.
        if (left && right)
        {
            left = false;
            right = false;
        }

        if (up && down)
        {
            up = false;
            down = false;
        }
        if (left)
            return PoseDirection.Left;

        if (right)
            return PoseDirection.Right;

        if (up)
            return PoseDirection.Up;

        if (down)
            return PoseDirection.Down;

        return PoseDirection.Neutral;
    }


    // Reads the desired pose based on analog stick input.
    private PoseDirection ReadAnalogInput(Vector2 input)
    {
        if (input.magnitude < stickThreshold)
        {
            return PoseDirection.Neutral;
        }

        float absX = Mathf.Abs(input.x);
        float absY = Mathf.Abs(input.y);
        bool horizontal;

        // Determine if the input is more horizontal or vertical, considering hysteresis.
        // Hysteresis is used to prevent flickering between horizontal and vertical poses when the stick is near diagonal directions.
        if (currentPose == PoseDirection.Left || currentPose == PoseDirection.Right)
        {
            horizontal = absX + directionHysteresis >= absY;
        }

        // If the current pose is Up or Down, we apply hysteresis in the opposite direction.
        else if (currentPose == PoseDirection.Up ||
                 currentPose == PoseDirection.Down)
        {
            horizontal =
                absX > absY + directionHysteresis;
        }

        else
        {
            horizontal = absX > absY;
        }


        if (horizontal)
        {
            if (input.x > 0f)
            {
                return PoseDirection.Right;
            }
            else
            {
                return PoseDirection.Left;
            }
        }

        if (input.y > 0f)
        {
            return PoseDirection.Up;
        }
        else
        {
            return PoseDirection.Down;
        }
    }

    // Coroutine that handles the transition between poses, including the neutral pose.
    // Full poses mean poses that are complete.
    // Transition poses mean the poses that are in between the neutral and full poses.
    private IEnumerator TransitionCoroutine()
    {
        isTransitioning = true;
        while (currentPose != desiredPose)
        {
            // Currently in a full pose.
            if (currentPose != PoseDirection.Neutral)
            {
                // The direction of the pose we are leaving from.
                PoseDirection fromPoseDirection = currentPose;
                DirectionalPose fromPose = GetPose(fromPoseDirection);
                if (fromPose == null)
                {
                    break;
                }

                // Full -> Transition
                PlayPose(fromPose.transitionState);
                yield return new WaitForSeconds(transitionHoldTime);
                // Player changed their mind and moved back toward the from pose.
                // Return to full without changing camera.
                if (desiredPose == fromPoseDirection)
                {
                    PlayPose(fromPose.fullState);
                    currentPose = fromPoseDirection;
                    break;
                }
                // Transition -> Neutral
                // The camera changes back to neutral when the pose changes to neutral.
                PlayPose(neutralState);
                ShowCamera(neutralCamera);
                currentPose = PoseDirection.Neutral;
                yield return new WaitForSeconds(neutralHoldTime);
            }

            // Currently in neutral pose.
            if (desiredPose == PoseDirection.Neutral)
            {
                break;
            }
            PoseDirection targetDirection = desiredPose;
            DirectionalPose targetPose = GetPose(targetDirection);
            if (targetPose == null)
            {
                break;
            }
            // Neutral -> Transition
            // Camera stays neutral.
            PlayPose(targetPose.transitionState);
            yield return new WaitForSeconds(transitionHoldTime);
            // Player changed direction while the transition frame was being shown.
            // Avoid showing the old Full pose or its camera.
            if (desiredPose != targetDirection)
            {
                PlayPose(neutralState);
                ShowCamera(neutralCamera);
                currentPose = PoseDirection.Neutral;
                yield return new WaitForSeconds(
                    neutralHoldTime
                );
                continue;
            }
            // Transition -> Full
            // Ca,=mera changes to the target pose's camera when the full pose is reached.
            PlayPose(targetPose.fullState);
            ShowCamera(targetPose.fullCamera);
            currentPose = targetDirection;
        }
        isTransitioning = false;
        if (desiredPose != currentPose)
        {
            // Player changed their mind while the transition was happening.
            // Start the transition again to reach the desired pose.
            StartCoroutine(TransitionCoroutine());
        }
    }

    // Plays the specified animation state, either as a stop-motion or tweened transition.
    private void PlayPose(string stateName)
    {
        if (string.IsNullOrEmpty(stateName))
        {
            return;
        }

        if (transitionStyle == TransitionStyle.StopMotion)
        {
            animator.Play(stateName, 0, 0f);
        }
        else
        {
            animator.CrossFadeInFixedTime(stateName, tweenCrossFadeTime, 0);
        }
    }

    // Shows the specified camera and hides all others, including the neutral camera.
    private void ShowCamera(GameObject targetCamera)
    {
        if (neutralCamera != null)
        {
            neutralCamera.SetActive(neutralCamera == targetCamera);
        }
        foreach (DirectionalPose pose in poses)
        {
            if (pose.fullCamera == null)
            {
                continue;
            }
            pose.fullCamera.SetActive(pose.fullCamera == targetCamera);
        }
    }

    // Retrieves the DirectionalPose corresponding to the specified PoseDirection.
    private DirectionalPose GetPose(PoseDirection direction)
    {
        foreach (DirectionalPose pose in poses)
        {
            if (pose.direction == direction)
            {
                return pose;
            }
        }
        return null;
    }
}