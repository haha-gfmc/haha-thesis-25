using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Timeline;

public class Swimmer : MonoBehaviour
{
    [Header("Movement")]
    public bool canMove=true;
    public bool canKickBack=true;
    public float acceleration=1f;
    public float backwardAcceleration=1f;
    [Tooltip("If at top speed, multiply backward acceleration by this factor")]
    public float backwardAccelerationFactor=3f;
    public float maxVelocity=10f;
    [Tooltip("Deceleration always takes effect")]
    public float deceleration=5f;
    public float lateralAcceleration=1f;
    public float lateralMaxVelocity=5f;
    [Tooltip("Dash when moving laterally (up/down/left/right with D-Pad)")]
    public float dashSpeed=1f;
    [Tooltip("Can't dash when moving faster than this in dash direction.")]
    public float maxDashSpeed=5f;
    public float dashCooldownTime=0.3f;
    [Tooltip("Register input and play it next if within this window of cooldown.")]
    public float dashCooldownWindow=0.15f;
    [Tooltip("How many dashes can be charged")]
    public int maxDashes=3;
    private float dashTimer=1f; //time since last dash
    private Directions prevDashDirection;
    private Directions savedDashDirection; //Save input if clicking during cooldown
    private Directions dashDirection=Directions.NULL;

    [Tooltip("Instant speed gain at the end of a stroke")]
    public float boostSpeed=1f;
    [Tooltip("Swimmer maintains coasting speed when not boosting")]
    public float coastingSpeed=3f;  //maintains coasting speed when not boosting

    [Tooltip("Timer for before boost takes effect")]
    public float boostTimer=0f;
    [Tooltip("Total time it takes before boost takes effect")]
    public float boostTime=1f;     //time before boost takes effect
    [Tooltip("Ignore new strides during this time right after last input.")]
    private float timerSinceMoveForwardInput=0f;
    public float ignoreStrideTime=.3f;
    [Tooltip("Max speed gained from kicking wall")]
    public float wallBoost=1.5f;
    [Tooltip("Number to multiply the capsule radius when checking for walls")]
    public float wallCheckCapsuleRadiusMultiplier=1.5f;

    private CharacterController controller;
    private Rigidbody body;
    private PlayerInput playerInput;
    private SwimmerCamera swimmerCamera;

    [Header("Rotation")]
    public bool canRotate=true;
    public float rotationAcceleration=180f;
    [Tooltip("If at top speed, multiply rotation acceleration by this factor")]
    public float rotationVelocityFactor=2f;
    [Tooltip("If coasting, multiply rotation acceleration by this much (to make it slow.)")]
    public float rotationCoastingVelocityFactor=.7f;
    public float rotationMaxVelocity=40f;
    [Tooltip("If coasting, this is max rotation velocity")]
    public float rotationCoastingMaxVelocity=45f;
    private Vector3 rotationVelocity=Vector3.zero;
    public float rotationDeceleration=180f;
    public float maxRotationXAngle=60f;

    public float maxTiltAngle=10f; //max angle to tilt when moving laterally
    public float angleTiltSpeed=1f; //speed to tile when moving laterally

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Quaternion targetRotationOverride;
    private bool overridingRotation=false;
    [Tooltip("Rotation speed when a rotation is forced on the player, like after a kick back")]
    public float rotationOverrideSpeed=1f;
    private float tempRotationOverrideSpeed=-1f;
    [Tooltip("Speed at which to restore rotation back to clamped value after kicking back")]
    public float rotationOverrideRestoreSpeed=1f;

    [Header("Collision")]
    [Tooltip("Factor to multiply anti-collision vector to mess with bounce. Usually 1.")]
    public float collisionSpeedFactor=1f;
    [Tooltip("Min reaction speed when reacting with anything even if the relative velocity is 0.")]
    public float minCollisionSpeed=0.01f;
    [Tooltip("Min distance to move player out of object it's clipping into.")]
    public float minMoveoutStep=0.001f;
    [Tooltip("How many maximum moveout steps can the player perform every fixedupdate.")]
    public int maxMoveoutEffort=100;
    [Tooltip("Distance to keep object away from any collider.")]
    public float skinWidth=0.05f;
    [Tooltip("Min angle to rotate player when colliding with something.")]
    public float minCollisionRotationAmount=0.1f;
    [Tooltip("Max angle to rotate player when colliding with something.")]
    public float maxCollisionRotationAmount=1f;
    [Tooltip("Minimum force of collision to make player rotate.")]
    public float minCollisionForceToRotate=0.01f;
    [Tooltip("Cap force of collision to make player rotate.")]
    public float maxCollisionForceToRotate=5f;

    [Header("Misc.")]
    public GameObject dustCloudPrefab;
    public GameObject strideEffectPrefab;
    private GameObject strideEffect;
    [Tooltip("Past this speed do not make stride effect.")]
    public float strideMaxSpeed=5f;
    [HideInInspector]
    public bool strideTrigger=false; //called by animator when the stride begins to trigger stride effect
    private float timerSinceLastStrideEffect=0f;
    private bool strideEffectCharged=false;
    public GameObject afterimageSprite;
    private Vector3 prevVelocity;

    ArrayList allHits=new ArrayList();

