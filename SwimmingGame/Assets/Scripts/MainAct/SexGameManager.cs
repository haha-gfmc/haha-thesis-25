using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FMOD.Studio;
using FMODUnity;

public class SexGameManager : MonoBehaviour
{
    public RopeMeanDistance ropeMeanDistance;
    public TMP_Text meterText;
    public LevelLoader levelLoader;

    [Header("Threshold Settings")]
    public float maxThreshold = 2.0f; // Maximum threshold where grow speed is at max
    public float minThreshold = 0.5f; // Minimum threshold where growth starts

    [Header("Grow Speed Settings")]
    public float minGrowSpeed = 0.1f; // Growth speed at maxThreshold
    public float maxGrowSpeed = 1.0f; // Growth speed at minThreshold
    public float decaySpeed = 0.5f;   // Decay speed when outside thresholds

    private float meterValue = 0f;
    private bool startCounting;

    [Tooltip("Distance between player organ head and npc head.")]
    public float headToHeadDistance;

    public Transform playerHead;
    public Transform npcHead;

    public float playerBodyVelocity;
    public Rigidbody playerBody;

    public bool moveOnAfterThresholdReached = false;

    private bool movingOn=false;

    //[Header("Sound")]

    private EventInstance ambianceEvent;
    private NPCSpring[] npcSprings;
    private BulgeEffect[] bulgeEffects;


    private void Start()
    {
        startCounting = false;

        ambianceEvent=FindObjectOfType<StudioEventEmitter>().EventInstance;
        npcSprings=FindObjectsOfType<NPCSpring>();
        bulgeEffects=FindObjectsOfType<BulgeEffect>();
    }

    void Update()
    {
        // Calculate distances
        headToHeadDistance = Vector3.Distance(npcHead.position, playerHead.position);
        playerBodyVelocity = playerBody.linearVelocity.magnitude;

        // Get the mean distance from ropeMeanDistance
        float meanDistance = GetMeanDistance();

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

        // Load the next level when the meter reaches max value
        if (meterValue == 100f && moveOnAfterThresholdReached)
        {
            MoveOn();
        }

        // Update the meter text
        //meterText.text = $"Meter: {meterValue:F2}";

        // Sound stuff
        float averageIntensity=0f;
        foreach(NPCSpring npcSpring in npcSprings){
            averageIntensity+=npcSpring.currentIntensity;
        }
        averageIntensity=averageIntensity/npcSprings.Length;
        ambianceEvent.setParameterByName("intensity",Mathf.Round(averageIntensity));

        bool isBulging=false;
        foreach(BulgeEffect bulgeEffect in bulgeEffects){
            isBulging=bulgeEffect.IsBulging();
            if(isBulging) break;
        }
        if(isBulging) ambianceEvent.setParameterByName("bulging",1f);
        else ambianceEvent.setParameterByName("bulging",0f);

    }

    public float GetMeanDistance()
    {
        float distance = ropeMeanDistance.meanDistance;
        return distance == 0f ? 10f : distance;
    }

    public void MoveOn()
    {
        if(!movingOn){
            levelLoader.LoadLevel();
            movingOn=true;
        }
    }
}
