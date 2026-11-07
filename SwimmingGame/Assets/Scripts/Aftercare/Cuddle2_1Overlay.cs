using System;
using UnityEngine;
using UnityEngine.UI;

public class Cuddle2_1Overlay : MonoBehaviour
{
    public GameObject targetCamera;
    private ReceivingPoseController receivingPoseController;
    public ReceivingPoseController.PoseDirection targetPose;
    public float timeBeforeAppearing;
    private float timer;
    public float opacityLerpValue;
    public Image[] images;
    private float[] ogOpacities;

    void Start()
    {
        receivingPoseController=FindAnyObjectByType<ReceivingPoseController>();
        ogOpacities=new float[images.Length];
        for(var i=0;i<images.Length;i++)
        {
            Color c=images[i].color;
            ogOpacities[i]=c.a;
            c.a=0f;
            images[i].color=c;
        }
    }

    void Update()
    {
        if (receivingPoseController.desiredPose == targetPose && targetCamera.activeInHierarchy)
        {
            timer+=Time.deltaTime;
        }
        else
        {
            timer-=Time.deltaTime;
        }
        timer = Mathf.Clamp(timer,0f, timeBeforeAppearing);
        
        float targetOpacity=0f;
        if(timer>=timeBeforeAppearing){
            targetOpacity=1f;
        }
        for(var i=0;i<images.Length;i++)
        {
            Color c=images[i].color;
            if(!targetCamera.activeInHierarchy) c.a=0f;
            else c.a=Mathf.Lerp(c.a,targetOpacity*ogOpacities[i],Time.deltaTime*opacityLerpValue);
            images[i].color=c;
        }
    }
}