    private CapsuleCollider capsule;


    private Vector3 forcesToAdd; //Forces to add at the beginning of the next frame; this is used for e.g. for ring boosts

    [Tooltip("Maximum time before the player lets go of the moving backwards button before a camera adjustment is triggered.")]
    public float maxKickbackPressingTime=0.3f;
    private float pressedBackTimer=10f; //Timer to check how long it's been since we kicked back against a wall for the purpose of rotating camera
    private float prevPressedBackTimer=0f; //For the previous pressed back input to check double presses to turn around
    private bool justKickedBack=false;


    private SwimmerSound swimmerSound;

    public Transform respawnTransform;

    private SwimmerTrails swimmerTrails;

    [Tooltip("Speed to lerp the color of the sprite, when dashing for e.g.")]
    public float colorLerpSpeed=5f;
    [Tooltip("Value for color of sprite when cooling down for dash.")]
    [Range(0f,1f)]
    public float dashCooldownColorValue=2/3f;

    public bool sleeping=false;
    public bool organOut=false;
    public float timeBeforeWakingUp=10f;
    public float timeBeforeFallingAsleep=-1f;
    private float noInputTimer=0f;
    private Coroutine wakingCoroutine;

    private bool visible=true;


    private RespawnTrigger[] respawnTriggers;   //Gathering these for cycling through
    private int respawnTriggerIndex;

    void Start()
    {
        controller=GetComponent<CharacterController>();
        playerInput=FindObjectOfType<PlayerInput>();
        animator=GetComponent<Animator>();
        spriteRenderer=GetComponentInChildren<SpriteRenderer>();
        body=GetComponent<Rigidbody>();
        capsule=GetComponentInChildren<CapsuleCollider>();
        swimmerCamera=GetComponent<SwimmerCamera>();
        swimmerSound=GetComponent<SwimmerSound>();
        swimmerTrails=GetComponentInChildren<SwimmerTrails>();

        Cursor.lockState = CursorLockMode.Locked;  // Locks the cursor to the center of the screen

        animator.SetBool("organOut",organOut);

        if(sleeping){
            Sleep();
            wakingCoroutine=StartCoroutine(WakeUp(timeBeforeWakingUp));
            animator.SetTrigger("skipFallingAsleep");
        }

        respawnTriggers=FindObjectsOfType<RespawnTrigger>();
        respawnTriggerIndex=0;

    }

    void Update(){
        //Fall asleep if idle for too long
        if(playerInput.noInput && canMove){
            noInputTimer+=Time.deltaTime;
        }else{
            noInputTimer=0f;
        }

        if(timeBeforeFallingAsleep!=-1f && noInputTimer>timeBeforeFallingAsleep && !sleeping){
            organOut=Random.Range(0f,1f)>.8f;
            animator.SetBool("organOut",organOut);
            Sleep();
        }

        timerSinceMoveForwardInput+=Time.deltaTime;
        
        if(playerInput.movedForwardTrigger && canMove && timerSinceMoveForwardInput>=ignoreStrideTime){
            swimmerSound.Stride(GetVelocity().magnitude);
            boostTimer=0f;
            timerSinceMoveForwardInput=0f;
            animator.SetTrigger("boostForward");
            playerInput.movedForwardTrigger=false;
            forcesToAdd+=CheckForWallAndKick(Vector3.forward);
        }else if(playerInput.movingBackward && !playerInput.prevMovingBackward && canMove){
            Vector3 force=CheckForWallAndKick(-Vector3.forward);
            forcesToAdd+=force;
            if(force!=Vector3.zero){
                justKickedBack=true;
            }
            else
            {
                swimmerSound.SwimBackward();
            }
            pressedBackTimer=0f;
        }else if(playerInput.movedForwardTrigger && !canMove){
            playerInput.movedForwardTrigger=false;
            if(sleeping && wakingCoroutine==null){
                StartCoroutine(WakeUp(0f));
            }
        }
        
        // Dash input
        // We take the input if past the timer OR going in opposite direction, to allow cancel
        if(canMove){
            if(((playerInput.movingUp && !playerInput.prevMovingUp) || (savedDashDirection==Directions.UP)) && (dashTimer>dashCooldownTime || prevDashDirection==Directions.DOWN)){
                DashInput(Directions.UP);
            }else if(playerInput.movingUp && !playerInput.prevMovingUp && dashTimer-dashCooldownTime>=dashCooldownWindow){
                savedDashDirection=Directions.UP;
            }
            if(((playerInput.movingDown && !playerInput.prevMovingDown) || (savedDashDirection==Directions.DOWN)) && (dashTimer>dashCooldownTime || prevDashDirection==Directions.UP)){
                DashInput(Directions.DOWN);
            }else if(playerInput.movingDown && !playerInput.prevMovingDown && dashTimer-dashCooldownTime>=dashCooldownWindow){
                savedDashDirection=Directions.DOWN;
            }
            if(((playerInput.movingLeft && !playerInput.prevMovingLeft) || (savedDashDirection==Directions.LEFT)) && (dashTimer>dashCooldownTime || prevDashDirection==Directions.RIGHT)){
                DashInput(Directions.LEFT);
            }else if(playerInput.movingLeft && !playerInput.prevMovingLeft && dashTimer-dashCooldownTime>=dashCooldownWindow){
                savedDashDirection=Directions.LEFT;
            }
            if(((playerInput.movingRight && !playerInput.prevMovingRight) || (savedDashDirection==Directions.RIGHT)) && (dashTimer>dashCooldownTime || prevDashDirection==Directions.LEFT)){
                DashInput(Directions.RIGHT);
            }else if(playerInput.movingRight && !playerInput.prevMovingRight && dashTimer-dashCooldownTime>=dashCooldownWindow){
                savedDashDirection=Directions.RIGHT;
            }
        }
        dashTimer+=Time.deltaTime;

        if(dashTimer>dashCooldownTime*maxDashes){
            spriteRenderer.material.color=Color.Lerp(spriteRenderer.material.color,Color.white,Time.deltaTime*colorLerpSpeed);
        }else{
            float v=dashCooldownColorValue;
            v=v+(1f-v)*Mathf.Floor(dashTimer-dashCooldownTime)/maxDashes;
            spriteRenderer.material.color=Color.Lerp(spriteRenderer.material.color,new Color(v,v,v),Time.deltaTime*colorLerpSpeed);
        }
        

        //Initiate stride effect if striding
        if(strideEffectCharged && strideTrigger && timerSinceLastStrideEffect>=boostTime && GetVelocity().magnitude<=strideMaxSpeed){
            strideEffectCharged=false;
            strideEffect=Instantiate(strideEffectPrefab,spriteRenderer.transform);
            strideEffect.SetActive(true);
            strideEffect.GetComponent<Animator>().Update(animator.GetCurrentAnimatorStateInfo(0).normalizedTime*animator.GetCurrentAnimatorStateInfo(0).length);   
            timerSinceLastStrideEffect=0f;
        }else{
            timerSinceLastStrideEffect+=Time.deltaTime;            
        }
        strideTrigger=false;

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("idleSwim")) strideEffectCharged=true;



