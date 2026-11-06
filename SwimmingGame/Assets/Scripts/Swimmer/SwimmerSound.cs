using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SwimmerSound : Sound
{
    [Tooltip("Past this speed stop increasing ambient swimming sound volume.")]
    public float maxSwimmingSpeed=3f;

    private EventInstance ambientSwimmingInstance;
    private EventInstance turningInstance;

    //private StudioEventEmitter ambience;


    public float strideVolume=1f;
    public float kickVolume=1f;
    public float dashVolume=1f;
    public float kickbackVolume = 1f;
    public float swimBackwardVolume = 1f;
    public float ambientSwimmingVolume = 1f;
    public float turningVolume = 1f;
    
    public Transform cameraTransform;
    public Transform swimmerTransform;

    public Swimmer swimmer;

    public bool ignoreCameraDistance;

    void Start()
    {
        ambientSwimmingInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Swimming/AmbientSwimming");
        turningInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Swimming/Turning");
        //ambience = GameObject.Find("Underwater Ambiance").GetComponent<StudioEventEmitter>();
    }

    void Update()
    {
        float cameraDistance = 2f;
        if (!ignoreCameraDistance)
        {
            cameraDistance = Vector3.Distance(swimmerTransform.position, cameraTransform.position);
        }
        RuntimeManager.StudioSystem.setParameterByName("distanceToCamera", cameraDistance);

        Turning();

    }

    public void Stride(float speed){
        PlayOneShotVolume("event:/Swimming/Stride",masterVolume*strideVolume,"swimmingSpeed",speed);
    } 

    public void Kick(){
        PlayOneShotVolume("event:/Swimming/Kick",masterVolume*kickVolume);
    }

    public void Dash(){
        PlayOneShotVolume("event:/Swimming/Dash",masterVolume*dashVolume);
    }

    public void KickBack()
    {
        PlayOneShotVolume("event:/Swimming/Kickback", masterVolume * kickbackVolume);
    }
    
    public void SwimBackward()
    {
        PlayOneShotVolume("event:/Swimming/SwimBackward", masterVolume * swimBackwardVolume);
    }


    public void StartSwimming(float speed){
        if(!IsPlaying(ambientSwimmingInstance)){
            ambientSwimmingInstance.start();
        }
        float volume=Mathf.Clamp(speed/maxSwimmingSpeed,0f,1f);
        ambientSwimmingInstance.setParameterByName("swimmingVolume", 1f);
        ambientSwimmingInstance.setVolume(masterVolume*ambientSwimmingVolume);
    }

    public void StopSwimming()
    {
        //ambientSwimmingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        ambientSwimmingInstance.setParameterByName("swimmingVolume", 0f);
    }

    public void Turning()
    {
        if(!IsPlaying(turningInstance)){
            turningInstance.start();
        }
        turningInstance.setParameterByName("turningSpeed", swimmer.GetRotationVelocity().magnitude); ;
        turningInstance.setParameterByName("swimmingSpeed", swimmer.GetVelocity().magnitude);;
        turningInstance.setVolume(masterVolume*turningVolume);
    }
}
