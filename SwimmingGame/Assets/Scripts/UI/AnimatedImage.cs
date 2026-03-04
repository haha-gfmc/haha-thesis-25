using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedImage : MonoBehaviour
{
    [HideInInspector]
    public float imageIndex=0f;
    public float imageSpeed=4f;
    public bool synchronizeImageWithParent=false;
    public AnimatedImage parentImage;
    public Sprite[] sprites;
    private Image image;
    private RectTransform rect;

    public float pulsePeriod=0f;
    private float pulseTimer=0f;
    public float pulseRotationIntensity=0f;
    public float pulseOpacityIntensity=0f;
    public float pulseScaleIntensity;
    private Vector3 initialRotation;
    private float initialOpacity;
    private Vector3 initialScale;
    
    public bool ignoreTimeDilation;

    void Start()
    {
        image=GetComponent<Image>();
        rect=GetComponent<RectTransform>();
        if(pulseRotationIntensity!=0f){
            initialRotation=rect.localRotation.eulerAngles;
        }
        initialOpacity=image.color.a;
        pulseTimer=Random.Range(0f,pulsePeriod);
        initialScale=rect.localScale;
    }

    void Update()
    {
        if(!synchronizeImageWithParent){
            imageIndex+=imageSpeed*Time.unscaledDeltaTime;
            imageIndex=imageIndex%sprites.Length;
        }else{
            imageIndex=parentImage.imageIndex;
        }

        if(sprites.Length>1) image.sprite=sprites[Mathf.FloorToInt(imageIndex)];

        if(ignoreTimeDilation) pulseTimer+=Time.unscaledDeltaTime;
        else pulseTimer+=Time.deltaTime;

        if(pulseRotationIntensity!=0f){
            Vector3 rot=initialRotation;
            rot.z=rot.z+Mathf.Sin(pulseTimer*Mathf.PI*2f/pulsePeriod)*pulseRotationIntensity;
            rect.localRotation=Quaternion.Euler(rot);
        }
        if(pulseOpacityIntensity!=0f){
            Color c=image.color;
            c.a=initialOpacity-Mathf.Sin(pulseTimer*Mathf.PI*2f/pulsePeriod)*pulseOpacityIntensity;
            image.color=c;
        }
        if(pulseScaleIntensity!=0f){
            Vector3 s=initialScale+initialScale*Mathf.Sin(pulseTimer*Mathf.PI*2f/pulsePeriod)*pulseScaleIntensity;
            rect.localScale=s;
        }
    }
}