        // DEBUG HOTKEYS

        if(Input.GetKeyDown(KeyCode.R) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))){
            Respawn();
        }

        if(Input.GetKeyDown(KeyCode.H) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))){
            TurnInvisible();
        }

        if(respawnTriggers.Length>0){
            if(Input.GetKeyDown(KeyCode.LeftArrow) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))){
                respawnTriggerIndex=(respawnTriggerIndex-1+respawnTriggers.Length)%respawnTriggers.Length;
                respawnTransform=respawnTriggers[respawnTriggerIndex].transform;
                Respawn();
            }
            if(Input.GetKeyDown(KeyCode.RightArrow) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))){
                respawnTriggerIndex=(respawnTriggerIndex+1)%respawnTriggers.Length;
                respawnTransform=respawnTriggers[respawnTriggerIndex].transform;
                Respawn();
            }
        }


        if (canMove && (playerInput.movingForward || playerInput.movingBackward))
        {
            Rumble.AddRumble("Cruising");
        }
    }

    void DashInput(Directions d){
        if(dashSpeed>0f){
            dashDirection=d;
            dashTimer=Mathf.Clamp(dashTimer,dashCooldownTime,dashCooldownTime*maxDashes);
            dashTimer-=dashCooldownTime;
            savedDashDirection=Directions.NULL;
            Rumble.AddRumbleWithTimeOut("Dash",1f,.25f);
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move(){
        Vector3 currentVelocity=new Vector3(body.velocity.x,body.velocity.y,body.velocity.z);
        Vector3 playerVelocity=currentVelocity;

        //Overriding rotation if a kickback just happened
        if((pressedBackTimer<=maxKickbackPressingTime) && playerInput.look.magnitude<0.2f && !playerInput.movingBackward && !overridingRotation){
            if(justKickedBack && playerVelocity.magnitude!=0f) OverrideRotation(Quaternion.LookRotation(playerVelocity,Vector3.up));
            else if(canKickBack){
                if(prevPressedBackTimer<=maxKickbackPressingTime){
                    OverrideRotation(Quaternion.LookRotation(-transform.forward,transform.up));
                    prevPressedBackTimer=maxKickbackPressingTime;
                    swimmerSound.KickBack();
                    Rumble.AddRumbleWithTimeOut("Swimming Stroke",1f,.25f);
                }else{
                    prevPressedBackTimer=0f;
                    pressedBackTimer=maxKickbackPressingTime;
                }
            }
            justKickedBack=false;
        }else if(overridingRotation && (playerInput.look!=Vector2.zero || Quaternion.Angle(targetRotationOverride,transform.rotation)<=1f)){
            overridingRotation=false;
            float spd=rotationOverrideSpeed;
            if(tempRotationOverrideSpeed!=-1f) spd=tempRotationOverrideSpeed;
            rotationVelocity=(Quaternion.Slerp(body.rotation,targetRotationOverride,spd*Time.fixedDeltaTime).eulerAngles-body.rotation.eulerAngles)/Time.fixedDeltaTime;
            rotationVelocity.z=0f;
        }else if(pressedBackTimer<=maxKickbackPressingTime && overridingRotation){
            pressedBackTimer=maxKickbackPressingTime;
        }
        pressedBackTimer+=Time.fixedDeltaTime;
        prevPressedBackTimer+=Time.deltaTime;

        Quaternion newRotationQ;
        if(!overridingRotation){
            //Deceleration of rotation speed
            Vector3 antiRotationVector=rotationVelocity;
            Vector3 prevRotationVelocity=rotationVelocity;
            antiRotationVector=antiRotationVector.normalized*rotationDeceleration*Time.fixedDeltaTime;

            rotationVelocity=rotationVelocity-=antiRotationVector;

            Vector3 newRotation=body.rotation.eulerAngles;

            if(canRotate){
                //Cancel rotation deceleration if it goes past the direction
                if(playerInput.look==Vector2.zero){
                    if(rotationVelocity.x/Mathf.Abs(rotationVelocity.x)!=prevRotationVelocity.x/Mathf.Abs(prevRotationVelocity.x)){
                        rotationVelocity.x=0;
                    }
                    if(rotationVelocity.y/Mathf.Abs(rotationVelocity.y)!=prevRotationVelocity.y/Mathf.Abs(prevRotationVelocity.y)){
                        rotationVelocity.y=0;
                    }
                    if(rotationVelocity.z/Mathf.Abs(rotationVelocity.z)!=prevRotationVelocity.z/Mathf.Abs(prevRotationVelocity.z)){
                        rotationVelocity.z=0;
                    }
                }

                if(playerInput.look!=Vector2.zero){//Setting rotation speed
                    float acc=rotationAcceleration;
                    float f=Mathf.Clamp(playerVelocity.magnitude/maxVelocity,0f,1f);
                    acc=acc+acc*f*(rotationVelocityFactor-1f);
                    float rotMaxVelocity=rotationMaxVelocity;
                    if(IsCoasting() || IsSwimmingBackwards()){
                        acc=acc*rotationCoastingVelocityFactor;
                        rotMaxVelocity=rotationCoastingMaxVelocity;
                    }
                    rotationVelocity+=new Vector3(playerInput.look.y,playerInput.look.x,0f)*acc*Time.fixedDeltaTime;
                    if (pressedBackTimer >= 1f) rotationVelocity = Vector3.ClampMagnitude(rotationVelocity, rotMaxVelocity);
                    else rotationVelocity = Vector3.Lerp(rotationVelocity, Vector3.ClampMagnitude(rotationVelocity, rotMaxVelocity), Time.fixedDeltaTime * rotationOverrideRestoreSpeed);

                    if (playerInput.look.x > 0.1f)
                    {
                        spriteRenderer.flipX=true;
                    }else if (playerInput.look.x < -0.1f)
                    {
                        spriteRenderer.flipX=false;
                    }
                }

                //Finding rotation to do
                newRotation+=rotationVelocity*Time.fixedDeltaTime;
                //Clamping x rotation
                if(newRotation.x<180f){
                    newRotation.x=Mathf.Clamp(newRotation.x,-maxRotationXAngle,maxRotationXAngle);
                }else{
                    newRotation.x=Mathf.Clamp(newRotation.x,360f-maxRotationXAngle,360f+maxRotationXAngle);
                }

                //Tilt player if moving laterally
                float targetRotationZ=0f;
                if(playerInput.movingLeft && !playerInput.movingRight){
                    targetRotationZ=maxTiltAngle;
                    spriteRenderer.flipX=true;
                }else if(playerInput.movingRight && !playerInput.movingLeft){
                    targetRotationZ=-maxTiltAngle;
                    spriteRenderer.flipX=false;
                }
                if(newRotation.z>=180f){
                    targetRotationZ=360f+targetRotationZ;
                }
                newRotation.z=Mathf.Lerp(newRotation.z,targetRotationZ,Time.fixedDeltaTime*angleTiltSpeed);
            }
            newRotationQ=Quaternion.Euler(newRotation);
        }else{
            float spd=rotationOverrideSpeed;
            if(tempRotationOverrideSpeed!=-1f) spd=tempRotationOverrideSpeed;
            newRotationQ=Quaternion.Slerp(body.rotation,targetRotationOverride,spd*Time.fixedDeltaTime);
        }

        

        //Deceleration
        Vector3 decelerationVector=currentVelocity;
        decelerationVector=decelerationVector.normalized*deceleration*Time.fixedDeltaTime;

        playerVelocity=playerVelocity-=decelerationVector;

        //Cancel deceleration if it goes past the direction
        if(!playerInput.movingForward && !playerInput.movingBackward && !playerInput.movingLeft && !playerInput.movingRight && !playerInput.movingUp && !playerInput.movingUp){
            if(playerVelocity.x/Mathf.Abs(playerVelocity.x)!=currentVelocity.x/Mathf.Abs(currentVelocity.x)){
                playerVelocity.x=0;
            }
            if(playerVelocity.y/Mathf.Abs(playerVelocity.y)!=currentVelocity.y/Mathf.Abs(currentVelocity.y)){
                playerVelocity.y=0;
            }
            if(playerVelocity.z/Mathf.Abs(playerVelocity.z)!=currentVelocity.z/Mathf.Abs(currentVelocity.z)){
                playerVelocity.z=0;
            }
        }


        //Deal with collisions
        //The way this works: find normal of every collision, project the player's velocity on it, move the player by that much
        ArrayList collisionImpulses=new ArrayList();
        foreach (ContactPoint hit in allHits)
        {
            Vector3 collisionPoint=hit.point;
            Vector3 normal=hit.normal;
            Vector3 relativeVelocity;
            if(hit.otherCollider.TryGetComponent<Rigidbody>(out Rigidbody otherBody)){
                relativeVelocity=otherBody.velocity;
            }
            else{
                relativeVelocity=Vector3.zero;
            }
            relativeVelocity=relativeVelocity-playerVelocity;
            Vector3 force=normal*relativeVelocity.magnitude*collisionSpeedFactor;
            //Projection of relative velocity on normal
            force=Vector3.Project(relativeVelocity,normal)*collisionSpeedFactor;
            if(Vector3.Angle(force,normal)>=90){
                force=Vector3.zero;
            }
            if(force==Vector3.zero){
                force+=normal*minCollisionSpeed;
            }

            //Dividing force by number of contacts
            force=force/allHits.Count;

            collisionImpulses.Add(force);

            //playerVelocity+=force;
            float rotationAmount=minCollisionRotationAmount+(maxCollisionRotationAmount-minCollisionRotationAmount)*
            Mathf.Clamp((force.magnitude-minCollisionForceToRotate)/(maxCollisionForceToRotate-minCollisionForceToRotate),0f,1f);
            newRotationQ=Quaternion.RotateTowards(newRotationQ,Quaternion.LookRotation(force),rotationAmount);

            //Attempt at changing rotation speed instead of immediately swerving MAYBE TRY THIS AGAIN
            // Vector3 rv=Quaternion.FromToRotation(body.rotation.eulerAngles,force).eulerAngles;
            // rv=rv.normalized*rv.magnitude*rotationAmount;
            // rotationVelocity+=rv;
        }

        foreach(Vector3 force in collisionImpulses){
            playerVelocity+=force;
        }

        //Friction
        ArrayList frictionImpulses=new ArrayList();
        for(int i=0;i<allHits.Count;i++){
            ContactPoint hit=(ContactPoint)allHits[i];
            PhysicsObject physicsObject;
            if(hit.otherCollider.gameObject.TryGetComponent<PhysicsObject>(out physicsObject)){
                Vector3 collisionPoint=hit.point;
                Vector3 normal=hit.normal;
                Vector3 relativeVelocity;
                if(hit.otherCollider.TryGetComponent<Rigidbody>(out Rigidbody otherBody)){
                    relativeVelocity=otherBody.velocity;
                }
                else{
                    relativeVelocity=Vector3.zero;
                }
                relativeVelocity=relativeVelocity-playerVelocity;
                Vector3 force=relativeVelocity-Vector3.Project(relativeVelocity,normal)*collisionSpeedFactor;

                force=Vector3.ClampMagnitude(force,((Vector3)collisionImpulses[i]).magnitude*physicsObject.friction);

                frictionImpulses.Add(force);
            }
        }

        foreach(Vector3 force in frictionImpulses){
            playerVelocity+=force;
        }


        //Rotating player
        body.MoveRotation(newRotationQ);


        boostTimer+=Time.fixedDeltaTime;

        //Boosting player velocity at the end of the swimstroke
        if(canMove){
            if(boostTimer>boostTime && boostTimer-Time.fixedDeltaTime<=boostTime){
                playerVelocity+=transform.forward*boostSpeed;
                BoostAnimation();
                Rumble.AddRumbleWithTimeOut("Swimming Stroke",1f,.25f);
                //swimmerSound.Stride();
            }
        }

        //Adding external forces, for e.g. from ring booster
        playerVelocity+=forcesToAdd;
        forcesToAdd=Vector3.zero;

        //Adding velocity from swimming
        if(canMove){
            if(playerInput.movingForward && !playerInput.movingBackward){
                if(playerVelocity.magnitude<coastingSpeed || Vector3.Angle(playerVelocity,transform.forward)>=90f){
                    playerVelocity+=transform.forward*acceleration*playerInput.movingForwardValue*Time.fixedDeltaTime;
                }
                animator.SetBool("swimmingForward",true);
                animator.SetBool("swimmingBackward",false);
            }else if(playerInput.movingBackward && !playerInput.movingForward){
                if(playerVelocity.magnitude<coastingSpeed || Vector3.Angle(playerVelocity,-transform.forward)>=90f){
                    float acc=backwardAcceleration;
                    float f=0f;
                    //if(Vector3.Angle(playerVelocity,-transform.forward)>=90f) f=Mathf.Clamp(Vector3.Project(playerVelocity,transform.forward).magnitude/maxVelocity,0f,1f);
                    if(Vector3.Angle(playerVelocity,-transform.forward)>=90f) f=Mathf.Clamp(playerVelocity.magnitude/maxVelocity,0f,1f);
                    acc=acc+acc*f*(backwardAccelerationFactor-1f);
                    playerVelocity+=-transform.forward*acc*Time.fixedDeltaTime;
                }
                animator.SetBool("swimmingBackward",true);
                animator.SetBool("swimmingForward",false);
                boostTimer=boostTime+1f;
            }else{
                animator.SetBool("swimmingForward",false);
                animator.SetBool("swimmingBackward",false);
                if(timerSinceMoveForwardInput<ignoreStrideTime && strideEffect!=null){
                    Destroy(strideEffect);
                    Debug.Log("cancel stride");
                }
                boostTimer=boostTime+1f;
            }

            //Lateral movement
            if(playerInput.movingLeft && !playerInput.movingRight && Mathf.Abs((body.rotation*playerVelocity).x)<lateralMaxVelocity){
                playerVelocity+=-transform.right*lateralAcceleration*Time.fixedDeltaTime;
            }else if(playerInput.movingRight && !playerInput.movingLeft && Mathf.Abs((body.rotation*playerVelocity).x)<lateralMaxVelocity){
                playerVelocity+=transform.right*lateralAcceleration*Time.fixedDeltaTime;
            }

            // Lateral Dash
            if(dashDirection==Directions.LEFT){
                if(Mathf.Abs((body.rotation*playerVelocity).x)<maxDashSpeed || (body.rotation*playerVelocity).x>0) playerVelocity-=dashSpeed*transform.right;
                DashAnimation(playerVelocity);
                swimmerSound.Dash();
            }
            if(dashDirection==Directions.RIGHT){
                if(Mathf.Abs((body.rotation*playerVelocity).x)<maxDashSpeed || (body.rotation*playerVelocity).x<0) playerVelocity+=dashSpeed*transform.right;
                DashAnimation(playerVelocity);
                swimmerSound.Dash();
            }

            //Vertical movement
            if(playerInput.movingUp && !playerInput.movingDown && Mathf.Abs((body.rotation*playerVelocity).y)<lateralMaxVelocity){
                playerVelocity+=transform.up*lateralAcceleration*Time.deltaTime;
            }else if(playerInput.movingDown && !playerInput.movingUp && Mathf.Abs((body.rotation*playerVelocity).y)<lateralMaxVelocity){
                playerVelocity+=-transform.up*lateralAcceleration*Time.fixedDeltaTime;
            }

            // Vertical Dash
            if(dashDirection==Directions.UP){
                if(Mathf.Abs((body.rotation*playerVelocity).y)<maxDashSpeed || (body.rotation*playerVelocity).y<0) playerVelocity+=dashSpeed*transform.up;
                DashAnimation(playerVelocity);
                swimmerSound.Dash();
            }
            if(dashDirection==Directions.DOWN){
                if(Mathf.Abs((body.rotation*playerVelocity).y)<maxDashSpeed || (body.rotation*playerVelocity).y>0) playerVelocity-=dashSpeed*transform.up;
                DashAnimation(playerVelocity);
                swimmerSound.Dash();
            }

        }else{
            animator.SetBool("swimmingForward",false);
            animator.SetBool("swimmingBackward",false);
        }

        playerVelocity=Vector3.ClampMagnitude(playerVelocity,maxVelocity);

        // Deal with collisions part 2 (in case of clipping)
        Vector3 collisionMovement=Vector3.zero;
        foreach (ContactPoint hit in allHits)
        {
            collisionMovement+=MoveOutOf(hit.otherCollider,transform.position+playerVelocity*Time.fixedDeltaTime,hit.normal);
        }

        // Anti-clip movement outside of MovePosition so it doesn't affect the velocity 
        body.position+=collisionMovement;
        // Using MovePosition because it interpolates movement smoothly and keeps velocity in next frame
        body.MovePosition(body.position+playerVelocity*Time.fixedDeltaTime);
        animator.SetFloat("speed",playerVelocity.magnitude);
        if(playerVelocity.magnitude>0f && (playerInput.movingForward || playerInput.movingBackward)){
            swimmerSound.StartSwimming(playerVelocity.magnitude);
        }else{
            swimmerSound.StopSwimming();
        }

        if(dashDirection!=Directions.NULL) prevDashDirection=dashDirection;
        dashDirection=Directions.NULL;

        prevVelocity=body.velocity;

        allHits.Clear();

    }

    public void OverrideRotation(Quaternion rotation, float rotationSpeed=-1f){
        targetRotationOverride=rotation;
        overridingRotation=true;
        tempRotationOverrideSpeed=rotationSpeed;
    }

    public void OverrideRotation(Transform target, float rotationSpeed=-1f){
        targetRotationOverride=Quaternion.LookRotation(target.position-transform.position,Vector3.up);
        overridingRotation=true;
        tempRotationOverrideSpeed=rotationSpeed;
    }

    // Boost can be from external effect for e.g. ring
    public void Boost(Vector3 force){
        forcesToAdd+=force;
    }

    //Things to animate right after boost happens
    void BoostAnimation(){
        swimmerCamera.BoostAnimation();
    }

    //Things to animate right after Dash happens
    void DashAnimation(Vector3 playerVelocity){
        if(visible){
            swimmerCamera.DashAnimation();
            Vector3 projectVector=transform.right;
            switch(dashDirection){
                case Directions.UP:
                    projectVector=transform.up;
                    break;
                case Directions.DOWN:
                    projectVector=-transform.up;
                    break;
                case Directions.LEFT:
                    projectVector=-transform.right;
                    break;
                case Directions.RIGHT:
                    projectVector=transform.right;
                    break;
            }
            Vector3 projectedVector=Vector3.Project(playerVelocity,projectVector);
            if(projectedVector.normalized==projectVector.normalized){
                swimmerTrails.DashTrail(dashDirection,projectedVector.magnitude);
            }
            GameObject g=Instantiate(afterimageSprite,afterimageSprite.transform.position,afterimageSprite.transform.rotation);
            g.SetActive(true);
            g.GetComponent<SpriteRenderer>().sprite=spriteRenderer.sprite;
        }
    }

    private void OnCollisionStay(Collision other) {
        if(other.collider.gameObject.tag!="Player"){
            ContactPoint[] myContacts = new ContactPoint[other.contactCount];
            for(int i = 0; i < myContacts.Length; i++)
            {
                myContacts[i] = other.GetContact(i);
                allHits.Add(myContacts[i]);
            }
        }
    }

    //This function is for movement of player out of a collider without moving too fast
    //It doesn't move the player but it returns the Vector3 to move it by
    public Vector3 MoveOutOf(Collider other, Vector3 pos, Vector3 normal){
        Vector3 prevVelocity=body.velocity;

        Vector3 dir=new Vector3();
        switch(capsule.direction){
            case 0:
                dir=transform.right;
                break;
            case 1:
                dir=transform.up;
                break;
            case 2:
                dir=transform.forward;
                break;
        }
        Vector3 point1=pos+capsule.center+dir*(capsule.height/2f-capsule.radius);
        Vector3 point2=pos+capsule.center-dir*(capsule.height/2f-capsule.radius);
        RaycastHit[] hits;

        bool colliding=true;
        int tries=0;
        Vector3 movement=Vector3.zero; //Amount to move the collider 

        LayerMask mask = LayerMask.GetMask("Default","Wall");

        while(colliding && tries<maxMoveoutEffort){
            hits=Physics.CapsuleCastAll(point1+movement,point2+movement,capsule.radius+skinWidth,normal,0f,mask,QueryTriggerInteraction.Ignore);
            colliding=false;
            foreach(RaycastHit hit in hits){
                if(hit.collider==other){
                    colliding=true;
                    movement+=normal*minMoveoutStep;
                    break;
                }
            }
            tries++;
        }

        return movement;

    }

    private Vector3 CheckForWallAndKick(Vector3 direction){
        bool isKicking=false;

        Vector3 prevVelocity=body.velocity;

        Vector3 dir=new Vector3();
        switch(capsule.direction){
            case 0:
                dir=transform.right;
                break;
            case 1:
                dir=transform.up;
                break;
            case 2:
                dir=transform.forward;
                break;
        }
        float radius=capsule.radius*wallCheckCapsuleRadiusMultiplier;
        Vector3 point1=body.position+capsule.center+dir*(capsule.height/2f-radius);
        Vector3 point2=body.position+capsule.center-dir*(capsule.height/2f-radius);
        RaycastHit[] hits;

        LayerMask mask = LayerMask.GetMask("Wall");

        Vector3 forward=transform.rotation*direction;

        hits=Physics.CapsuleCastAll(point1,point2,radius+skinWidth,-forward,0f,mask,QueryTriggerInteraction.Ignore);

        Vector3 totalForce=Vector3.zero;

        GameObject dustCloud=null;

        foreach(RaycastHit hit in hits){
            if(hit.collider.gameObject.tag!="Player"){
                RaycastHit hit2=CreateRaycastHitFromCollider(body.position+capsule.center,hit.collider);
                Vector3 normal=hit2.normal;
                Debug.Log(normal);
                if(Vector3.Angle(normal,forward)<90f){
                    Vector3 force=Vector3.Project(forward,normal)*wallBoost;
                    force=forward*force.magnitude;
                    Debug.DrawRay(body.position,normal*3f, Color.magenta, 3f);
                    Debug.DrawRay(body.position,force*3f, Color.green, 3f);
                    totalForce+=force;
                    isKicking=true;
                    if(dustCloud==null){
                        //If hit2 position is very far away (probably infinity), do not make dust cloud
                        if((Mathf.Abs(hit2.point.x) > 1000) ||(Mathf.Abs(hit2.point.y) > 500) ||
                            (Mathf.Abs(hit2.point.z) > 1000)
                        ){

                        }else{
                            dustCloud=Instantiate(dustCloudPrefab,transform.parent);
                            dustCloud.transform.position=hit2.point+normal*0.2f;
                        }

                    }
                }
            }
        }

        if(isKicking){
            swimmerSound.Kick();
        }

        totalForce=Vector3.ClampMagnitude(totalForce,wallBoost);
        return totalForce;
    }
    public void ToggleMovement(){
        canMove=!canMove;
        canRotate=!canRotate;
    }

    // Function borrowed from https://discussions.unity.com/t/how-do-i-obtain-the-surface-normal-for-a-point-on-a-collider-cant-use-raycasthit-normal/16223/4
    public static RaycastHit CreateRaycastHitFromCollider(Vector3 _rayOrigin, Collider _collider)
    {
        var colliderTr = _collider.transform;

        // Returns a point on the given collider that is closest to the specified location.
        // Note that in case the specified location is inside the collider, or exactly on the boundary of it, the input location is returned instead.
        // The collider can only be BoxCollider, SphereCollider, CapsuleCollider or a convex MeshCollider.
        var closestPoint = Physics.ClosestPoint(_rayOrigin, _collider, colliderTr.position, colliderTr.rotation);
  
        if (_collider is MeshCollider {convex: false} meshCollider)
        {
            Debug.LogWarning($"do not use convex mesh-colliders as it does not deal well with physics at all. " +
                             $"There are solutions provided in the asset store to automatically transform non-convex meshes to convex meshes. The problematic mesh: {_collider.transform} meshName:{meshCollider.sharedMesh.name}");
            // This is not great. If we have complex meshColliders we will encounter issues.
            closestPoint = _collider.ClosestPointOnBounds(_rayOrigin);
        }

        bool inside; 
        inside = _collider.bounds.Contains(_rayOrigin); 
        if (inside) { 
            Debug.LogWarning("Getting raycast to nearest point failed - we are inside provided collider"); 
        } 
        
        var dir = (closestPoint - _rayOrigin).normalized;
        var ray = new Ray(_rayOrigin, dir);
        var hasHit = _collider.Raycast(ray, out var hitInfo, float.MaxValue);
        
        if (hasHit == false)
        {
            Debug.DrawRay(_rayOrigin, dir*5f, Color.magenta, 10f);
            Debug.Log(_collider);
            Debug.LogError($"This case will never happen!");
        }

        return hitInfo;
    }

    public Vector3 GetVelocity()
    {
        if(body!=null) return body.velocity;
        else return Vector3.zero;
    }
    
    public Vector3 GetRotationVelocity(){
        return rotationVelocity;
    }

    public bool IsCoasting(){
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("swimForwardSlow") || animator.GetCurrentAnimatorStateInfo(0).IsName("swimForwardSlower")){
            return true;
        }else return false;
    }

    public bool IsSwimmingBackwards(){
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("swimBackward") || animator.GetCurrentAnimatorStateInfo(0).IsName("startSwimBackwardFast")){
            return true;
        }else return false;
    }

    public void StartedDialogue(bool isAmbient=false){
        if(!isAmbient){
            canMove=false;
        }
    }

    public void FinishedDialogue(bool isAmbient=false){
        if(!isAmbient){
            canMove=true;
            canRotate=true;
        }
    }

    public void Respawn(){
        if(respawnTransform!=null){
            Transport(respawnTransform.position,respawnTransform.rotation);
            StartCoroutine(WakeUp(0f));
        }
    }

    public void TurnInvisible(){
        visible=!visible;
        spriteRenderer.enabled=visible;
        swimmerTrails.visible=visible;
    }

    public void Transport(Vector3 position){
        transform.position=position;
    }

    public void Transport(Vector3 position,Quaternion rotation){
        transform.position=position;
        transform.rotation=rotation;
        body.position=transform.position;
        body.rotation=transform.rotation;
        Physics.SyncTransforms();
        swimmerCamera.ResetCamera();
        FindObjectOfType<LevelLoader>().FadeIn(1f);
    }

    IEnumerator WakeUp(float time){
        yield return new WaitForSeconds(time);
        animator.SetBool("sleeping",false);
        sleeping=false;
        yield return new WaitForSeconds(1.5f);
        if(organOut) yield return new WaitForSeconds(1f);
        canMove=true;
        canRotate=true;
        GetComponentInChildren<SwimmerSinging>().canSing=true;
        wakingCoroutine=null;
    }

    void Sleep(){
        canMove=false;
        canRotate=false;
        GetComponentInChildren<SwimmerSinging>().canSing=false;
        sleeping=true;
        animator.SetBool("sleeping",sleeping);
    }
}



