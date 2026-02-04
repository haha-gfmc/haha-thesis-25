using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rumble : MonoBehaviour
{
    [Range(0f,1f)]
    public float maxRumbleIntensity=1f;

    private static float currentLeftMotorIntensity=0f;
    private static float currentRightMotorIntensity=0f;

    public RumbleSettings[] rumbleSettings;

    static RumbleSettings[] settings;

    private PlayerInput playerInput;

    private static float timeout=0f;   // Time before cancelling rumble
    private static float timer=0f;
    private static float timedLeftMotorIntensity=0f;   //When adding a rumble that fades out, go from this to zero
    private static float timedRightMotorIntensity=0f; //When adding a rumble that fades out, go from this to zero
    private static bool fadingOut=true;
    

    void Start()
    {
        settings=rumbleSettings;
        playerInput=GetComponent<PlayerInput>();
    }

    private void LateUpdate() {
        if(playerInput.currentControlScheme=="Gamepad"){
            if (timer > 0f)
            {
                if (timedLeftMotorIntensity != 0f && timedLeftMotorIntensity != -1f)
                {
                    if(fadingOut) currentLeftMotorIntensity=Mathf.Lerp(0f,timedLeftMotorIntensity,timer/timeout);
                    else currentLeftMotorIntensity=Mathf.Lerp(0f,timedLeftMotorIntensity,timer/timeout);
                }
                else
                {
                    currentLeftMotorIntensity=0f;
                }

                if (timedRightMotorIntensity != 0f && timedRightMotorIntensity != -1f)
                {
                    if(fadingOut) currentRightMotorIntensity=Mathf.Lerp(0f,timedRightMotorIntensity,timer/timeout);
                    else currentRightMotorIntensity=Mathf.Lerp(0f,timedRightMotorIntensity,timer/timeout);
                }
                else
                {
                    currentRightMotorIntensity=0f;
                }

                timer-=Time.unscaledDeltaTime;

                Gamepad.current.SetMotorSpeeds(currentLeftMotorIntensity*maxRumbleIntensity,currentRightMotorIntensity*maxRumbleIntensity);
            }
            else
            {
                Gamepad.current.SetMotorSpeeds(currentLeftMotorIntensity*maxRumbleIntensity,currentRightMotorIntensity*maxRumbleIntensity);

                currentLeftMotorIntensity=0f;
                currentRightMotorIntensity=0f;
            }
        }
    }

    public static void AddRumble(string nameOfAction,float modifier=1f){
        foreach(RumbleSettings setting in settings){
            if(setting.name==nameOfAction){
                AddRumble(setting.leftMotorIntensity,setting.rightMotorIntensity,setting.rumbleType,modifier);
                break;
            }
        }
    }

    public static void AddRumble(float leftMotorIntensity,float rightMotorIntensity,RumbleType rumbleType=RumbleType.Max,float modifier=1f){
        switch(rumbleType){
            case RumbleType.Additive:
                currentLeftMotorIntensity+=leftMotorIntensity*modifier;
                currentRightMotorIntensity+=rightMotorIntensity*modifier;
                break;
            case RumbleType.Override:
                currentLeftMotorIntensity=leftMotorIntensity*modifier;
                currentRightMotorIntensity=rightMotorIntensity*modifier;
                break;
            case RumbleType.Max:
                currentLeftMotorIntensity=Mathf.Max(leftMotorIntensity*modifier,currentLeftMotorIntensity);
                currentRightMotorIntensity=Mathf.Max(rightMotorIntensity*modifier,currentRightMotorIntensity);
                break;
        }
    }

    public static void AddRumbleWithTimeOut(float leftMotorIntensity=-1f,float rightMotorIntensity=-1f,float modifier = 1f, float time=1f,bool isFadingOut=true)
    {
        if(leftMotorIntensity*modifier>timedLeftMotorIntensity || rightMotorIntensity*modifier>timedRightMotorIntensity || time>timer){
            fadingOut=isFadingOut;
            if (leftMotorIntensity != -1f)
            {
                timedLeftMotorIntensity=leftMotorIntensity*modifier;
            }
            if (rightMotorIntensity != -1f)
            {
                timedRightMotorIntensity=rightMotorIntensity*modifier;
            }
            timeout=time;
            timer=timeout;
        }
    }

    public static void AddRumbleWithTimeOut(string nameOfAction,float modifier = 1f, float time=1f,bool isFadingOut=true)
    {
        float leftMotorIntensity=0f;
        float rightMotorIntensity=0f;
        foreach(RumbleSettings setting in settings){
            if (setting.name == nameOfAction)
            {
                leftMotorIntensity=setting.leftMotorIntensity;
                rightMotorIntensity=setting.rightMotorIntensity;
            }
        }
        if(leftMotorIntensity*modifier>timedLeftMotorIntensity || rightMotorIntensity*modifier>timedRightMotorIntensity || time>timer){
            fadingOut=isFadingOut;
            if (leftMotorIntensity != -1f)
            {
                timedLeftMotorIntensity=leftMotorIntensity*modifier;
            }
            if (rightMotorIntensity != -1f)
            {
                timedRightMotorIntensity=rightMotorIntensity*modifier;
            }
            timeout=time;
            timer=timeout;
        }
    }

    public static void ResetRumble(){
        if(Gamepad.current!=null){
            Gamepad.current.SetMotorSpeeds(0f,0f);
        }
    }

    private void OnDestroy() {
        ResetRumble();
    }

}

[System.Serializable]
public struct RumbleSettings{
    public string name;
    [Range(0f,1f)]
    public float leftMotorIntensity;
    [Range(0f,1f)]
    public float rightMotorIntensity;
    public RumbleType rumbleType;
}

public enum RumbleType{
    Additive, //This rumble intensity gets added to the current one
    Override, //This rumble replaces the current one
    Max //If current intensity bigger than new one then we don't add anything.
}