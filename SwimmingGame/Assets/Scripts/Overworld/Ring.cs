using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public float targetOpacity=0.2f;
    public float minIntensity=0f;
    public float maxIntensity=1f;

    private float currentIntensity=0;

    public float opacityChangeSpeed=0.5f;

    private Renderer renderer;
    private Color color;

    public float cooldownTimer=0f;
    public float cooldownTime=1f;
    public float boostIntensity=1.5f;
    // Start is called before the first frame update
    void Start()
    {
        renderer=GetComponent<Renderer>();
        color=renderer.material.GetColor("_EmissionColor");
        currentIntensity=minIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        currentIntensity=Mathf.Lerp(currentIntensity,targetOpacity,opacityChangeSpeed*Time.deltaTime);
        renderer.material.SetColor("_EmissionColor",color*currentIntensity);
        cooldownTimer+=Time.deltaTime;
    }

    void OnTriggerEnter(Collider other){
        Debug.Log("trigger enter");
        if(other.gameObject.tag=="Player"){
            Debug.Log("player");
            targetOpacity=maxIntensity;
            if(cooldownTimer>=cooldownTime){
                Vector3 force=transform.up*boostIntensity;
                Swimmer swimmer=other.gameObject.GetComponentInParent<Swimmer>();
                if(Vector3.Angle(swimmer.GetComponent<Rigidbody>().linearVelocity,transform.up)>=90){
                    force=-force;
                }
                swimmer.Boost(force);
            }
            cooldownTimer=0f;
        }
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.tag=="Player"){
            targetOpacity=minIntensity;
        }
    }
    
}
