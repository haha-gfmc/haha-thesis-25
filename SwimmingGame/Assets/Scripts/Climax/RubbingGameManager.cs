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
    public ObiRope ropeC;
    public ObiRope ropeD;
    public ObiRope ropeE;
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
    public bool orgy;
    public float ropeMoveOnThreshold;
    private bool levelLoaded;

    private ObiParticleAttachment[] obiParticleAttachmentA;
    private ObiParticleAttachment[] obiParticleAttachmentB;
    private ObiParticleAttachment[] obiParticleAttachmentC;
    private ObiParticleAttachment[] obiParticleAttachmentD;
    private ObiParticleAttachment[] obiParticleAttachmentE;

    private void Start()
    {
        startCounting = false;
        obiParticleAttachmentA = ropeA.GetComponents<ObiParticleAttachment>();
        obiParticleAttachmentB = ropeB.GetComponents<ObiParticleAttachment>();
        if(orgy){
            obiParticleAttachmentC = ropeC.GetComponents<ObiParticleAttachment>();
            obiParticleAttachmentD = ropeD.GetComponents<ObiParticleAttachment>();
            obiParticleAttachmentE = ropeE.GetComponents<ObiParticleAttachment>();
        }

    }

    void Update()
    {
        // Calculate distances
        headToHeadDistance = Vector3.Distance(npcHead.position, playerHead.position);
        playerBodyVelocity = playerBody.velocity.magnitude;

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
            if(orgy){
                obiParticleAttachmentC[i].enabled = false;
                obiParticleAttachmentD[i].enabled = false;
                obiParticleAttachmentE[i].enabled = false;
            }
        }

    }

    public void MoveOn()
    {

    }
}
