using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Cinemachine;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SwimmerCamera : MonoBehaviour
{
    public bool cameraControl=false;
    public Camera camera;
    public Transform target;

    private PlayerInput playerInput;
    private Swimmer swimmer;

    public float rotationSmoothTime = 1;  // Smoothing factor for the camera movement
    public float maxRotationationAngle=45;
    public float rotationSpeed=60f;
    [Tooltip("If there is no input from player for this long, camera moves back to player.")]
    public float pauseLength=2f;
    private float pauseTimer=0f;
    
    public float boostEffectChangeSpeed=2f;
    public float boostEffectRestoreSpeed=2f;
    public float targetFov=80f;
    [Tooltip("FOV to change to when boosting")]
    public float boostFov=90f;
    [Tooltip("Lerp the FOV to this when swimming slowly.")]
    public float slowFov=60f;
    [Tooltip("At this velocity start attenuating effects such as FOV change.")]
    public float minVelocityToAttenuateEffects=7f;
    [Tooltip("At this velocity stop effects such as FOV change.")]
    public float maxVelocityToAttenuateEffects=10f;
    private float baseFov;
    private Vector3 targetRotation;
    private Vector3 rotationVelocity;

    [Header("Post-Processing")]

    private VolumeProfile profile;

    private ChromaticAberration chromaticAberration;
    private bool hasChromaticAberration=false;
    private float chromaticAberrationBaseIntensity;
    public float chromaticAberrationIntensityChange=0.2f;
    private float chromaticAberrationTargetIntensity;
    

    private PaniniProjection paniniProjection;
    private bool hasPanini=false;
    private float paniniBaseIntensity;
    public float paniniIntensityChange=0.2f;
    private float paniniTargetIntensity;

    CinemachineVirtualCamera cinemachineVirtualCamera;
    private bool restoreDampingTrigger=false;
    private Vector3 savedDamping;

    private Animator animator;
    [Header("Music")]

    public bool danceToMusic=false;
    public float beatFovChange=5f;
    [Tooltip("Do FOV change every (this number) beats.")]
    public float beatFrequency=1f;
    private MusicBeat musicBeat;
    private PartyMusic partyMusic;

    [Header("Shockwave")]
    public ShockwavePulseTrigger shockwavePulseTrigger;
    [Tooltip("At this velocity start attenuating shockwave.")]
    public float shockwaveAttenuationMinVelocity=3f;
    [Tooltip("At this velocity stop start attenuating shockwave.")]
    public float shockwaveAttenuationMaxVelocity=7f;



    // FAILED ATTEMPT to be able to change what parts of the volume get changed via inspector

    // [Serializable]
    // public struct PostProcessingValueToChange{
    //     [Tooltip("For e.g.: ChromaticAberration")]
    //     public string componentName;
    //     [Tooltip("For e.g.: intensity")]
    //     public PostProcessingPropertyNames propertyName;
    //     public float valueChange;
    //     [System.NonSerialized]
    //     public float baseValue;
    //     [System.NonSerialized]
    //     public float targetValue;
    //     [System.NonSerialized]
    //     public VolumeComponent component;
    //     [System.NonSerialized]
    //     public bool hasComponent;
    // }

    // public PostProcessingValueToChange[] postProcessingValuesToChange;

    void Start()
    {
        playerInput=FindObjectOfType<PlayerInput>();
        swimmer=GetComponent<Swimmer>();
        profile=FindObjectOfType<Volume>().sharedProfile;
        animator=GetComponentInChildren<Animator>();
        if(profile.TryGet<ChromaticAberration>(out chromaticAberration)){
            hasChromaticAberration=true;
            chromaticAberrationBaseIntensity=chromaticAberration.intensity.value;
            chromaticAberrationTargetIntensity=chromaticAberrationBaseIntensity;
        }
        if(profile.TryGet<PaniniProjection>(out paniniProjection)){
            hasPanini=true;
            paniniBaseIntensity=paniniProjection.distance.value;
            paniniTargetIntensity=paniniBaseIntensity;
        }

        baseFov=camera.fieldOfView;
        targetFov=baseFov;

        CinemachineVirtualCamera cinemachineVirtualCamera=camera.GetComponent<CinemachineVirtualCamera>();

        // for(int i=0;i<postProcessingValuesToChange.Length;i++){
        //     PostProcessingValueToChange ppv=postProcessingValuesToChange[i];
        //     string s="UnityEngine.Rendering.Universal."+ppv.componentName+", UnityEngine";
        //     Debug.Log(s);
        //     Type t=GetTypeByName(ppv.componentName);
        //     Debug.Log(profile.Has(t));
        //     Debug.Log(t);
        //     if(profile.TryGet(t,out ppv.component)){
        //         Debug.Log("found "+ppv.componentName);
        //         ppv.hasComponent=true;
        //         //ppv.baseValue=(float)ppv.component.GetType().GetProperty(ppv.componentName).GetValue(ppv.component);
        //         switch(ppv.propertyName){
        //             case PostProcessingPropertyNames.intensity:
        //                 ppv.baseValue=ppv.component.intensity;
        //                 break;
        //             case PostProcessingPropertyNames.distance:
        //                 ppv.baseValue=ppv.component.distance;
        //                 break;
        //         }
        //     }else{
        //         Debug.Log("not found "+ppv.componentName);
        //     }
        // }

        if(danceToMusic){
            musicBeat=FindObjectOfType<MusicBeat>();
            partyMusic=FindObjectOfType<PartyMusic>();
        } 
    }

    public static Type GetTypeByName(string name)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.Name == name)
                    return type;
            }
        }

        return null;
    }

    void Update()
    {
        if(danceToMusic){
            if(MusicBeat.newBeat && musicBeat.timelineInfo.currentBeat%beatFrequency==0f){
                if(partyMusic.IsMuffled()){
                    targetFov+=beatFovChange*.25f;
                }
                else{
                    targetFov+=beatFovChange;
                }
            }
        }

        if(cameraControl){
            pauseTimer+=Time.deltaTime;

            Vector3 input=new Vector3(playerInput.rotation.y,playerInput.rotation.x,0f);

            if(input.magnitude>=0.05f){
                pauseTimer=0f;
            }

            targetRotation+=input*Time.deltaTime*rotationSpeed;

            if(pauseTimer>=pauseLength){
                targetRotation=Vector3.zero;
            }

            targetRotation.x=Mathf.Clamp(targetRotation.x,-maxRotationationAngle,maxRotationationAngle);
            targetRotation.y=Mathf.Clamp(targetRotation.y,-maxRotationationAngle,maxRotationationAngle);

            //Inverting current rotation values if they go over 180
            Vector3 currentRotation=target.localRotation.eulerAngles;
            if(currentRotation.x>180f){
                currentRotation.x-=360;
            }
            if(currentRotation.y>180f){
                currentRotation.y-=360;
            }

            // Lerp the camera's rotation for a, heavier feel
            Vector3 newRotation=Vector3.SmoothDamp(currentRotation, targetRotation, 
            ref rotationVelocity, rotationSmoothTime);

            // Apply the smoothed rotation to the cameraRoot
            target.localRotation = Quaternion.Euler(newRotation);
        }

        camera.fieldOfView=Mathf.Lerp(camera.fieldOfView,targetFov,boostEffectChangeSpeed*Time.deltaTime);
        //Attenuate changes by this much when going rly fast
        float mod=0;
        //mod=(Mathf.Clamp(swimmer.GetVelocity().magnitude,minVelocityToAttenuateEffects,maxVelocityToAttenuateEffects)-minVelocityToAttenuateEffects)/
        //    (maxVelocityToAttenuateEffects-minVelocityToAttenuateEffects);
        float currentBaseFOV=baseFov+(boostFov-baseFov)*mod;
        if(swimmer.IsCoasting()){
            currentBaseFOV=slowFov;
            //Rumble.AddRumble("Cruising",(targetFov-baseFov)/(currentBaseFOV-baseFov));
        }
        targetFov=Mathf.Lerp(targetFov,currentBaseFOV,boostEffectRestoreSpeed*Time.deltaTime);
        

        if(hasChromaticAberration){
            chromaticAberration.intensity.value=Mathf.Lerp(chromaticAberration.intensity.value,
            chromaticAberrationTargetIntensity,boostEffectChangeSpeed*Time.deltaTime);
            chromaticAberrationTargetIntensity=Mathf.Lerp(chromaticAberrationTargetIntensity,
            chromaticAberrationBaseIntensity,boostEffectRestoreSpeed*Time.deltaTime);
        }

        if(hasPanini){
            paniniProjection.distance.value=Mathf.Lerp(paniniProjection.distance.value,
            paniniTargetIntensity,boostEffectChangeSpeed*Time.deltaTime);
            paniniTargetIntensity=Mathf.Lerp(paniniTargetIntensity,
            paniniBaseIntensity,boostEffectRestoreSpeed*Time.deltaTime);
        }

        // for(int i=0;i<postProcessingValuesToChange.Length;i++){
        //     PostProcessingValueToChange ppv=postProcessingValuesToChange[i];
        //     if(ppv.hasComponent){
        //         float currentVal=(float)ppv.component.GetType().GetProperty(ppv.componentName).GetValue(ppv.component);
        //         float val=Mathf.Lerp(currentVal,ppv.targetValue,boostEffectChangeSpeed*Time.deltaTime);
        //         ppv.component.GetType().GetProperty(ppv.componentName).SetValue(ppv.component,val);
        //         ppv.targetValue=Mathf.Lerp(ppv.targetValue,ppv.baseValue,boostEffectRestoreSpeed*Time.deltaTime);
        //     }
        // }

        if(restoreDampingTrigger){
            cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().Damping=savedDamping;
            restoreDampingTrigger=false;
            Debug.Log("restore damping");
        }

        // if(swimmer.IsCoasting() || swimmer.IsSwimmingBackwards())
        // {
        //     Rumble.AddRumble("Cruising",1f);
        // }

    }

    //Called Boost but really when doing a stride
    public void BoostAnimation(){
        //Attenuate fov effect by this much when going rly fast
        float mod=1;
        mod=1-(Mathf.Clamp(swimmer.GetVelocity().magnitude,minVelocityToAttenuateEffects,maxVelocityToAttenuateEffects)-minVelocityToAttenuateEffects)/
            (maxVelocityToAttenuateEffects-minVelocityToAttenuateEffects);
        targetFov=baseFov+(boostFov-baseFov)*mod;
        
        chromaticAberrationTargetIntensity=chromaticAberrationBaseIntensity+chromaticAberrationIntensityChange;
        paniniTargetIntensity=paniniBaseIntensity+paniniIntensityChange;
        // for(int i=0;i<postProcessingValuesToChange.Length;i++){
        //     PostProcessingValueToChange ppv=postProcessingValuesToChange[i];
        //     if(ppv.hasComponent){
        //         ppv.targetValue=ppv.baseValue+ppv.valueChange;
        //     }
        // }
        //Plan to add panino effect and/or chromatic aberration

        float strength=Mathf.Clamp((swimmer.GetVelocity().magnitude-shockwaveAttenuationMinVelocity)/(shockwaveAttenuationMinVelocity-shockwaveAttenuationMaxVelocity),0f,1f);
        shockwavePulseTrigger.EmitPulse(strength);
    }

    
    public void DashAnimation(){
        //Attenuate fov effect by this much when going rly fast
        float mod=1;
        mod=1-(Mathf.Clamp(swimmer.GetVelocity().magnitude,minVelocityToAttenuateEffects,maxVelocityToAttenuateEffects)-minVelocityToAttenuateEffects)/
            (maxVelocityToAttenuateEffects-minVelocityToAttenuateEffects);
        targetFov=baseFov+(boostFov-baseFov)*mod;
        
        chromaticAberrationTargetIntensity=chromaticAberrationBaseIntensity+chromaticAberrationIntensityChange;
        paniniTargetIntensity=paniniBaseIntensity+paniniIntensityChange;
        // for(int i=0;i<postProcessingValuesToChange.Length;i++){
        //     PostProcessingValueToChange ppv=postProcessingValuesToChange[i];
        //     if(ppv.hasComponent){
        //         ppv.targetValue=ppv.baseValue+ppv.valueChange;
        //     }
        // }
        //Plan to add panino effect and/or chromatic aberration

        float strength=0f;
        shockwavePulseTrigger.EmitPulse(strength);
    }

    void OnApplicationQuit(){
        chromaticAberration.intensity.value=chromaticAberrationBaseIntensity;
        paniniProjection.distance.value=paniniBaseIntensity;
        // for(int i=0;i<postProcessingValuesToChange.Length;i++){
        //     PostProcessingValueToChange ppv=postProcessingValuesToChange[i];
        //     if(ppv.hasComponent){
        //         ppv.component.GetType().GetProperty(ppv.componentName).SetValue(ppv.component,ppv.baseValue);
        //     }
        // }
    }

    public void ResetCamera(){
        cinemachineVirtualCamera=camera.GetComponent<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.ForceCameraPosition(swimmer.transform.position-
            swimmer.transform.forward*cinemachineVirtualCamera.
            GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance, 
            swimmer.transform.rotation);
        savedDamping=cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().Damping;
        cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().Damping=Vector3.zero;
        restoreDampingTrigger=true;
        Debug.Log("reset camera");
    }
}

public enum PostProcessingPropertyNames{
    intensity,
    distance
}