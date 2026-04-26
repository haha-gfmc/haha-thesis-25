using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpring : SexSpring
{
    [Header("NPC Movement Parameters")]

    public float timeBetweenBreaths=1f;
    [Tooltip("Min distance before stopping to move")]
    public float minDistanceFromPlayer=1f;
    [Tooltip("Max distance before stopping to move")]
    public float maxDistanceFromPlayer=5f;
    [Tooltip("When following player, go towards player + this distance to not run into head.")]
    public float playerOrbitingDistance=3f;
    private Vector3 targetOffset;

    [Tooltip("Time to inhale for.")]
    public float inhaleLength=1f;

    [Tooltip("Distance away from NPC to pick new target location")]
    public float targetDistance=5f;


    private Transform player;
    private Vector3 targetLocation;

    private MovementBehavior prevMovementBehavior;
    private int prevIntensity;

    private float turningTimer; //timer for turning. when more than turningTime, turn
    [Tooltip("Time before turning, applicable if look at player or looking around")]
    public float turningTime=1f;
    [Tooltip("Max distance body is allowed to move away from origin.")]
    public float maxDistanceFromOrigin=20f;

    private bool foundTarget=false;

    [Tooltip("Max time to modify values like time between breaths each cycle for randomness")]
    public float timeVariance=1f;
    private float currentTimeBetweenBreaths;
    private float currentTurningTime;

    [Tooltip("How many follow to run cycles to run when in FollowThenRun mode. For example if 2, NPC follows player twice then runs once")]
    public float followToRunRatio=1f;
    private int counter=0; //using this for followToRun
    private bool following=false; //only used for FollowToRun
    [Header("Debug Values")]
    public MovementBehavior movementBehavior;
    [Tooltip("Intensity determines movement speed and frequency etc.")]
    public int currentIntensity=0;

    [Header("Misc.")]
    public CapsuleCollider capsule;

    public Transform origin;
    [Tooltip("New parameters for each state/intensity.")]
    public SpringMovementValues[] springMovementValues;
    public Transform currentTarget;

    [Header("Animation")]
    public Animator organAnimator;
    private bool justExhaled;




    void Start()
    {
        player=GameObject.FindWithTag("Player").transform;
        SpringStart();

        GetMovementValues(movementBehavior);

        targetOffset=new Vector3();
    }

    void Update()
    {
        SpringUpdate();
    }

    void FixedUpdate()
    {
        //HandleTurning(playerInput.look);
        if(prevMovementBehavior!=movementBehavior || prevIntensity!=currentIntensity){
            GetMovementValues(movementBehavior);
        }
        Swim();

        UpdateAnimator();

        SpringFixedUpdate();

        if (!justExhaled)
        {
            justExhaled = isExhaling;
        }

        prevMovementBehavior=movementBehavior;
        prevIntensity=currentIntensity;
    }

    //Get movement parameters depending on current behavior type and intensity
    void GetMovementValues(MovementBehavior behavior){
        bool foundValues=false;
        foreach(SpringMovementValues values in springMovementValues){
            if(values.movementBehavior==behavior && currentIntensity==values.intensity){
                CopyValues(values);
                foundValues=true;
                break;
            }
        }
        if(!foundValues){
            int intensityFound=-1;
            SpringMovementValues valuesFound=new SpringMovementValues();
            bool valuesWereFound=false;
            foreach(SpringMovementValues values in springMovementValues){
                if(values.movementBehavior==behavior){
                    if(intensityFound==-1){
                        valuesFound=values;
                        intensityFound=values.intensity;
                    }else{
                        if(Mathf.Abs(currentIntensity-values.intensity)<Mathf.Abs(currentIntensity-intensityFound)){
                            valuesFound=values;
                            intensityFound=values.intensity;
                        }
                    }    
                    valuesWereFound=true;
                }
            }
            if(valuesWereFound){
                CopyValues(valuesFound);
            }
        }
    }

    void CopyValues(SpringMovementValues values){
        timeBetweenBreaths=values.timeBetweenBreaths;
        minDistanceFromPlayer=values.minDistanceFromPlayer;
        maxDistanceFromPlayer=values.maxDistanceFromPlayer;
        inhaleLength=values.inhaleLength;
        turnSpeed=values.turnSpeed;
        if (values.accelerationMultiplier != 0f)
        {
            accelerationMultiplier=values.accelerationMultiplier;
        }
        else
        {
            accelerationMultiplier=1f;
        }
        TimeVariance();
    }

    public void ChangeMovementBehavior(MovementBehavior newBehavior){
        movementBehavior=newBehavior;
        counter=0;
        if(movementBehavior==MovementBehavior.FollowThenRun || movementBehavior==MovementBehavior.FollowPlayer){
            following=true;
        }
    }

    public void ChangeIntensity(int changeInIntensity){
        currentIntensity+=changeInIntensity;
        currentIntensity=Mathf.Max(currentIntensity,0);
    }

    //Randomize timer durations such as turning time and time between breaths
    void TimeVariance(){
        if(turningTime>0){
            currentTurningTime=turningTime+Random.Range(-timeVariance,timeVariance);
            currentTurningTime=Mathf.Max(Mathf.Min(turningTime/2,0.5f),currentTurningTime);
        }
        if(timeBetweenBreaths>0){
            currentTimeBetweenBreaths=timeBetweenBreaths+Random.Range(-timeVariance,timeVariance);
            currentTimeBetweenBreaths=Mathf.Max(Mathf.Min(turningTime/2,0.5f),currentTimeBetweenBreaths);
        }
    }

    //Handle movement depending on current movement behavior
    void Swim(){
        float inhaleTime=Time.time-inhaleStartTime;
        float distanceFromPlayer=Vector3.Distance(player.transform.position,characterRb.position);

        if(prevMovementBehavior!=movementBehavior){
            turningTimer=turningTime;
        }

        switch(movementBehavior){
            case MovementBehavior.FollowPath:
                FollowPath();   //CURRENTLY DOES NOTHING
                break;
            case MovementBehavior.FollowTarget:
                FollowTarget(currentTarget);
                break;
            case MovementBehavior.FollowPlayer:
                FollowPlayer();
                break;
            case MovementBehavior.RunFromPlayer:
                RunFromPlayer();
                break;
            case MovementBehavior.FollowThenRun:
                if(timeSinceExhale>=currentTimeBetweenBreaths){
                    counter+=1;
                    if((following && counter>followToRunRatio) || (!following && counter>1/followToRunRatio)){
                        following=!following;
                    }
                    if(following) GetMovementValues(MovementBehavior.FollowPlayer);
                    else GetMovementValues(MovementBehavior.RunFromPlayer);
                }
                if(following){
                    FollowPlayer();
                }else{
                    RunFromPlayer();
                }
                
                break;
            case MovementBehavior.LookAtPlayer:
                if(turningTimer>=currentTurningTime){
                    targetLocation=player.position;
                    turningTimer=0;
                    TimeVariance();
                }
                TurnTowards(targetLocation,turnSpeed);
                break;
            case MovementBehavior.Wander:
                Wander();
                break;
            case MovementBehavior.LookAround:
                turningTimer+=Time.fixedDeltaTime;
                if(turningTimer>=currentTurningTime){
                    targetLocation=characterRb.position+
                        new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f))
                        .normalized*targetDistance;
                    turningTimer=0;
                    TimeVariance();
                }
                TurnTowards(targetLocation,turnSpeed);
                break;

        }
    }

    void FollowPath(){

    }

    //SHOULD CHANGE DISTANCEFROMPLAYER TO DISTANCETOTARGET
    void FollowTarget(Transform target){    
        float inhaleTime=Time.time-inhaleStartTime;
        float distanceFromPlayer=Vector3.Distance(target.position,characterRb.position);
        targetLocation=target.position+targetOffset;
        if(isInhaling || (!isExhaling && timeSinceExhale>=currentTimeBetweenBreaths/2f)){
            TurnTowards(targetLocation,turnSpeed);
        }
        if(!isInhaling && !isExhaling){
            if(timeSinceExhale>=currentTimeBetweenBreaths && distanceFromPlayer>=minDistanceFromPlayer){
                bool blockedPath=CheckRayCast(target.position-characterRb.position,distanceFromPlayer);
                if(!blockedPath){
                    timeSinceExhale=0f;
                    StartInhaling();
                    TimeVariance();
                }
            }
        }else if(isInhaling){
            if(inhaleTime>=inhaleLength){
                StartExhaling();
                // Add random offset when following player so we don't ram straight into the head
                if (movementBehavior == MovementBehavior.FollowPlayer)
                {
                    targetOffset=new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f)).normalized*playerOrbitingDistance;
                }
                else
                {
                    targetOffset=Vector3.zero;
                }
            }
        }
    }

    void FollowPlayer(){
        FollowTarget(player.transform);
    }

    void RunFromPlayer(){
        float inhaleTime=Time.time-inhaleStartTime;
        float distanceFromPlayer=Vector3.Distance(player.transform.position,characterRb.position);
        targetLocation=characterRb.position+(characterRb.position-player.position);
        if(!isExhaling){
            TurnTowards(targetLocation,turnSpeed);
        }
        if(!isInhaling && !isExhaling){
            if(timeSinceExhale>=currentTimeBetweenBreaths && distanceFromPlayer<=maxDistanceFromPlayer){
                bool blockedPath=CheckRayCast(characterRb.position-player.position,targetDistance);
                if(!blockedPath){
                    timeSinceExhale=0f;
                    StartInhaling();
                    TimeVariance();
                }
            }
        }else if(isInhaling){
            if(inhaleTime>=inhaleLength){
                StartExhaling();
            }
        }
    }

    void Wander(){
        float inhaleTime=Time.time-inhaleStartTime;
        float distanceFromPlayer=Vector3.Distance(player.transform.position,characterRb.position);
        turningTimer+=Time.fixedDeltaTime;
        if(turningTimer<=currentTurningTime){
            TurnTowards(targetRotation,turnSpeed);
        }
        if(!isInhaling && !isExhaling){
            if(timeSinceExhale>=currentTimeBetweenBreaths && distanceFromPlayer>=minDistanceFromPlayer){
                if(!foundTarget){
                    //Find target location
                    Vector3 target=new Vector3();
                    int tries=0;
                    while(!foundTarget && tries<100){
                        tries++;
                        Vector3 displacement=new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f))
                            .normalized*targetDistance;
                        target=characterRb.position+displacement;
                        float distance=Vector3.Distance(origin.position,target);
                        if(distance>maxDistanceFromOrigin){
                            foundTarget=false;
                            continue;
                        }
                        foundTarget=!CheckRayCast(target-characterRb.position,targetDistance);
                    }
                    if(!foundTarget){
                        target=origin.position;
                        foundTarget=true;
                    }
                    if(foundTarget){
                        targetLocation=target;
                        targetRotation=Quaternion.LookRotation(targetLocation-characterRb.transform.position,Vector3.up);
                    }
                }
                if(foundTarget){
                    timeSinceExhale=0f;
                    StartInhaling();
                    foundTarget=false;
                    TimeVariance();
                    turningTimer=0f;
                }
            }
        }else if(isInhaling){
            if(inhaleTime>=inhaleLength){
                StartExhaling();
            }
        }
    }

    //True if colliding False if not
    bool CheckRayCast(Vector3 direction, float distance){
        bool colliding=false;
        Vector3 pos=characterRb.position;
        Vector3 point1=pos+capsule.center+capsule.transform.forward*(capsule.height/2f-capsule.radius);
        Vector3 point2=pos+capsule.center-capsule.transform.forward*(capsule.height/2f-capsule.radius);
        RaycastHit[] hits;
        LayerMask mask = LayerMask.GetMask("Default","Wall");
        hits=Physics.CapsuleCastAll(point1,point2,capsule.radius,direction,targetDistance,mask,QueryTriggerInteraction.Ignore);
        foreach(RaycastHit hit in hits){
            if(hit.collider!=capsule){
                colliding=true;
                break;
            }
        }
        
        return colliding;
    }
    void UpdateAnimator()
    {
        if (organAnimator == null) return;

        organAnimator.SetBool("Inhaling", isInhaling);
        organAnimator.SetBool("Exhaling", isExhaling);

        if (isInhaling && justExhaled)
        {
            organAnimator.SetFloat("Blend", Random.Range(0f, 1f));
            justExhaled = false;
        }
    }

}

[System.Serializable]
public struct SpringMovementValues{
    public MovementBehavior movementBehavior;
    public int intensity;
    public float timeBetweenBreaths;
    [Tooltip("Min distance before stopping to move")]
    public float minDistanceFromPlayer;
    [Tooltip("Max distance before stopping to move")]
    public float maxDistanceFromPlayer;
    [Tooltip("Time to inhale for.")]
    public float inhaleLength;
    public float turnSpeed;
    [Tooltip("Time before turning, applicable if look at player or looking around")]
    public float turningTime;
    [Tooltip("Multiply acceleration by this much.")]
    public float accelerationMultiplier;

}

/*TO DO:
+ more complex behavior: run then turn, run then follow, etc.
+ distance buffer where if close or far ish it breathes for shorter amount of time
+ organize code/public values
*/ 