public enum Directions{
    NULL,
    UP,
    DOWN,
    LEFT,
    RIGHT
}


/*
TO-DO:
    + MAKE LATERAL/FORWARD MOVEMENT ONLY ONE AT A TIME
    + ADD BOOST
    + EXPRIMENT WITH LERP INSTEAD OF LINEAR DECELERATION
    + LOOK INTO PHYSICS SNAPPING
    + MAKE CAMERA COLLISIONS work
*/

/* REMOVED BUT COULD STILL BE USEFUL CODE:

Dampening post-collision speed:
        //Checking if the player just collided and went flying off => dampen speed
        
        [Tooltip("Sometimes when colliding with something the character goes flying off. We use this value to limit the speed resulting.")]
        public float maxCollisionSpeed=4f;

        if(justCollided){
            
            if(collisionNumber==1){

            }
            Vector3 velocityChange=playerVelocity-collisionVelocity;
            if(velocityChange.magnitude>=maxCollisionSpeed && Vector3.Angle(collisionVelocity,playerVelocity)>=45){
            //     //playerVelocity=Vector3.zero;

                
                //playerVelocity=collisionVelocity+Vector3.ClampMagnitude(velocityChange,maxCollisionSpeed);
                playerVelocity=collisionVelocity+velocityChange.normalized*(maxCollisionSpeed+Mathf.Sqrt(velocityChange.magnitude-maxCollisionSpeed+1)-1);
            }
            justCollided=false;
        }else{
            collisionNumber=0;
        }

*/