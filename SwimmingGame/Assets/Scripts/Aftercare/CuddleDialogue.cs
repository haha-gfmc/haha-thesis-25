using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuddleDialogue : Dialogue
{
    [Header("Cuddle Specific")]
    public bool caressing;
    [Tooltip("If true, the camera moves with the arm.")]
    public bool traveling=false;
    [Tooltip("If true, the control is moving the MC's body not hand.")]
    public bool moving=false;
    public float caressRequiredLength=1f;
    public float caressTimer=0f;
    public float caressCancelSpeed=0.5f;

    public float opacitySpeed=1f;
    public float notSelectedOpacity=0.625f;
    public float hoveredOpacity=0.9f;
    public float chosenOpacity=1f;
    public float hoveredScale=1.1f;

    public float offscreenBoxScale=.75f;

    private RectTransform[] choiceRects;
    private Vector3[] choiceRectScales;

    private int prevChoiceIndex=-1;

    private bool justHovered = false;
    private bool justCaressed = false;

    private float selectingIntensity=1f;

    private int[] choiceBoxIndexes;
    private bool differentChoiceBoxOrder; //If true, the choice boxes are assigned to specific ones in the world instead of in hierarchical order.
    public Transform[] choiceCollisionBoxes;


    public override void DialogueAwake()
    {
        base.DialogueAwake();

        if(!traveling) ChangeView(currentViewIndex);
        else{
            choiceRects=new RectTransform[choiceTextBoxes.Length];
            choiceRectScales=new Vector3[choiceTextBoxes.Length];
            for(var k=0;k<choiceTextBoxes.Length;k++){
                choiceRects[k]=choiceTextBoxes[k].GetComponent<RectTransform>();
                choiceRectScales[k]=choiceRects[k].localScale;
            }
        }
    }

    public override void DialogueUpdate()
    {
        base.DialogueUpdate();

    }

    void LateUpdate(){
        if (caressing)
        {
            caressTimer += Time.unscaledDeltaTime*selectingIntensity;
            Rumble.AddRumble("Picking Dialogue",caressTimer/caressRequiredLength);
            Rumble.AddRumble("Singing",caressTimer/caressRequiredLength);
        }

        if (caressTimer >= caressRequiredLength && story.currentChoices.Count > 0 &&
            currentChoiceIndex <= story.currentChoices.Count && currentChoiceIndex >= 0)
        {
            if(moving){
                FindObjectOfType<MoveBody>().PickedChoice(currentChoiceIndex);
                GameObject cb=choiceTextBoxes[choiceTextBoxes.Length-1];
                choiceTextBoxes[choiceTextBoxes.Length-1]=choiceTextBoxes[currentChoiceIndex];
                choiceTextBoxes[currentChoiceIndex]=cb;
                ChangeView(currentViewIndex);
            }
            PickChoice(currentChoiceIndex);
        }
        else if (story.currentChoices.Count == 0)
        {
            caressTimer = 0f;
            currentChoiceIndex = -1;
        }

        caressTimer -= Time.deltaTime * caressCancelSpeed / caressRequiredLength;
        caressTimer = Mathf.Clamp(caressTimer, 0f, caressRequiredLength);

        if (justHovered)
        {
            justHovered = false;
        }
        else {
            currentChoiceIndex = -1;
        }
        if (justCaressed)
        {
            justCaressed = false;
        }
        else caressing = false;
        //Debug.Log("current choice index = "+currentChoiceIndex);
    }

    public void HoveringChoice(int choiceIndex){
        if(differentChoiceBoxOrder){
            // Find index of this choice box depending on order dictated in dialogue file
            for(var i=0;i<choiceBoxIndexes.Length;i++){
                if(choiceBoxIndexes[i]==choiceIndex){
                    currentChoiceIndex=i;
                    break;
                }
            }
        }
        else currentChoiceIndex=choiceIndex;
        if(currentChoiceIndex!=prevChoiceIndex){
            //caressTimer=0f;
        }
        prevChoiceIndex=currentChoiceIndex;
        justHovered=true;
    }

    public void SelectingChoice(int choiceIndex, float intensity=1f){
        caressing=true;
        justCaressed=true;
        selectingIntensity=intensity;
    }


    public override void ShowChoices()
    {
        choiceBoxIndexes=new int[story.currentChoices.Count];
        for(var i=0;i<choiceBoxIndexes.Length;i++){
            choiceBoxIndexes[i]=i;
        }

        for(int i=0;i<Mathf.Min(story.currentChoices.Count,choiceTextBoxes.Length);i++){


            if(!choiceTextBoxes[i].gameObject.activeInHierarchy){
                choiceTextBoxes[i].gameObject.SetActive(true);
                choiceTextBoxes[i].GetComponentInChildren<Animator>().StartPlayback();
                choiceTextBoxes[i].GetComponentInChildren<Animator>().Play("Base Layer.smallDialogueBubbleIdle",-1,Random.Range(0f,1f));
                choiceTextBoxes[i].GetComponentInChildren<Animator>().speed=1f;
            }
            
            choiceTMPs[i].text=story.currentChoices[i].text;

            DialogueBoxPlacement dbp=choiceTextBoxes[i].GetComponent<DialogueBoxPlacement>();

            if(ContainsTag(story.currentChoices[i].tags,"place")){
                string tag=GetTag(story.currentChoices[i].tags,"place");
                int index=int.Parse(tag.Replace("place:","").Trim());
                choiceBoxIndexes[i]=index;
                differentChoiceBoxOrder=true;

            }

            float targetS=1f;
            float targetA=1f;
            if(i==currentChoiceIndex){
                targetA=hoveredOpacity;
                targetS+=hoveredScale-1f;
                if(caressing){
                    float k=EaseOutSine(caressTimer/caressRequiredLength);
                    targetS=targetS+(chosenScale-hoveredScale)*k;
                    targetA+=k*(chosenOpacity-hoveredOpacity);
                }
                choiceTextBoxes[i].GetComponentInChildren<Animator>().speed=1.25f;
            }else{
                targetA=notSelectedOpacity;
                choiceTextBoxes[i].GetComponentInChildren<Animator>().speed=0.5f;
            }

            Image[] images=choiceTextBoxes[i].GetComponentsInChildren<Image>();
            for(var k=0;k<images.Length;k++){
                Color c=images[k].color;
                c.a=Mathf.Lerp(c.a/defaultChoiceBoxOpacites[k],targetA,opacitySpeed*Time.deltaTime)
                    *defaultChoiceBoxOpacites[k];
                images[k].color=c;
            }

            if(dbp!=null){
                if(dbp.isOffscreen){
                    targetS=offscreenBoxScale;
                }
                dbp.overrideTarget=choiceCollisionBoxes[choiceBoxIndexes[i]];
            }

            Vector3 localScale=choiceRects[i].localScale;
            float s=Mathf.Lerp(localScale.x,targetS*choiceRectScales[i].x,scaleSpeed*Time.deltaTime);
            localScale=new Vector3(s,s,s);
            choiceRects[i].localScale=localScale;

        }

    }

    public override void HideChoices()
    {
        differentChoiceBoxOrder=false;

        base.HideChoices();

        for(int i=0;i<choiceTextBoxes.Length;i++){
            choiceRects[i].localScale=Vector3.one;
            Image[] images=choiceTextBoxes[i].GetComponentsInChildren<Image>();
            for(var k=0;k<images.Length;k++){
                Color c=images[k].color;
                c.a=notSelectedOpacity*defaultChoiceBoxOpacites[k];
                images[k].color=c;
            }
            if(choiceRectScales!=null){
                choiceRects[i].localScale=choiceRectScales[i];
            }
        }
    }

    public override void ChangeView(int i){
        if(choiceRectScales!=null && choiceRectScales.Length>0){
            for(var k=0;k<choiceTextBoxes.Length;k++){
                choiceRects[k].localScale=choiceRectScales[k];
            }
        }

        base.ChangeView(i);

        Debug.Log("Change view cuddle");

        choiceRects=new RectTransform[choiceTextBoxes.Length];
        choiceRectScales=new Vector3[choiceTextBoxes.Length];
        for(var k=0;k<choiceTextBoxes.Length;k++){
            choiceRects[k]=choiceTextBoxes[k].GetComponent<RectTransform>();
            choiceRectScales[k]=choiceRects[k].localScale;
        }

        FindObjectOfType<CuddleCameraManager>().shotIndex=currentViewIndex;

        SetUpView();
    }


}
