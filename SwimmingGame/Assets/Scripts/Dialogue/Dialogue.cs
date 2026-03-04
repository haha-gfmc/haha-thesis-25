using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;

public class Dialogue : MonoBehaviour
{
    public static Dialogue Instance { get; private set; }

    [SerializeField]
    private TextAsset inkJSONAsset = null;
    [HideInInspector]
    public Story story;

    public string currentKnotName;

    public string displayText; //Current text to display

    public int currentChoiceIndex = 0; //Currently selected choice index
    [Tooltip("Name of current person talking")]
    public string talker = "";
    private float pauseTimer = 0f;
    [HideInInspector]
    public bool isAmbient = false;
    private float ambientTimer = 0f;


    private PlayerInput playerInput;
    private GameManager gameManager;
    private SwimmerSinging swimmerSinging;

    public bool startStoryTrigger;

    public bool inDialogue = false;
    private bool choicePicked = false;

    public NPCOverworld npcInterlocutor;
    private Swimmer swimmer;

    [Tooltip("Can the player control choice with buttons?")]
    public bool controlChoice = true;
    [Tooltip("Can select dialogue with D-Pad or equivalent vs singing?")]
    public bool canSelectDialogue = true;

    [Tooltip("Place choice boxes along singing wheel?")]
    public bool placeChoiceBoxesAroundWheel = false;


    [Header("Canvas objects")]
    [Tooltip("Group to completely show or hide when start/stop dialogue.")]
    public GameObject canvasParent;
    [Tooltip("Dialogue views with specific textbox arrangement.")]
    public GameObject[] views;
    public int currentViewIndex = 0;
    [Tooltip("Object to display interlocutor dialogue line.")]
    public GameObject interlocutorTextBox;
    // TMP object for the interlocutor spoken line
    private TMP_Text interlocutorLineTMP;
    [Tooltip("Object to display player dialogue line.")]
    public GameObject playerTextBox;
    // TMP object for the spoken line
    private TMP_Text playerLineTMP;
    [Tooltip("Textboxes for individual characters")]
    public InterlocutorBox[] interlocutorsTextBoxes;
    private GameObject textBox;
    private TMP_Text lineTMP;
    [Tooltip("Objects to display choices.")]
    public GameObject[] choiceTextBoxes;
    [Tooltip("Move boxes to these positions if we only have 2 choices.")]
    public RectTransform[] alternateChoiceTextBoxes2;
    [Tooltip("Holder of position for choice boxes along singing wheel.")]
    public RectTransform[] choiceBoxesSingingWheelPlaces;
    private Vector3[] choiceBoxesPositions3; //Use these positions if we have 3 choices
    private Vector3[] choiceBoxesPositions2;  //Use these positions if we have 2 choices
    private Vector3[] choiceBoxesPositionsWheel;
    [HideInInspector]
    public TMP_Text[] choiceTMPs;


    [Header("Misc.")]
    public float dialogueSpeedSlow = 10f;
    public float dialogueSpeedNormal = 30f;
    public float dialogueSpeedFast = 80f;

    //Index of progress in showing the current line
    private float currentCharacterIndex = 0f;
    private float currentTextSpeed = 1f;

    //Inline pause: pausing in the middle of a line when writing \pause
    public float inlinePauseLength = 1f; //in seconds
    private float inlinePauseTimer = 0f;
    [Tooltip("Font size for next character being written")]
    public int nextCharSize = 38;
    [Tooltip("Color for next character being written")]
    public string nextCharColor = "grey";
    [Tooltip("Modify speed of typewriting if last char is a period")]
    public float periodSpeedModifier = .1f;

    [Tooltip("Modify speed of typewriting if last char is another kind of punctuation")]
    public float otherPunctuationSpeedModifier = .2f;

    [Header("Additional Text Boxes")]
    public GameObject standardTextBox;
    public GameObject floralTextBox;
    public GameObject boneTextBox;

    private Color defaultTextBoxColor;
    private bool changedColor = false;
    [HideInInspector]
    public float[] defaultChoiceBoxOpacites;

    [Header("Singing & Choice")]

    [Tooltip("The angle value for pointing straight down according to swimmerSinging angle.")]
    public float choiceSingingAngleOffset = 0.5f;
    public float choiceSingingAngleWindow = 0.2f;
    private float singingTimer = 0f;
    public float singingRequiredLength = 1f;
    public float singingCancelSpeed = .5f;
    public float chosenScale = 1.5f;
    public float scaleSpeed = 1f;


    private EventInstance bubblesInstance;

    private List<GameObject> lingeringBoxes;    //Storing boxes that stay on screen

    [Tooltip("Burst settings")]
    private bool burst = false;
    public bool burstAfterEveryLine = false;
    public bool burstAtPauses = true;
    List<GameObject> burstClones;
    List<Coroutine> burstCoroutines;


    private List<string> prevTags;

    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 

        playerInput = FindObjectOfType<PlayerInput>();
        gameManager = FindObjectOfType<GameManager>();

        SetUpView();

        Image[] images = choiceTextBoxes[0].GetComponentsInChildren<Image>();
        defaultChoiceBoxOpacites = new float[images.Length];
        for (var i = 0; i < defaultChoiceBoxOpacites.Length; i++)
        {
            Color c = images[i].color;
            defaultChoiceBoxOpacites[i] = c.a;
        }

        choiceBoxesPositions3 = new Vector3[choiceTextBoxes.Length];
        for (var i = 0; i < choiceTextBoxes.Length; i++)
        {
            choiceBoxesPositions3[i] = choiceTextBoxes[i].GetComponent<RectTransform>().anchoredPosition;
        }

