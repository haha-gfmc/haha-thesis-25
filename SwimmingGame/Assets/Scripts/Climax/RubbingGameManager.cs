using Obi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RubbingGameManager : MonoBehaviour
{
    public RopeMeanDistance ropeMeanDistance;
    public TMP_Text meterText;
    public LevelLoader levelLoader;
    public ObiRope ropeA;
    public ObiRope ropeB;
    public GameObject MCClimaxhead;
    public ClimaxCameraManager climaxCameraManager;

    [Header("Threshold Settings")]
    public float maxThreshold = 2.0f; // Maximum threshold where grow speed is at max
    public float minThreshold = 0.5f; // Minimum threshold where growth starts

    [Header("Grow Speed Settings")]
    public float minGrowSpeed = 0.1f; // Growth speed at maxThreshold
    public float maxGrowSpeed = 1.0f; // Growth speed at minThreshold
    public float decaySpeed = 0.5f;   // Decay speed when outside thresholds

    public float meanDistance;
    public float meterValue = 0f;
    private bool startCounting;

    [Tooltip("Distance between player organ head and npc head.")]
    public float headToHeadDistance;

    public Transform playerHead;
    public Transform npcHead;

    public float playerBodyVelocity;
    public Rigidbody playerBody;

    public bool moveOnAfterThresholdReached = false;
    public float ropeMoveOnThreshold;
    private bool levelLoaded;

    private ObiParticleAttachment[] obiParticleAttachmentA;
    private ObiParticleAttachment[] obiParticleAttachmentB;


    [Header("Animators")]

    public string inhalingParameter = "Inhaling";
    public string exhalingParameter = "Exhaling";
    public string blendParameter = "Blend";

    public float movementInputThreshold = 0.05f;
    public float exhaleDuration = 0.35f;
    public float blendLerpSpeed = 8f;
    public List<Animator> playerAnimators = new List<Animator>();

    public List<Animator> npcAnimators = new List<Animator>();

    public Rigidbody npcBody;
    public float npcBodyVelocity;

    private bool playerWasMovingInput;
    private bool npcWasMovingInput;

    private float playerExhaleTimer;
    private float npcExhaleTimer;

    private void Start()
    {
        startCounting = false;
        obiParticleAttachmentA = ropeA.GetComponents<ObiParticleAttachment>();
        obiParticleAttachmentB = ropeB.GetComponents<ObiParticleAttachment>();

    }

    void Update()
    {
        // Calculate distances
        headToHeadDistance = Vector3.Distance(npcHead.position, playerHead.position);
        playerBodyVelocity = playerBody != null ? playerBody.velocity.magnitude : 0f;
        npcBodyVelocity = npcBody != null ? npcBody.velocity.magnitude : 0f;

        HandleBreathingAnimation();

        // Get the mean distance from ropeMeanDistance
        meanDistance = GetMeanDistance();

        // Start counting when the mean distance falls below minThreshold
        if (meanDistance < minThreshold && !startCounting)
        {
            startCounting = true;
        }

        if (startCounting)
        {
            if (meanDistance <= minThreshold && meanDistance >= maxThreshold)
            {
                // Interpolate grow speed inversely based on meanDistance
                float normalizedDistance = (meanDistance - maxThreshold) / (minThreshold - maxThreshold);
                float growSpeed = Mathf.Lerp(minGrowSpeed, maxGrowSpeed, normalizedDistance);

                meterValue = Mathf.Min(meterValue + growSpeed * Time.deltaTime, 100f);
            }
            else if (meanDistance > minThreshold)
            {
                meterValue = Mathf.Max(meterValue - decaySpeed * Time.deltaTime, 0f);
            }
        }
        if (meterValue == 100f && moveOnAfterThresholdReached)
        {
            Detach();
            if (!levelLoaded)
            {
                levelLoaded = true;
                levelLoader.LoadLevel();
                Sound.StopInstance("Underwater Ambiance", true);
                Sound.PlayOneShotVolume("event:/Non-Diagetic SFX/Climax End",1f);
            }
            climaxCameraManager.isClimaxCompleted = true;
        }

        float rumbleModifier=1f;

        if(levelLoaded){
            rumbleModifier=Mathf.Pow((levelLoader.transitionTime-levelLoader.transitionTimer)/levelLoader.transitionTime,.25f);
        }

        Rumble.AddRumble("Base Climax",rumbleModifier);
        Rumble.AddRumble("Climax Intensity",rumbleModifier*Mathf.Clamp(meterValue/100f,0f,1f));

        // Update the meter text
        //meterText.text = $"Meter: {meterValue:F2}";

        if (MCClimaxhead.transform.position.y < ropeMoveOnThreshold)
        {
            MoveOn();
        }
    }

    public float GetMeanDistance()
    {
        float distance = ropeMeanDistance.meanDistance;
        return distance == 0f ? 10f : distance;
    }

    public void Detach()
    {
        for (int i = 0; i < obiParticleAttachmentA.Count(); i++)
        {
            obiParticleAttachmentA[i].enabled = false;
            obiParticleAttachmentB[i].enabled = false;
        }

    }

    public void MoveOn()
    {

    }

    private void HandleBreathingAnimation()
    {
        HandleBreathing(
            playerBodyVelocity,
            playerAnimators,
            ref playerWasMovingInput,
            ref playerExhaleTimer
        );

        HandleBreathing(
            npcBodyVelocity,
            npcAnimators,
            ref npcWasMovingInput,
            ref npcExhaleTimer
        );
    }

private void HandleBreathing(
    float velocity,
    List<Animator> targetAnimators,
    ref bool wasMovingInput,
    ref float exhaleTimer
)
{
    bool hasMovementInput = velocity > movementInputThreshold;

    if (hasMovementInput)
    {
        exhaleTimer = 0f;
        SetBreathingState(targetAnimators, true, false, 1f);
    }
    else
    {
        if (wasMovingInput)
        {
            exhaleTimer = exhaleDuration;
        }

        if (exhaleTimer > 0f)
        {
            exhaleTimer -= Time.deltaTime;
            SetBreathingState(targetAnimators, false, true, 0.5f);
        }
        else
        {
            SetBreathingState(targetAnimators, false, false, 0f);
        }
    }

    wasMovingInput = hasMovementInput;
}

    private void SetBreathingState(
        List<Animator> targetAnimators,
        bool inhaling,
        bool exhaling,
        float targetBlend
    )
    {
        foreach (Animator animator in targetAnimators)
        {
            if (animator == null) continue;

            animator.SetBool(inhalingParameter, inhaling);
            animator.SetBool(exhalingParameter, exhaling);

            float currentBlend = animator.GetFloat(blendParameter);
            float newBlend = Mathf.Lerp(
                currentBlend,
                targetBlend,
                blendLerpSpeed * Time.deltaTime
            );

            animator.SetFloat(blendParameter, newBlend);
        }
    }
}
