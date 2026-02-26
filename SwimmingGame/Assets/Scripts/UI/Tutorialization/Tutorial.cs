using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public TutorialPart[] tutorialParts;

    public int index=0;

    private float timer=0f;

    private float targetOpacity=0f;
    [HideInInspector]
    public float opacity=0f;
    public float opacityFadeinSpeed=10f;
    public float opacityFadeoutSpeed=1f;

    private TutorializationIcon[] icons;
    private TMP_Text tMPro;

    [HideInInspector]
    public bool currentlyUsed=false; //For when the button we want to use is currently being used

    private bool hasEnteredTrigger;
    private Collider enteredTrigger, exitedTrigger;
    private SwimmerSinging swimmerSinging;

    [HideInInspector]
    public bool paused=false;


    void Start()
    {
        for(var i=0;i<tutorialParts.Length;i++){
            tutorialParts[i].active=false;
            tutorialParts[i].triggered=false;
            if(tutorialParts[i].canvasParent!=null) tutorialParts[i].canvasParent.SetActive(false);
        }

        swimmerSinging=FindObjectOfType<SwimmerSinging>();
    }

    void Update()
    {
        if(enteredTrigger!=null){
            bool delete=true;
            for(var i=0;i<tutorialParts.Length;i++){
                if(i!=index && tutorialParts[i].triggerZone==enteredTrigger){
                    if(!tutorialParts[i].done){
                        GoToTutorialPart(i);
                        delete=false;
                        Debug.Log("entered trigger and skip");
                    }
                    break;
                }else if(i==index){
                    delete=false;
                }
            }
            if(delete){
                enteredTrigger=null;
            }
        }

        if(index<tutorialParts.Length){
            TutorialPart currentTutorialPart=tutorialParts[index];
            icons=currentTutorialPart.canvasParent.GetComponentsInChildren<TutorializationIcon>();
            tMPro=currentTutorialPart.canvasParent.GetComponentInChildren<TMP_Text>();

            if(!currentTutorialPart.active){
                if(currentTutorialPart.triggerZone!=null && currentTutorialPart.triggerZone==enteredTrigger){
                    hasEnteredTrigger=true;
                    Debug.Log("entered trigger and move on");
                }
                if(hasEnteredTrigger || currentTutorialPart.triggerZone==null){
                    timer+=Time.deltaTime;
                }
                enteredTrigger=null;
                if(timer>=currentTutorialPart.delay){
                    hasEnteredTrigger=false;
                    timer=0f;
                    currentTutorialPart.canvasParent.SetActive(true);
                    opacity=0f;
                    targetOpacity=1f;
                    icons=currentTutorialPart.canvasParent.GetComponentsInChildren<TutorializationIcon>();
                    tMPro=currentTutorialPart.canvasParent.GetComponentInChildren<TMP_Text>();
                    
                    Color c=tMPro.color;
                    c.a=opacity;
                    tMPro.color=c;

                    Image[] images=currentTutorialPart.canvasParent.GetComponentsInChildren<Image>();
                    foreach(Image image in images){
                        image.color=c;
                    }

                    tutorialParts[index].active=true; //IMPORTANT: i think currentTutorialPart is essentially a copy so we gotta change this at the root
                }
            }else{
                if(!paused){
                    //Deal with harmony special icon
                    if(currentTutorialPart.isHarmony){
                        foreach(TutorializationIcon icon in icons){
                            icon.active=false;
                        }
                        float minDistance=100f;
                        string note="";
                        NPCSinging[] npcSingings=FindObjectsOfType<NPCSinging>();
                        foreach(NPCSinging npcSinging in npcSingings){
                            if(npcSinging.canSing){
                                float distance=Vector3.Distance(swimmerSinging.transform.position,npcSinging.transform.position);
                                if(npcSinging.InRange() && distance<minDistance){
                                    minDistance=distance;
                                    if(npcSinging.singing){
                                        note=npcSinging.singingNote;
                                        if(npcSinging.isHarmonizing()){
                                            currentlyUsed=true;
                                        }
                                    }
                                }
                            }
                        }
                        if(note=="A4"){
                            icons[0].active=true;
                        }else if(note=="B4"){
                            icons[1].active=true;
                        }else if(note=="C#5"){
                            icons[2].active=true;
                        }else if(note=="D#5"){
                            icons[3].active=true;
                        }else if(note=="G#5"){
                            icons[4].active=true;
                        }

                        string prevText=tMPro.text;
                        
                        if (note != "")
                        {
                            tMPro.text="To harmonize, <br>sing one of the notes <br>across your singing partner's.";
                        }
                        else if (minDistance < 100f)
                        {
                            tMPro.text="To harmonize, <br>wait for your partner<br> to start singing.";
                        }
                        else
                        {
                            tMPro.text="To harmonize, <br>get close to a singing <br>coralnet or a person.";
                        }

                        if (prevText != tMPro.text)
                        {
                            opacity=0f;
                        }
                    }

                    if((currentlyUsed || currentTutorialPart.disappearsAutomatically) && targetOpacity>0f){
                        timer+=Time.deltaTime;
                        //targetOpacity=Mathf.Clamp(1f-timer/currentTutorialPart.timeBeforeDisappearing,0f,1f);
                        if(timer>=currentTutorialPart.timeBeforeDisappearing){
                            targetOpacity=0f;
                        }
                    }else if(targetOpacity==0f){
                        timer+=Time.deltaTime;
                        if(currentlyUsed && !currentTutorialPart.skipEvenIfUsing){
                            timer=0f;
                        }
                        if(!currentTutorialPart.disappearsAutomatically && timer>=currentTutorialPart.timeBeforeReappearing){
                            targetOpacity=1f;
                        }
                    }
                }else{
                    targetOpacity=0f;
                }

                if(tutorialParts[index].done){
                    targetOpacity=0f;
                }

                float opacityLerpSpeed=opacityFadeinSpeed;
                if(opacity>targetOpacity){
                    opacityLerpSpeed=opacityFadeoutSpeed;
                }
                opacity=Mathf.Lerp(opacity,targetOpacity,opacityLerpSpeed*Time.deltaTime);
                Color c=tMPro.color;
                c.a=opacity;
                tMPro.color=c;

                if(!tutorialParts[index].done){
                    tutorialParts[index].done=IsDone(index);
                }

                if(tutorialParts[index].done && opacity<=0.05f){
                    NextTutorialPart();
                }

            }
        }

        currentlyUsed=false;

        if(Input.GetKeyDown(KeyCode.T) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))){
            PauseTutorial(!paused);
        }
        
    }

    bool IsDone(int tutorialPartIndex){
        bool done=false;
        TutorialPart currentTutorialPart=tutorialParts[index];
        if(((timer>=currentTutorialPart.timeBeforeDisappearing && currentTutorialPart.disappearsAfterTime) ||
            (currentTutorialPart.disappearsAutomatically && timer>=currentTutorialPart.timeBeforeDisappearing))
            && opacity<0.1f){
            done=true;
        }else if(currentTutorialPart.disappearsAfterLeavingZone && exitedTrigger==currentTutorialPart.triggerZone){
            done=true;
        }
        exitedTrigger=null;
        return done;
    }

    public void NextTutorialPart(){
        GoToTutorialPart(index+1);
    }

    public void GoToTutorialPart(int i){
        DeactivateTutorialPart(index);
        index=i;
    }

    public void RepeatTutorialPart(int i){
        DeactivateTutorialPart(index);
        index=i;
        tutorialParts[index].done=false;
    }

    void DeactivateTutorialPart(int i){
        if(i<tutorialParts.Length){
            tutorialParts[index].active=false;
            tutorialParts[index].canvasParent.SetActive(false);
            tutorialParts[index].done=true;
        }
        hasEnteredTrigger=false;
        timer=0f;
    }

    public void EnteredTrigger(Collider other){
        enteredTrigger=other;
    }

    public void ExitedTrigger(Collider other){
        exitedTrigger=other;
    }

    public void PauseTutorial(bool b){
        paused=b;
    }

    public void FinishTutorialPart(int i){
        tutorialParts[i].done=true;
    }
}

[System.Serializable]
public struct TutorialPart{
    public float delay;
    public Collider triggerZone;
    public GameObject canvasParent;
    [Tooltip("How much time user needs to press the relevant button before the message disappears.")]
    public float timeBeforeDisappearing;

    public float timeBeforeReappearing;
    [HideInInspector]
    public bool active, triggered;
    public bool done;
    [Tooltip("Disappears after using the button for the time above.")]
    public bool disappearsAfterTime;
    public bool disappearsAfterLeavingZone; 
    [Tooltip("Disappears after \"time before disappearing\".")]
    public bool disappearsAutomatically;
    public bool isHarmony;
    [Tooltip("If this is false, the tutorial doesn't move on until you stopped using the input.")]
    public bool skipEvenIfUsing;
}