        if (alternateChoiceTextBoxes2.Length > 0)
        {
            choiceBoxesPositions2 = new Vector3[2];
            for (var i = 0; i < alternateChoiceTextBoxes2.Length; i++)
            {
                choiceBoxesPositions2[i] = alternateChoiceTextBoxes2[i].anchoredPosition;
            }
        }

        if (placeChoiceBoxesAroundWheel)
        {
            choiceBoxesPositionsWheel = new Vector3[choiceBoxesSingingWheelPlaces.Length];
            for (var i = 0; i < choiceBoxesPositionsWheel.Length; i++)
            {
                choiceBoxesPositionsWheel[i] = choiceBoxesSingingWheelPlaces[i].anchoredPosition;
            }
        }

        DialogueAwake();

        HideText();
        HideChoices();

        swimmer = FindObjectOfType<Swimmer>();

        if (inkJSONAsset != null)
        {
            StartStory();
        }

        swimmerSinging = FindObjectOfType<SwimmerSinging>();

        bubblesInstance = RuntimeManager.CreateInstance("event:/Non-Diagetic SFX/Bubbles/Bubbles - Loop");

        lingeringBoxes = new List<GameObject>();

        burstClones = new List<GameObject>();
        burstCoroutines = new List<Coroutine>();

    }

    public void SetUpView()
    {
        choiceTMPs = new TMP_Text[choiceTextBoxes.Length];
        for (int i = 0; i < choiceTextBoxes.Length; i++)
        {
            choiceTMPs[i] = choiceTextBoxes[i].GetComponentInChildren<TMP_Text>();
        }
        interlocutorLineTMP = interlocutorTextBox.GetComponentInChildren<TMP_Text>();
        playerLineTMP = playerTextBox.GetComponentInChildren<TMP_Text>();

        lineTMP = interlocutorLineTMP;
        textBox = interlocutorTextBox;

        standardTextBox.SetActive(false);
        floralTextBox.SetActive(false);
        boneTextBox.SetActive(false);

    }

    virtual public void DialogueAwake()
    {

    }

    void Update()
    {
        DialogueUpdate();

        if (inDialogue)
        {
            if (!isAmbient && pauseTimer <= 0f)
            {
                TypeWriter();

                //Handling player input: Continuing/Picking
                if (playerInput.interacting && !playerInput.prevInteracting)
                {
                    if (currentCharacterIndex < displayText.Length)
                    {
                        currentCharacterIndex = displayText.Length;
                    }
                    else
                    {
                        if (story.canContinue && story.currentChoices.Count == 0)
                        {
                            Continue();
                            if (story.currentChoices.Count == 0)
                            {
                                HideChoices();
                            }
                        }
                        else if (story.currentChoices.Count > 0 && currentChoiceIndex <= story.currentChoices.Count
                            && currentChoiceIndex >= 0 && controlChoice)
                        {
                            PickChoice(currentChoiceIndex);
                        }
                        else if (story.currentChoices.Count > 0 && currentChoiceIndex <= story.currentChoices.Count)
                        {
                            if (canSelectDialogue) currentChoiceIndex = 0;
                        }
                        else
                        {
                            EndDialogue();
                        }
                        if (displayText == "" && !story.canContinue)
                        {
                            EndDialogue();
                        }
                    }
                }

                //Handling player input: picking choice
                if (story.currentChoices.Count > 0 && currentCharacterIndex >= displayText.Length && controlChoice)
                {
                    singingTimer -= Time.deltaTime * singingCancelSpeed * singingRequiredLength;
                    if (swimmerSinging.singing && swimmerSinging.singingVolume > .2f)
                    {
                        bool pointingToAChoice = false;
                        for (var i = 0; i < choiceTextBoxes.Length; i++)
                        {
                            if (choiceTextBoxes[i].activeInHierarchy)
                            {
                                float a = Vector2.SignedAngle(choiceTextBoxes[i].GetComponent<RectTransform>().anchoredPosition, new Vector2(0f, -1f)) / 180f;
                                //float a=Vector2.SignedAngle(choiceTextBoxes[i].GetComponent<RectTransform>().position,FindObjectOfType<SwimmerSinging>().wheelRect.position)/180f;
                                a = (2 - a + choiceSingingAngleOffset) % 2f;
                                if (Mathf.Abs(a - swimmerSinging.singingAngle) <= choiceSingingAngleWindow || Mathf.Abs(a - swimmerSinging.singingAngle - 2f) <= choiceSingingAngleWindow)
                                {
                                    if (currentChoiceIndex != i)
                                    {
                                        singingTimer = 0f;
                                        bubblesInstance.start();
                                    }
                                    pointingToAChoice = true;
                                    currentChoiceIndex = i;
                                    singingTimer += Time.deltaTime;
                                    Rumble.AddRumble("Picking Dialogue", singingTimer / singingRequiredLength);
                                    if (singingTimer >= singingRequiredLength)
                                    {
                                        PickChoice(currentChoiceIndex);
                                        Rumble.AddRumbleWithTimeOut("Picking Dialogue",1f,.5f);
                                    }
                                }
                            }
                        }
                        if (!pointingToAChoice)
                        {
                            currentChoiceIndex = -1;
                            bubblesInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        }
                        // if(angle>choiceSingingMinAngle && angle<choiceSingingMaxAngle){
                        //     for(var i=0;i<story.currentChoices.Count;i++){
                        //         if(angle>=choiceSingingMinAngle+i*(choiceSingingMaxAngle-choiceSingingMinAngle)/story.currentChoices.Count &&
                        //         angle<=choiceSingingMinAngle+(i+1)*(choiceSingingMaxAngle-choiceSingingMinAngle)/story.currentChoices.Count){
                        //             currentChoiceIndex=i;
                        //         }
                        //     }
                        // }
                    }
                    else
                    {
                        bubblesInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        if (canSelectDialogue)
                        {
                            if ((playerInput.navigateDown && !playerInput.prevNavigateDown) ||
                                (playerInput.navigateRight && !playerInput.prevNavigateRight))
                            {
                                currentChoiceIndex += 1;
                                currentChoiceIndex = (currentChoiceIndex + story.currentChoices.Count) % story.currentChoices.Count;
                            }
                            if ((playerInput.navigateUp && !playerInput.prevNavigateUp) ||
                                (playerInput.navigateLeft && !playerInput.prevNavigateLeft))
                            {
                                currentChoiceIndex -= 1;
                                currentChoiceIndex = (currentChoiceIndex + story.currentChoices.Count) % story.currentChoices.Count;
                            }
                        }
                    }
                    singingTimer = Mathf.Clamp(singingTimer, 0f, singingRequiredLength);
                }

                //Move on with picking
                if (inDialogue && choicePicked && story.currentChoices.Count > 0 &&
                    currentChoiceIndex <= story.currentChoices.Count && currentChoiceIndex >= 0)
                {
                    story.ChooseChoiceIndex(currentChoiceIndex);
                    Continue();
                    HideChoices();
                    choicePicked = false;
                    currentChoiceIndex = -1;
                    if (story.currentChoices.Count == 0)
                    {
                        if (!story.canContinue)
                        {
                            EndDialogue();
                        }
                    }
                }

                //Showing/Hiding text
                if (inDialogue && pauseTimer <= 0f)
                {
                    ShowText();
                    if (story.currentChoices.Count > 0 && currentCharacterIndex >= displayText.Length)
                    {
                        ShowChoices();
                    }
                    else
                    {

                    }
                }
            }
            else if (pauseTimer >= 0f)
            {
                pauseTimer -= Time.deltaTime;
                HideText();
                if (pauseTimer <= 0f)
                {
                    ShowText();
                }
            }
            else if (isAmbient)
            { //Dialogue is ambient if it just appears without player control
                TypeWriter();

                ambientTimer -= Time.deltaTime;
                if (ambientTimer <= 0f)
                {
                    if (story.canContinue)
                    {
                        Continue();
                    }
                    else
                    {
                        EndDialogue();
                    }
                }
                else
                {
                    if (inDialogue)
                    {
                        ShowText();
                    }
                }
            }

        }

        //Trigger mostly for debug purposes, remove later
        if (startStoryTrigger)
        {
            StartDialogue(inkJSONAsset, currentKnotName, npcInterlocutor);
            startStoryTrigger = false;
        }

        if (burstAfterEveryLine) burst = true;
        else burst = false;
    }

    virtual public void DialogueUpdate()
    {

    }

    public void PickChoice(int choiceIndex)
    {
        currentChoiceIndex = choiceIndex;
        choicePicked = true;
        bubblesInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        singingTimer = 0f;
        Sound.PlayOneShotVolume("event:/Non-Diagetic SFX/Bubbles/Bubbles - Burst", 1f);
    }

    //Show spoken text (not choices) on UI
    void ShowText()
    {
        textBox.SetActive(true);
        //lineTMP.text=GetWordByWordLine(displayText,Mathf.FloorToInt(currentCharacterIndex));
        string text = displayText.Substring(0, Mathf.FloorToInt(currentCharacterIndex));
        if (currentCharacterIndex < displayText.Length && currentCharacterIndex > 1)
        {
            if (inlinePauseTimer <= 0f) text = displayText.Substring(0, Mathf.Max(text.Length - 1, 0)) + "<color=" + nextCharColor + "><size=" + nextCharSize.ToString() + ">" + text[Mathf.Max(text.Length - 1, 0)] + "</size></color>";
            text += "<color=#00000000>" + displayText.Substring(Mathf.FloorToInt(currentCharacterIndex)) + "</color>";
            text = text.Replace("\\pause", "");
        }
        lineTMP.text = text;
        canvasParent.SetActive(true);
        if (currentCharacterIndex < displayText.Length && inlinePauseTimer <= 0f)
        {
            textBox.GetComponentInChildren<Animator>().speed = currentTextSpeed / dialogueSpeedNormal;
        }
        else
        {
            textBox.GetComponentInChildren<Animator>().speed = 0.5f;
        }
    }

    string GetWordByWordLine(string text, int index)
    {
        while (index < currentCharacterIndex && System.Char.IsLetter(text[index - 1]))
        {
            index -= 1;
        }
        text = text.Substring(0, Mathf.FloorToInt(index));
        return text;
    }

    //Show choices on UI
    public virtual void ShowChoices()
    {
        if (placeChoiceBoxesAroundWheel && !choiceTextBoxes[0].gameObject.activeInHierarchy)
        {
            // int[] positions=new int[choiceBoxesPositionsWheel.Length];
            // for(int k=0;k<positions.Length;k++){
            //     positions[k]=k;
            // }
            //Shuffle(positions);
            int offsetInt = Random.Range(0, choiceBoxesPositionsWheel.Length);
            if (interlocutorTextBox == boneTextBox)
            {
                if (story.currentChoices.Count == 2) offsetInt = 0;
                else offsetInt = 4;
            }
            for (int i = 0; i < Mathf.Min(story.currentChoices.Count, choiceTextBoxes.Length); i++)
            {
                RectTransform rect = choiceTextBoxes[i].GetComponent<RectTransform>();
                //Randomize starting position of where choice boxes appear
                rect.anchoredPosition = choiceBoxesPositionsWheel[(i + offsetInt) % choiceBoxesPositionsWheel.Length];
            }
        }

        for (int i = 0; i < Mathf.Min(story.currentChoices.Count, choiceTextBoxes.Length); i++)
        {
            RectTransform rect = choiceTextBoxes[i].GetComponent<RectTransform>();

            if (!choiceTextBoxes[i].gameObject.activeInHierarchy)
            {
                choiceTextBoxes[i].gameObject.SetActive(true);
                choiceTextBoxes[i].GetComponentInChildren<Animator>().StartPlayback();
                choiceTextBoxes[i].GetComponentInChildren<Animator>().Play("Base Layer.smallDialogueBubbleIdle", -1, Random.Range(0f, 1f));
                choiceTextBoxes[i].GetComponentInChildren<Animator>().speed = 1f;
            }

            if (!placeChoiceBoxesAroundWheel)
            {
                if (story.currentChoices.Count == 2 && choiceBoxesPositions2.Length > 0)
                {
                    rect.anchoredPosition = choiceBoxesPositions2[i];
                }
                else if (story.currentChoices.Count == 1 || story.currentChoices.Count == 3)
                {
                    rect.anchoredPosition = choiceBoxesPositions3[i];
                }
            }

            choiceTMPs[i].text = story.currentChoices[i].text;
            float targetS = 1f;
            if (i == currentChoiceIndex)
            {
                Image[] images = choiceTextBoxes[i].GetComponentsInChildren<Image>();
                for (var k = 0; k < images.Length; k++)
                {
                    Color c = images[k].color;
                    c.a = 1 * defaultChoiceBoxOpacites[k];
                    images[k].color = c;
                }
                if (singingTimer > 0f)
                {
                    float k = EaseOutSine(singingTimer / singingRequiredLength);
                    targetS = targetS + (chosenScale - 1f) * k;
                }
                choiceTextBoxes[i].GetComponentInChildren<Animator>().speed = 1.25f;
            }
            else
            {
                Image[] images = choiceTextBoxes[i].GetComponentsInChildren<Image>();
                for (var k = 0; k < images.Length; k++)
                {
                    Color c = images[k].color;
                    c.a = 0.625f * defaultChoiceBoxOpacites[k];
                    images[k].color = c;
                }
                choiceTextBoxes[i].GetComponentInChildren<Animator>().speed = 0.5f;
            }

            Vector3 localScale = rect.localScale;
            float s = Mathf.Lerp(localScale.x, targetS, scaleSpeed * Time.deltaTime);
            localScale = new Vector3(s, s, s);
            rect.localScale = localScale;
        }
    }

    public float EaseOutSine(float x)
    {
        return Mathf.Sin((x * Mathf.PI) / 2f);
    }

    //Remove all UI elements
    void HideText(bool everything = true)
    {
        interlocutorLineTMP.text = "";
        playerLineTMP.text = "";
        if (burst && playerTextBox.activeInHierarchy && !ContainsTag(prevTags, "stayonscreen"))
        {
            BurstTextbox(playerTextBox);
        }
        playerTextBox.SetActive(false);
        if (burst && interlocutorTextBox.activeInHierarchy && !ContainsTag(prevTags, "stayonscreen"))
        {
            BurstTextbox(interlocutorTextBox);
        }
        interlocutorTextBox.SetActive(false);
        foreach (InterlocutorBox interlocutorBox in interlocutorsTextBoxes)
        {
            if (burst && interlocutorBox.textBox.activeInHierarchy && !ContainsTag(prevTags, "stayonscreen"))
            {
                BurstTextbox(interlocutorBox.textBox);
            }
            interlocutorBox.textBox.SetActive(false);
        }
        if (everything) canvasParent.SetActive(false);
    }

    void BurstTextbox(GameObject burstingTextBox)
    {
        GameObject imageCarrier = burstingTextBox.GetComponentInChildren<Image>().gameObject;
        Animator a;
        if(imageCarrier.TryGetComponent<Animator>(out a)) burstClones.Add(Instantiate(imageCarrier, imageCarrier.transform.position, imageCarrier.transform.rotation, canvasParent.transform.parent));
        DialogueBoxPlacement dbp;
        if (burstingTextBox.TryGetComponent<DialogueBoxPlacement>(out dbp))
        {
            if (dbp.bubble1 != null && dbp.bubble1.TryGetComponent<Animator>(out a))
            {
                burstClones.Add(Instantiate(dbp.bubble1.gameObject, dbp.bubble1.transform.position, dbp.bubble1.transform.rotation, canvasParent.transform.parent));
            }
            if (dbp.bubble2 != null && dbp.bubble2.TryGetComponent<Animator>(out a))
            {
                burstClones.Add(Instantiate(dbp.bubble2.gameObject, dbp.bubble2.transform.position, dbp.bubble2.transform.rotation, canvasParent.transform.parent));
            }
        }

        foreach (GameObject clone in burstClones)
        {
            burstCoroutines.Add(StartCoroutine(SetBursting(clone)));
        }
    }

    IEnumerator SetBursting(GameObject g)
    {
        yield return new WaitForSeconds(.01f);
        g.GetComponentInChildren<Animator>().SetTrigger("burst");
        yield return new WaitForSeconds(1f);
        burstClones.Remove(g);
        Destroy(g);
        burstCoroutines.RemoveAt(0);
    }

    //Remove choice UI elements
    public virtual void HideChoices()
    {
        singingTimer = 0f;
        for (int i = 0; i < choiceTextBoxes.Length; i++)
        {
            if (choiceTextBoxes[i].gameObject.activeInHierarchy)
            {
                BurstTextbox(choiceTextBoxes[i]);
                Debug.Log(choiceTextBoxes[i].gameObject);
                Debug.Log(burstClones[burstClones.Count - 1]);
            }
            choiceTextBoxes[i].gameObject.SetActive(false);
            choiceTextBoxes[i].GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    //Progress text writing and watch for pauses
    void TypeWriter()
    {
        if (inlinePauseTimer <= 0f)
        {
            float speedModifier = 1f;
            //Slow down if encountering punctuation
            if (currentCharacterIndex < displayText.Length - 1 && currentCharacterIndex > 2)
            {
                char c = displayText[Mathf.FloorToInt(currentCharacterIndex) - 2];
                if (c == '.')
                {
                    speedModifier = periodSpeedModifier;
                }
                else if (c == ',' || c == '?' || c == '!' || c == ':')
                {
                    speedModifier = otherPunctuationSpeedModifier;
                }
            }
            currentCharacterIndex += currentTextSpeed * Time.deltaTime * speedModifier;
            currentCharacterIndex = Mathf.Clamp(currentCharacterIndex, 0f, displayText.Length);
        }
        else
        {
            inlinePauseTimer -= Time.deltaTime;
        }

        //Looking for inline pause
        if (displayText.Substring(0,
        Mathf.Min(Mathf.FloorToInt(currentCharacterIndex) + 5, displayText.Length)).Contains("\\pause"))
        {
            currentCharacterIndex = displayText.IndexOf("\\pause");
            inlinePauseTimer = inlinePauseLength;
            displayText = displayText.Replace("\\pause", "");
        }

        if (displayText.Length - currentCharacterIndex >= "\\pause".Length &&
        displayText.Substring(Mathf.FloorToInt(currentCharacterIndex + 1), "\\pause".Length).ToLower() ==
        "\\pause")
        {
            inlinePauseTimer = inlinePauseLength;
            displayText = displayText.Replace("\\pause", "");
        }
    }

    //Only happens if not currently in dialogue
    public void TryStartDialogue(TextAsset textAsset = null, string knotName = "", NPCOverworld interlocutor = null)
    {
        if (!inDialogue)
        {
            StartDialogue(textAsset, knotName, interlocutor);
        }
    }

    public void StartDialogue(TextAsset textAsset = null, string knotName = "", NPCOverworld interlocutor = null)
    {
        if (inDialogue)
        {
            EndDialogue();
        }

        SetDialogueBubble("standard");
        npcInterlocutor = interlocutor;

        TextAsset prevTextAsset = inkJSONAsset;
        if (textAsset == null)
        {
            textAsset = inkJSONAsset;
        }
        if (knotName == "")
        {
            knotName = currentKnotName;
        }
        if (textAsset != prevTextAsset) StartStory(textAsset);
        if (currentKnotName != "")
        {
            StartKnot(knotName);
        }
        inDialogue = true;
        if (displayText == "")
        {
            Continue();
        }
        if (ContainsTag(story.TagsForContentAtPath(currentKnotName), "ambient"))
        {
            isAmbient = true;
        }
        else
        {
            isAmbient = false;
        }

        if (ContainsTag(story.TagsForContentAtPath(currentKnotName), "color"))
        {
            Image[] images = interlocutorTextBox.GetComponentsInChildren<Image>();
            defaultTextBoxColor = images[0].color;
            changedColor = true;
            string tag = GetTag(story.TagsForContentAtPath(currentKnotName), "color");
            Color newColor;
            if (ColorUtility.TryParseHtmlString("#" + tag.Replace("color:", "").Trim(), out newColor))
            {
                foreach (Image image in images)
                {
                    newColor.a = image.color.a;
                    image.color = newColor;
                }
            }
        }
        if (ContainsTag(story.TagsForContentAtPath(currentKnotName), "outline"))
        {
            Image[] images = interlocutorTextBox.GetComponentsInChildren<Image>();
            string tag = GetTag(story.TagsForContentAtPath(currentKnotName), "outline");
            Color newColor;
            if (!changedColor)
            {
                defaultTextBoxColor = images[0].color;
            }
            changedColor = true;
            if (ColorUtility.TryParseHtmlString("#" + tag.Replace("outline:", "").Trim(), out newColor))
            {
                foreach (Image image in images)
                {
                    if (image.gameObject.name == "Outline")
                    {
                        newColor.a = image.color.a;
                        image.color = newColor;
                    }
                }
            }
        }
        //if(!isAmbient) playerInput.SwitchMap("UI");

        if (swimmer != null) swimmer.StartedDialogue(isAmbient);

        if (displayText == "" && !story.canContinue && story.currentChoices.Count <= 0)
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        if (burstAtPauses) burst = true;
        inDialogue = false;
        HideText();
        playerInput.RestoreDefaultMap();
        if (npcInterlocutor != null)
        {
            npcInterlocutor.FinishedDialogue(isAmbient);
            npcInterlocutor = null;
        }
        if (swimmer != null) swimmer.FinishedDialogue(isAmbient);
        displayText = "";

        if (changedColor)
        {
            Image[] images = interlocutorTextBox.GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                defaultTextBoxColor.a = image.color.a;
                image.color = defaultTextBoxColor;
            }
        }
        changedColor = false;
    }

    public void StartStory(TextAsset textAsset = null)
    {
        if (textAsset == null)
        {
            textAsset = inkJSONAsset;
        }
        story = new Story(textAsset.text);
        BindFunctions(story);
    }

    void StartKnot(string knotName)
    {     // A knot is a section of a story
        currentKnotName = knotName;
        story.ChoosePathString(knotName);
    }

    void Continue()
    {
        prevTags = story.currentTags;

        if (story.canContinue)
        {
            //Destroying burst clones when skipping dialogue too fast
            // TO TRY TO DO: REVERSE ANIMATION INSTEAD
            for (var i = burstCoroutines.Count - 1; i >= 0; i--)
            {
                StopCoroutine(burstCoroutines[i]);
                burstCoroutines.RemoveAt(i);
            }
            for (var i = burstClones.Count - 1; i >= 0; i--)
            {
                Destroy(burstClones[i]);
                burstClones.RemoveAt(i);
            }

            displayText = "";

            GameObject prevTextBox = textBox;

            if (ContainsTag(story.currentTags, "stayonscreen"))
            {
                GameObject lingeringBox = Instantiate(prevTextBox, prevTextBox.GetComponentInParent<Canvas>().transform);
                lingeringBox.GetComponentInChildren<Animator>().speed = 0.5f;
                lingeringBoxes.Add(lingeringBox);
                lingeringBox.GetComponent<RectTransform>().position = prevTextBox.GetComponent<RectTransform>().position;
                lingeringBox.transform.SetSiblingIndex(lingeringBoxes.Count - 1);
            }

            while (displayText == "" && story.canContinue)
            {
                displayText = story.Continue().Trim();
            }
            displayText = FindTalker(displayText); //Remove "NPC: " from display text

            if (talker == "MC")
            {
                textBox = playerTextBox;
                lineTMP = playerLineTMP;
            }
            else
            {
                bool foundName = false;
                foreach (InterlocutorBox interlocutorBox in interlocutorsTextBoxes)
                {
                    if (interlocutorBox.name == talker)
                    {
                        textBox = interlocutorBox.textBox;
                        lineTMP = textBox.GetComponentInChildren<TMP_Text>();
                        foundName = true;
                        break;
                    }
                }
                if (!foundName)
                {
                    textBox = interlocutorTextBox;
                    lineTMP = interlocutorLineTMP;
                }
            }
            if (textBox != prevTextBox)
            {
                HideText(false);
            }

            // PARSING INLINE TAGS:

            if (ContainsTag(story.currentTags, "notambient"))
            {
                isAmbient = false;
            }
            else if (ContainsTag(story.currentTags, "ambient"))
            {
                isAmbient = true;
            }

            //Get text display time length
            if (ContainsTag(story.currentTags, "time"))
            {
                string tag = GetTag(story.currentTags, "time");
                ambientTimer = float.Parse(tag.Replace(" ", "").Replace("time:", "").Trim());
            }

            //Get text speed
            currentTextSpeed = GetTextSpeed(story.currentTags);

            currentCharacterIndex = 0;

            //RN this happens even if advancing thru nothing dialogue (maybe this is ok)
            Rumble.AddRumbleWithTimeOut("Advancing Dialogue",1f,.5f);
        }
    }

    //Get speed that we are displaying text (typerwriter effect)
    float GetTextSpeed(List<string> tags)
    {
        float speed = dialogueSpeedNormal;
        if (ContainsTag(story.currentTags, "speed"))
        {
            string tag = GetTag(story.currentTags, "speed");
            tag = tag.Replace("speed:", "").Trim().ToLower();
            if (tag == "slow")
            {
                speed = dialogueSpeedSlow;
            }
            else if (tag == "fast")
            {
                speed = dialogueSpeedFast;
            }
            else if (tag == "normal")
            {
                speed = dialogueSpeedNormal;
            }
            else
            {
                speed = float.Parse(tag);
            }
        }
        return speed;
    }

    //Removes "NPC:" from text and sets talker, returns the line without talker
    string FindTalker(string text)
    {
        int i = text.IndexOf(":");
        if (i > 0 && text.Substring(i - 1, 1) != "\\")
        {
            talker = text.Substring(0, i).Trim();
            text = text.Substring(i + 1, text.Length - (i + 1)).Trim();
        }
        return text;
    }

    //Does the story or current line contain this tag?
    public bool ContainsTag(List<string> list, string tag)
    {
        if (list != null && list.Count > 0)
        {
            foreach (string s in list)
            {
                if (s.ToLower().Contains(tag.ToLower()))
                {
                    return true;
                }
            }
        }
        return false;
    }

    //Get tag from story by name
    public string GetTag(List<string> list, string tag)
    {
        if (list != null && list.Count > 0)
        {
            foreach (string s in list)
            {
                if (s.ToLower().Contains(tag.ToLower()))
                {
                    return s;
                }
            }
        }
        return "";
    }

    //Bind external functions that may be used by dialogue
    void BindFunctions(Story story)
    {
        story.BindExternalFunction("pause", (float time) =>
        {
            Pause(time);
        });
        story.BindExternalFunction("stopSinging", () =>
        {
            StopSinging();
        });
        story.BindExternalFunction("continueSinging", () =>
        {
            ContinueSinging();
        });
        story.BindExternalFunction("restartSinging", () =>
        {
            RestartSinging();
        });
        story.BindExternalFunction("toggleSingingMode", () =>
        {
            ToggleSingingMode();
        });
        story.BindExternalFunction("loadLevel", (string destinationScene) =>
        {
            LoadLevel(destinationScene);
        });
        story.BindExternalFunction("goToNextLevel", () =>
        {
            GoToNextLevel();
        });
        story.BindExternalFunction("nextBrain", () =>
        {
            NextBrain();
        });
        story.BindExternalFunction("setDialogueBubble", (string bubble) =>
        {
            SetDialogueBubble(bubble);
        });
        story.BindExternalFunction("pauseTutorial", (bool b) =>
        {
            PauseTutorial(b);
        });
        story.BindExternalFunction("finishTutorialPart", (int i) =>
        {
            FinishTutorialPart(i);
        });
        story.BindExternalFunction("goToTutorialPart", (int i) =>
        {
            GoToTutorialPart(i);
        });
        story.BindExternalFunction("switchObject", (string name, bool b) =>
        {
            SwitchObject(name, b);
        });
        story.BindExternalFunction("switchInterlocutor", (string name) =>
        {
            SwitchInterlocutor(name);
        });
        story.BindExternalFunction("makeInterlocutorIdle", () =>
        {
            MakeInterlocutorIdle();
        });
        story.BindExternalFunction("overrideRotation", (string targetName) =>
        {
            OverrideRotation(targetName);
        });
        story.BindExternalFunction("overrideRotationWithSpeed", (string targetName, float rotationSpeed) =>
        {
            OverrideRotation(targetName, rotationSpeed);
        });
        story.BindExternalFunction("changeDialogueView", (int index) =>
        {
            ChangeView(index);
        });
        story.BindExternalFunction("loadInt", (string name) =>
        {
            LoadInt(name);
        });
        story.BindExternalFunction("loadBool", (string name) =>
        {
            LoadBool(name);
        });
        story.BindExternalFunction("loadString", (string name) =>
        {
            LoadString(name);
        });
        story.BindExternalFunction("loadFloat", (string name) =>
        {
            LoadFloat(name);
        });
        story.BindExternalFunction("saveValue", (string name, object value) =>
        {
            SaveValue(name, value);
        });
        story.BindExternalFunction("fadeIn", (float time) =>
        {
            FadeIn(time);
        });
        story.BindExternalFunction("fadeOut", (float time) =>
        {
            FadeOut(time);
        });
        story.BindExternalFunction("setFMODGlobalParameter", (string name, float value) =>
        {
            SetFMODGlobalParameter(name, value);
        });
        story.BindExternalFunction("changeDesire", (string text) =>
        {
            ChangeDesire(text);
        });
        story.BindExternalFunction("clearScreen", () =>
        {
            ClearScreen();
        });
        story.BindExternalFunction("changeStartKnot", (string name) =>
        {
            ChangeStartKnot(name);
        });
        story.BindExternalFunction("activateBorder", (string name, bool b) =>
        {
            ActivateBorder(name, b);
        });
        story.BindExternalFunction("triggerMetamorphosis", () =>
        {
            TriggerMetamorphosis();
        });
        story.BindExternalFunction("triggerMetamorphosisObject", (string name) =>
        {
            TriggerMetamorphosis(name);
        });
        // Play Sound Effect
        story.BindExternalFunction("playOneShot", (string path, float volume) =>
        {
            Sound.PlayOneShotVolume(path, volume);
        });
        story.BindExternalFunction("playInstance", (string path, string name, float volume) =>
        {
            Sound.PlayInstance(path,name,volume);
        }); 
        story.BindExternalFunction("setInstanceParameter", (string name, string parameterName,float parameterValue, bool ignoreSeekSpeed) =>
        {
            Sound.SetInstanceParameter(name,parameterName,parameterValue,ignoreSeekSpeed);
        }); 
        story.BindExternalFunction("setInstanceVolume", (string name, float volume) =>
        {
            Sound.SetInstanceVolume(name,volume);
        }); 
        story.BindExternalFunction("stopInstance", (string name, bool release) =>
        {
            Sound.StopInstance(name, release);
        }); 
        story.BindExternalFunction("addRumbleWithTimeout", (float leftMotorIntensity, float rightMotorIntensity, float modifier,float time) =>
        {
            Rumble.AddRumbleWithTimeOut(leftMotorIntensity,rightMotorIntensity,modifier,time);
        }); 
        story.BindExternalFunction("addSavedRumbleWithTimeout", (string rumbleName, float modifier,float time,bool isFadingOut) =>
        {
            Rumble.AddRumbleWithTimeOut(rumbleName,modifier,time,isFadingOut);
        }); 

        /* Template
        story.BindExternalFunction("FUNCTIONNAME", () =>
        {
            FUNCTIONNAME();
        }); 
        */
    }

    // EXTERNAL FUNCTIONS

    void Pause(float time)
    {
        pauseTimer = time;
        Debug.Log("start pause");
        if (burstAtPauses) burst = true;
        HideText();
    }

    void StopSinging()
    {
        if (npcInterlocutor != null)
        {
            npcInterlocutor.GetComponent<NPCSinging>().StopSinging();
        }
    }

    void ContinueSinging()
    {
        if (npcInterlocutor != null)
        {
            npcInterlocutor.GetComponent<NPCSinging>().ContinueSinging();
        }
    }

    void RestartSinging()
    {
        if (npcInterlocutor != null)
        {
            npcInterlocutor.GetComponent<NPCSinging>().RestartSinging();
        }
    }

    void ToggleSingingMode()
    {
        if (npcInterlocutor != null)
        {
            npcInterlocutor.GetComponent<NPCSinging>().ToggleSingingMode();
        }
    }

    void LoadLevel(string destinationScene)
    {
        FindObjectOfType<LevelLoader>().LoadLevel(destinationScene);
    }

    void GoToNextLevel()
    {
        if (npcInterlocutor != null)
        {
            FindObjectOfType<LevelLoader>().LoadLevel();
        }
    }

    void NextBrain()
    {
        NPCSequencer npcSequencer;
        if (npcInterlocutor != null && npcInterlocutor.transform.parent.TryGetComponent<NPCSequencer>(out npcSequencer))
        {
            npcSequencer.NextBrain();
        }
    }

    void SetDialogueBubble(string bubble)
    {
        bool isActive = interlocutorTextBox.activeInHierarchy;
        interlocutorTextBox.SetActive(false);
        switch (bubble.ToLower())
        {
            case "standard":
                interlocutorTextBox = standardTextBox;
                break;
            case "floral":
                interlocutorTextBox = floralTextBox;
                break;
            case "bone":
                interlocutorTextBox = boneTextBox;
                break;
        }
        interlocutorLineTMP = interlocutorTextBox.GetComponentInChildren<TMP_Text>();
        interlocutorTextBox.SetActive(isActive);
    }

    void PauseTutorial(bool b)
    {
        FindObjectOfType<Tutorial>().PauseTutorial(b);
    }

    void FinishTutorialPart(int i)
    {
        FindObjectOfType<Tutorial>().FinishTutorialPart(i);
    }

    void GoToTutorialPart(int i)
    {
        FindObjectOfType<Tutorial>().GoToTutorialPart(i);
    }


    void SwitchObject(string name, bool b)
    {
        gameManager.SwitchObject(name, b);
    }

    void SwitchInterlocutor(string name)
    {
        GameObject g = gameManager.FindObject(name);
        NPCOverworld newInterlocutor;
        if (g != null && g.TryGetComponent<NPCOverworld>(out newInterlocutor))
        {
            if (npcInterlocutor != null)
            {
                npcInterlocutor.FinishedDialogue(isAmbient);
            }
            npcInterlocutor = newInterlocutor;
            npcInterlocutor.DialogueStarted();
        }
        else if (g == null)
        {
            Debug.Log("Tried switching interlocutor & couldn't find gameobject");
        }
        else
        {
            Debug.Log("Tried switching interlocutor & couldn't find NPCOverworld");
        }
    }

    void MakeInterlocutorIdle()
    {
        if (npcInterlocutor != null)
        {
            npcInterlocutor.BecomeIdle();
        }
    }

    void OverrideRotation(string targetName, float rotationSpeed = -1f)
    {
        GameObject g = gameManager.FindObject(targetName);
        if (g != null)
        {
            swimmer.OverrideRotation(g.transform, rotationSpeed);
        }
        else
        {
            Debug.Log("Tried overriding rotation & couldn't find gameobject");
        }
    }

    virtual public void ChangeView(int i)
    {
        Debug.Log("Change view dialogue");
        views[currentViewIndex].SetActive(false);
        currentViewIndex = Mathf.Clamp(i - 1, 0, views.Length);
        views[currentViewIndex].SetActive(true);

        DialogueView dialogueView = views[currentViewIndex].GetComponent<DialogueView>();
        interlocutorTextBox = dialogueView.interlocutorTextBox;
        playerTextBox = dialogueView.playerTextBox;
        standardTextBox = dialogueView.standardTextBox;
        boneTextBox = dialogueView.boneTextBox;
        floralTextBox = dialogueView.floralTextBox;
        interlocutorsTextBoxes = dialogueView.interlocutorsTextBoxes;

        choiceTextBoxes = dialogueView.choiceTextBoxes;

        SetUpView();

        HideText();
    }

    void LoadInt(string name)
    {
        object value = DialogueValues.Instance.LoadVariable(name);
        if (value == null) value = PlayerPrefs.GetInt(name);
        if (value != null)
        {
            story.variablesState[name] = (int)value;
        }

    }

    void LoadFloat(string name)
    {
        object value = DialogueValues.Instance.LoadVariable(name);
        if (value == null) value = PlayerPrefs.GetFloat(name);
        if (value != null)
        {
            story.variablesState[name] = (float)value;
        }
    }

    void LoadString(string name)
    {
        object value = DialogueValues.Instance.LoadVariable(name);
        if (value == null) value = PlayerPrefs.GetString(name);
        if (value != null)
        {
            story.variablesState[name] = (string)value;
        }
    }

    void LoadBool(string name)
    {
        object value = DialogueValues.Instance.LoadVariable(name);
        if (value != null)
        {
            story.variablesState[name] = (bool)value;
        }
    }

    void SaveValue(string name, object value)
    {
        DialogueValues.Instance.SaveVariable(name, value);
    }

    void FadeIn(float time = -1f)
    {
        FindObjectOfType<LevelLoader>().FadeIn(time);
    }

    void FadeOut(float time = -1f)
    {
        FindObjectOfType<LevelLoader>().FadeOut(time);
    }

    void SetFMODGlobalParameter(string name, float value)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(name, value);
    }

    void ChangeDesire(string text)
    {
        FindObjectOfType<PauseMenu>().ChangeDesire(text);
    }

    void ClearScreen()
    {
        foreach (GameObject lingeringBox in lingeringBoxes)
        {
            if (burstAfterEveryLine) BurstTextbox(lingeringBox);
            Destroy(lingeringBox);
        }
        lingeringBoxes.Clear();
    }

    void ChangeStartKnot(string knotName)
    {
        if (npcInterlocutor != null)
        {
            npcInterlocutor.knotName = knotName;
        }
    }

    void ActivateBorder(string name, bool b)
    {
        ScreenBorders sb = FindObjectOfType<ScreenBorders>();
        if (sb != null) sb.ActivateBorder(name, b);
    }

    void TriggerMetamorphosis(string targetName="")
    {
        GameObject obj=null;
        if (targetName != "")
        {
            obj = gameManager.FindObject(targetName);
        }else if (npcInterlocutor != null){
            obj=npcInterlocutor.gameObject;
        }
        if (obj!=null)
        {
            Metamorphosis[] metamorphoses = obj.GetComponentsInChildren<Metamorphosis>();
            foreach (Metamorphosis m in metamorphoses)
            {
                m.TriggerMetamorphosis();
            }
        }
        else
        {
            Debug.LogWarning("Tried metamorphosis but failed");
        }
    }


    void Oestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }        
    }

}

[System.Serializable]
public struct InterlocutorBox{
    [Tooltip("Name of NPC to whom to assign this text box to")]
    public string name;
    [Tooltip("Textbox game object for this interlocutor")]
    public GameObject textBox;
}
