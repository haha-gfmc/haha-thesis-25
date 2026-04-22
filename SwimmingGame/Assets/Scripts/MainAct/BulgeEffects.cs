using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulgeEffect : MonoBehaviour
{
    //public SpringController springController;
    public GameObject head;
    public float headBulgeFactor = 1.5f; 
    public float headBulgeSpeed = 1f; // head bulge lerp speed
    public float headShrinkSpeed = 1f; // head shrink lerp speed
    public float bulgeThickness = 0.06f; 
    public float baseThickness = 0.04f; 
    public float pulseSpeed = 1f; // Speed at which the pulse travels
    public float pulseLength = 0.5f; // Length of the pulse 
    public float startScaleMargin; // higher to make the pulse start faster after head bulging
    public float shrinkThreshold; // higher to make the shrink start faster

    public bool triggerPulse; 

    private ObiRope rope;
    private float pulseTime = -1f; 
    private Vector3 headOriginalScale; 
    [SerializeField]
    private bool isHeadBulgedUp = false; 
    [SerializeField]
    private bool isFinishedBulging = false; 
    [SerializeField]
    private bool isPulseStarted = false; 
    [SerializeField]
    private bool isBulgingLocked = false;

    void OnEnable()
    {
        rope = GetComponent<ObiRope>();
        rope.OnSimulationStart += Rope_OnBeginStep;
    }

    void OnDisable()
    {
        rope.OnSimulationStart -= Rope_OnBeginStep;
    }

    void Start()
    {
        //springController = springController != null ? springController : FindObjectOfType<SpringController>();

        if (head != null)
            headOriginalScale = head.transform.localScale; // Store the original scale of the head
    }

    void Update()
    {
        //Debug.Log(pulseTime);

        // Trigger new pulse
        if (triggerPulse && !isPulseStarted)
        {
            Debug.Log("Set");
            isPulseStarted = true;
            triggerPulse = false; 
        }

        // Reset and trigger a new pulse when a pulse is going
        if (triggerPulse && isPulseStarted)
        {
            Reset();
            isPulseStarted = true;
            triggerPulse = false;
        }


        if (isPulseStarted)
        {
            // bulge the head first
            if (!isFinishedBulging && !isBulgingLocked)
            {
                //Debug.Log("Bulging");
                head.transform.localScale = Vector3.Lerp(head.transform.localScale,headOriginalScale * headBulgeFactor,Time.deltaTime * headBulgeSpeed);
                //Debug.Log((Vector3.Distance(head.transform.localScale, headOriginalScale * headBulgeFactor)));
                // when the head reaches the threshold to start the pulse
                if (Vector3.Distance(head.transform.localScale, headOriginalScale * headBulgeFactor) < startScaleMargin)
                {
                    isHeadBulgedUp = true;
                }
                // finish bulging and start shrinking
                if (Vector3.Distance(head.transform.localScale, headOriginalScale * headBulgeFactor) < shrinkThreshold)
                {
                    isFinishedBulging = true;
                }
            }


            // start the pulse
            if (isHeadBulgedUp && pulseTime < 0)
            {
                pulseTime = 1f; 
            }

            if (pulseTime > 0)
            {
                // Decrease pulse position over time
                pulseTime -= Time.deltaTime * pulseSpeed;
            }


            // Stop the pulse when it moves past the beginning of the rope
            if (pulseTime <= 0f && isHeadBulgedUp)
            {
                Reset();
            }
        }

        // handle the shrinking process of the rope
        if (isFinishedBulging)
        {
            isBulgingLocked = true;

            // Shrink the head back to its original size 
            head.transform.localScale = Vector3.Lerp(head.transform.localScale, headOriginalScale, Time.deltaTime * headShrinkSpeed);
            if (Vector3.Distance(head.transform.localScale, headOriginalScale) < 1f)
            {
                isFinishedBulging = false;
            }
        }
    }

    private void Rope_OnBeginStep(ObiActor actor, float stepTime, float substepTime)
    {
        if (pulseTime < 0) return; // Skip if no active pulse

        for (int i = 0; i < rope.elements.Count; i++)
        {
            var element = rope.elements[i];
            int particleIndex = element.particle2;

            // Calculate the distance of this particle from the pulse center
            float distance = Mathf.Abs((float)i / rope.elements.Count - pulseTime);

            // If within the pulse length, adjust thickness
            if (distance <= pulseLength)
            {
                float t = 1f - (distance / pulseLength); // Normalize thickness based on distance
                float thickness = Mathf.Lerp(baseThickness, bulgeThickness, t);
                rope.solver.principalRadii[particleIndex] = Vector3.one * thickness;
            }
            else
            {
                // Reset particles outside the pulse to base thickness
                rope.solver.principalRadii[particleIndex] = Vector3.one * baseThickness;
            }
        }
    }

    // Reset the pulseTime and all the flags
    private void Reset()
    {
        pulseTime = -1f; // Deactivate the pulse

        isPulseStarted = false; // Reset the pulse trigger
        isHeadBulgedUp = false;
        isFinishedBulging = false;
        isBulgingLocked = false;
    }

    public bool IsBulging(){
        return isPulseStarted;
    }
}