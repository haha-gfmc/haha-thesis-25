using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingThing : MonoBehaviour
{
    public float floatPeriod=5f;
    public float offset=0f;
    private float floatTimer=0f;
    public Vector3 floatIntensity;
    private Vector3 originalPosition;

    public bool matchMusic=false;

    public bool randomOffset=false;

    void Start(){
        SpriteRenderer spriteRenderer=GetComponentInChildren<SpriteRenderer>();
        originalPosition=transform.localPosition;
        //if(!matchMusic) floatTimer=Random.Range(0f,floatPeriod);
        //  ^ Why did i do this...?
        if(randomOffset) offset=Random.Range(0f,floatPeriod);
        floatTimer+=offset;
    }

    void Update(){
        float period=floatPeriod;
        if(matchMusic){
            period=60f*period/MusicBeat.GetBPM();
        }
        floatTimer+=Time.deltaTime;
        Vector3 currentPos=transform.localPosition;
        if(floatIntensity.x!=0){
            currentPos.x=originalPosition.x+Mathf.Sin(floatTimer*Mathf.PI*2f/period)*floatIntensity.x;
        }
        if(floatIntensity.y!=0){
            currentPos.y=originalPosition.y+Mathf.Sin(floatTimer*Mathf.PI*2f/period)*floatIntensity.y;
        }        
        if(floatIntensity.z!=0){
            currentPos.z=originalPosition.z+Mathf.Sin(floatTimer*Mathf.PI*2f/period)*floatIntensity.z;
        }
        
        transform.localPosition=currentPos;
    }
}
