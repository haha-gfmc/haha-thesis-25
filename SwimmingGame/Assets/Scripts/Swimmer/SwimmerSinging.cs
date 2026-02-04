using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Runtime.InteropServices;
using System.Dynamic;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class SwimmerSinging : Singing
{
    public bool canSing=false;
    private PlayerInput playerInput;

    private Vector2 singingNotePosition; // Relative position on the wheel

    private float startingAngle=1.5f;
    [HideInInspector]
    public float singingAngle;

    private Vector2 inputNote;

    private float targetOpacity=0f;
    public float imageOpacityLerpSpeed=1f;
    public float lightIntensityLerpSpeed=1f;

    public float mouseSensitivity=2f;

    public bool singing;

    private Animator animator;

    public Light singingLight;
    [Tooltip("Target intensity for point light when singing.")]
    public float singingTargetIntensity;
    public Light spotlight;
    [Tooltip("Target intensity for spotlight when singing.")]
    public float spotlightTargetIntensity=0.4f;

    public SpriteRenderer auraSprite;
    [Tooltip("Target intensity for spotlight when singing.")]
    public float auraTargetOpacity=.05f;

    private int shiftingAmount=0; //Amount by which we are shifting notes (from -2 to 2)

    [Header("UI Things")]
    [Tooltip("The dot that moves on the canvas to signify singing")]
    public GameObject[] singingDots;
    private RectTransform[] singingDotRects;
    [Tooltip("Radius of singing circle on canvas.")]
    public float circleRadius=255f;
    private Vector2 rectTargetPosition;
    private Vector2 rectCenter;
    [Tooltip("Images to affect opacity.")]
    public Image[] images;
    private float[] maxOpacities; //max opacity for each image
    public float noteLerpValue=10f;
    public Color[] wheelColors;
    public Color[] auraColors;
    public float colorLerpValue=1f;

    private float targetWheelRotation;
    [Tooltip("How much to rotate singing wheel when shifting a tone.")]
    public float wheelRotationPerTone=2.5f;
    public RectTransform wheelRect;

    private RustlingThing[] rustlingThings;


    private bool isHarmonizing=false;

    void Start()
    {
        playerInput=FindObjectOfType<PlayerInput>();
        singingDotRects=new RectTransform[singingDots.Length];
        for(int i=0;i<singingDots.Length;i++){
            singingDotRects[i]=singingDots[i].GetComponent<RectTransform>();
        }
        rectTargetPosition=singingDotRects[0].anchoredPosition;
        rectCenter=singingDotRects[0].anchoredPosition;

        maxOpacities=new float[images.Length];
        for(int i=0;i<images.Length;i++){
            Image image=images[i];
            Color c=image.color;
            maxOpacities[i]=c.a;
            image.color=new Color(c.r,c.g,c.b,0f);
        }

        SingingStart();

        animator=GetComponentInParent<Animator>();

        rustlingThings=FindObjectsOfType<RustlingThing>();

    }

    void Update()
    {
        if(canSing){
            
            if(playerInput.shiftLeft && !playerInput.shiftRight && !playerInput.shiftTwice){
                shiftingAmount=-1;
            }else if((playerInput.shiftLeft && !playerInput.prevShiftLeft && playerInput.shiftRight)
             || (playerInput.shiftRight && playerInput.shiftTwice && !playerInput.shiftLeft)){
                shiftingAmount=2;
            }else if(playerInput.shiftRight && !playerInput.shiftLeft && !playerInput.shiftTwice){
                shiftingAmount=1;
            }else if((playerInput.shiftRight && !playerInput.prevShiftRight && playerInput.shiftLeft)
             || (playerInput.shiftLeft && playerInput.shiftTwice && !playerInput.shiftRight)){
                shiftingAmount=-2;
            }else if(!playerInput.shiftRight && !playerInput.shiftLeft){
                shiftingAmount=0;
            }

            targetWheelRotation=wheelRotationPerTone*shiftingAmount;

            if(playerInput.currentControlScheme=="Gamepad"){
                inputNote=playerInput.singingNote;
                if(inputNote.magnitude>0f){
                    singing=true;
                }else{
                    singing=false;
                }
            }else if(playerInput.currentControlScheme=="Keyboard&Mouse" && playerInput.singing){
                inputNote=playerInput.singingNote*Time.deltaTime*mouseSensitivity+inputNote;
                singing=true;
            }else{
                inputNote=Vector2.zero;
                singing=false;
            }

            inputNote=Vector2.ClampMagnitude(inputNote,1f);
            
            singingNotePosition=Vector2.Lerp(singingNotePosition,inputNote,noteLerpValue*Time.deltaTime);
            foreach(RectTransform singingDotRect in singingDotRects){
                singingDotRect.anchoredPosition=rectCenter+singingNotePosition*circleRadius;
            }

            singingVolume=singingNotePosition.magnitude;

            singingAngle=Mathf.Atan2(singingNotePosition.y,singingNotePosition.x)/Mathf.PI+1;

            string note="";
            int keyIndex=0;

            for(var i=0;i<possibleNotes.Count;i++){
                float minAngle=(startingAngle-((i*2f+1f)/possibleNotes.Count)+2f)%2f;
                float maxAngle=(startingAngle-((i*2f-1f)/possibleNotes.Count)+2f)%2f;
                if((singingAngle<maxAngle && singingAngle>=minAngle) ||
                (singingAngle<maxAngle && singingAngle>=0 && maxAngle<minAngle) || (singingAngle<=2 && singingAngle>=minAngle && maxAngle<minAngle)){
                    note=possibleNotes[i];
                    keyIndex=keys.IndexOf(note);
                    //Finding correct note if shifting
                    if(shiftingAmount!=0){
                        if(keyIndex!=-1){
                            keyIndex=keyIndex+shiftingAmount;
                            if(keyIndex>=0 && keyIndex<keys.Count){
                                note=keys[keyIndex];
                            }
                        }
                    }
                    break;
                }
            }

            if(singingNote!=note){
                if(singingNote!=""){
                    StopNote(singingNote);
                }
                if(note!=""){
                    PlayNote(note);
                }
                singingNote=note;
            }

            SingingUpdate();

            if(singing){
                targetOpacity=1f;
                animator.SetBool("singing",true);
            }else{
                targetOpacity=0f;
                animator.SetBool("singing",false);
            }

            Color c;

            for(int i=0;i<images.Length;i++){
                Image image=images[i];
                c=image.color;
                float a=Mathf.Lerp(c.a,targetOpacity*maxOpacities[i],imageOpacityLerpSpeed*Time.deltaTime);
                image.color=new Color(c.r,c.g,c.b,a);
            }

            singingLight.intensity=Mathf.Lerp(singingLight.intensity,singingTargetIntensity*targetOpacity*singingVolume,lightIntensityLerpSpeed*Time.deltaTime);

            if(spotlight!=null){
                spotlight.intensity=Mathf.Lerp(spotlight.intensity,spotlightTargetIntensity*targetOpacity*singingVolume,lightIntensityLerpSpeed*Time.deltaTime);
            }
            
            c=auraSprite.color;
            c.a=Mathf.Lerp(c.a,auraTargetOpacity*singingVolume,lightIntensityLerpSpeed*Time.deltaTime);
            auraSprite.color=c;

            //Changing color depending on shifting note amount
            Color targetWheelColor=Color.Lerp(wheelColors[1],wheelColors[Mathf.Clamp(1+shiftingAmount,0,wheelColors.Length-1)],Mathf.Abs(shiftingAmount)/2f);
            Color targetAuraColor=Color.Lerp(auraColors[1],auraColors[Mathf.Clamp(1+shiftingAmount,0,wheelColors.Length-1)],Mathf.Abs(shiftingAmount)/2f);

            targetWheelColor.a=images[0].color.a;
            targetAuraColor.a=images[1].color.a;

            images[0].color=Color.Lerp(images[0].color,targetWheelColor,colorLerpValue*Time.deltaTime);   //Changing wheel color
            images[1].color=Color.Lerp(images[1].color,targetAuraColor,colorLerpValue*Time.deltaTime);   //Changing wheel color

            Vector3 rot=wheelRect.rotation.eulerAngles;
            rot.z=targetWheelRotation;
            wheelRect.rotation=Quaternion.Lerp(wheelRect.rotation,Quaternion.Euler(rot),colorLerpValue*Time.deltaTime);

            if(singing){
                //Rumble.AddRumble("Singing",singingVolume);
                Rumble.AddRumble("Singing Left",singingVolume*(Mathf.Cos(singingAngle*Mathf.PI)+1)/2);
                Rumble.AddRumble("Singing Right",singingVolume*(1-Mathf.Cos(singingAngle*Mathf.PI)+1)/2);
            }
            if(singing && isHarmonizing){
                Rumble.AddRumble("Harmonizing");
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("harmonizing",1f);
            }else{
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("harmonizing",0f);
            }

        }

        isHarmonizing=false;

    }

    public void Harmonizing(){
        isHarmonizing=true;
    }

    public void Harmonized(){
        if(harmonyCompleteAnimator!=null) harmonyCompleteAnimator.SetTrigger("complete");
    }

}
