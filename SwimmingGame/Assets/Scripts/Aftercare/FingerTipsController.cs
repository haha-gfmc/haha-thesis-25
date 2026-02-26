using UnityEngine;

public class FingerTipsController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float maxMoveRadius = 1.5f;
    public float dampingFactor = 0.9f;
    public float lerpSpeed = 1.0f;
    public float yLerpSpeed = 1.0f;
    public float yOffset = 1.0f;
    public float inputResetDelay = 0.5f; // Time in seconds before resetting position

    [Header("References")]
    public GameObject reference;
    public LayerMask sexPartnerMask;

    private PlayerInput playerInput;
    private Vector3 startLocalPosition;
    private Vector3 velocity;
    private CuddleGameManager gameManager;
    public string lastDialogueOption = "Null option";
    private bool isUsingGamepad;

    private float inputTimer = 0f; // Tracks time since last input

    private CuddleDialogue cuddleDialogue;

    public float changeTimer=0f;
    private float prevInputAngle;
    public float maxNoChangeTime=0.5f;

    [SerializeField] private bool caressing = false;
    public bool useThisFingerTipToDetectDialogue = true; 
    public LightBeamFollow[] lightBeamFollow;
    public GameObject[] choiceBox;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        startLocalPosition = transform.localPosition; // Set the starting position in local space
        gameManager = FindObjectOfType<CuddleGameManager>();
        cuddleDialogue=FindObjectOfType<CuddleDialogue>();

    }

    private void Update()
    {
        if (useThisFingerTipToDetectDialogue){
            DetectDialogueOption(caressing);
        }


        if (changeTimer >= maxNoChangeTime)
        {
            caressing = false;
        }
        changeTimer += Time.deltaTime;

    }

    void FixedUpdate()
    {
        HandleInput();
        Moving();

        if (inputTimer >= inputResetDelay)
        {
            ResetPosition();
        }
        AdjustYPosition();
    }

    // Handle input and determine control scheme
    private void HandleInput()
    {
        if (playerInput.currentControlScheme == "Gamepad")
        {
            isUsingGamepad = true;
        }
        else
        {
            isUsingGamepad = false;
        }
    }

    private void Moving()
    {
        float moveX = isUsingGamepad ? -playerInput.rotation.y : -playerInput.look.y;
        float moveY = isUsingGamepad ? playerInput.rotation.x : playerInput.look.x;


        float inputAngle;    //input angle

        if (Mathf.Abs(moveX) > gameManager.controllerDeadZone || Mathf.Abs(moveY) > gameManager.controllerDeadZone)
        {
            Vector3 inputDirection = new Vector3(moveX, moveY, 0).normalized;
            velocity = inputDirection * moveSpeed * Time.fixedDeltaTime;
            inputTimer = 0f; 
            caressing=true;

            //Checking if there has been significant change in player input
            inputAngle=Mathf.Atan2(moveX,moveY);
            if(Mathf.Abs(inputAngle-prevInputAngle)>=Mathf.PI/4){
                prevInputAngle=inputAngle;
                changeTimer=0f;
            }else{
            }

            //If no big change in a while (aka if player is just pointing one way) caress isn't working anymore
            //IN THE FUTURE maybe we should find new ways that character like different caresses,
            // and maybe make caress timer go up faster if caressing in a nicer way?
            
        }
        else
        {
            inputAngle=0f;
            velocity *= dampingFactor; // Apply damping
            inputTimer += Time.fixedDeltaTime; // increment timer when no input
            caressing = false;
        }

        

        Vector3 localPosition = transform.localPosition + velocity;
        Vector3 localOffsetFromCenter = localPosition - startLocalPosition;

        if (localOffsetFromCenter.magnitude > maxMoveRadius)
        {
            localOffsetFromCenter = localOffsetFromCenter.normalized * maxMoveRadius;
            localPosition = startLocalPosition + localOffsetFromCenter;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, lerpSpeed * Time.fixedDeltaTime);
    }

    // Return to the reference object's position
    private void ResetPosition()
    {
        Vector3 referenceWorldPosition = reference.transform.position;
        transform.position = Vector3.Lerp(transform.position, referenceWorldPosition, lerpSpeed * Time.fixedDeltaTime);
        startLocalPosition = transform.localPosition; // Update start position
    }

    // Detect nearby dialogue options
    private void DetectDialogueOption(bool c)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f, LayerMask.GetMask("DialogueOption"));
        foreach (var hitCollider in hitColliders)
        {
            string currentOption = hitCollider.gameObject.name;
            if (currentOption != lastDialogueOption)
            {
                lastDialogueOption = currentOption;
                //gameManager.UpdateDialogueText(currentOption); 
            }

            //int i=int.Parse(currentOption.Substring(0,1))-1;

            int i=hitCollider.gameObject.transform.GetSiblingIndex();
            
            cuddleDialogue.HoveringChoice(i);
            Debug.Log("Hovering choice: " + i);
            for (int j = 0; j < lightBeamFollow.Length; j++)
            {
                if (choiceBox[j].activeSelf)
                {
                    lightBeamFollow[j].hovering = true;
                    lightBeamFollow[j].ActivateLightBeam(i);
                }
            }


            
            if(c){
                cuddleDialogue.SelectingChoice(i);
            }

            if(cuddleDialogue.story.currentChoices.Count>0 && cuddleDialogue.currentChoiceIndex!=-1) Rumble.AddRumble("In Cuddle Zone");

        }
        if (hitColliders.Length == 0){
            for (int j = 0; j < lightBeamFollow.Length; j++)
            {
                if (choiceBox[j].activeSelf)
                {
                    lightBeamFollow[j].hovering = false;
                }
            }
        }

    }

    // Adjust Y position based on the highest object below
    private void AdjustYPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, 100f, transform.position.z), Vector3.down, out hit, Mathf.Infinity, sexPartnerMask))
        {
            Vector3 targetPosition = new Vector3(transform.position.x, hit.point.y + yOffset, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, yLerpSpeed * Time.fixedDeltaTime);
        }
    }
}
