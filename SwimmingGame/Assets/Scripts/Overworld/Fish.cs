using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Fish : NPCOverworld
{

    [Header("Fish Misc.")]
    private float closeToPlayerTimer=0f;
    private float awayFromPlayerTimer=0f;
    [Tooltip("After this time start following player again")]
    public float followToWanderTime=2f;
    public float wanderToFollowShortTime=0.5f;
    public float wanderToFollowLongTime=5f;
    public float awayFromPlayerTime=5f;
    public float wanderToRunTime=1f;

    private SwimmerSinging swimmerSinging;

    public float maxLeaderDistance=5f;

    [Tooltip("FMOD Path sound for when player boops fish.")]
    public string boopSound="event:/Overworld/Fish/Boop";
    public float pitch=1f;

    private Animator playerAnimator;

    public bool boopable=true;

    void Start(){
        swimmerSinging=FindObjectOfType<SwimmerSinging>();
        playerAnimator=player.GetComponent<Animator>();
    }

    void Update(){

        if(movementBehavior==MovementBehavior.FollowLeader && leader==null){
            int tries=0;
            Fish[] fishArray=FindObjectsOfType<Fish>();
            List<Fish> fishes=new List<Fish>();
            for(int i=0;i<fishArray.Length;i++){
                fishes.Add(fishArray[i]);
            }
            while(tries<1000 && leader==null && fishes.Count>0){
                tries++;
                Fish otherFish=fishes[Random.Range(0,fishes.Count)];
                if(otherFish!=this && Vector3.Distance(transform.position,otherFish.transform.position)<=maxLeaderDistance){
                    Fish lead=otherFish;
                    while(lead.leader!=null && tries<1000){
                        tries+=1;
                        lead=lead.leader.GetComponent<Fish>();
                        if(lead==this){
                            break;
                        }
                    }
                    if(lead!=this){
                        leader=otherFish.transform;
                        break;
                    }
                }
                fishes.Remove(otherFish);  
            }
        }

        float distanceFromPlayer=Vector3.Distance(transform.position,player.transform.position);

        switch(movementBehavior){
            case MovementBehavior.FollowPlayer:
                if(distanceFromPlayer<=maxDistanceFromPlayer){
                    closeToPlayerTimer+=Time.deltaTime;
                    if(closeToPlayerTimer>=followToWanderTime){
                        ChangeMovementBehavior(MovementBehavior.Wander);
                        closeToPlayerTimer=0f;
                        awayFromPlayerTimer=0f;
                        break;
                    }
                }else{
                    closeToPlayerTimer-=Time.deltaTime;
                }
                if(distanceFromPlayer>=minDistanceFromPlayer){
                    awayFromPlayerTimer+=Time.deltaTime;
                    if(awayFromPlayerTimer>=awayFromPlayerTime){
                        ChangeMovementBehavior(MovementBehavior.Wander);
                        pastMovementBehavior=MovementBehavior.Wander;
                        awayFromPlayerTimer=0f;
                        closeToPlayerTimer=0f;
                    }
                }
                AnimatorStateInfo animatorStateInfo=playerAnimator.GetCurrentAnimatorStateInfo(0);
                if((animatorStateInfo.IsName("swimmerWakingUp") || animatorStateInfo.IsName("swimmerWakingUpWithOrgan")) && animatorStateInfo.normalizedTime>0.8f){
                    ChangeMovementBehavior(MovementBehavior.RunFromPlayer);
                }
                break;
            case MovementBehavior.Wander:
                if(distanceFromPlayer<=minDistanceFromPlayer){
                    closeToPlayerTimer+=Time.deltaTime;
                    if(swimmerSinging.singing){
                        closeToPlayerTimer+=Time.deltaTime;
                    }
                    if(pastMovementBehavior==MovementBehavior.FollowPlayer && closeToPlayerTimer>=wanderToFollowShortTime){
                        ChangeMovementBehavior(MovementBehavior.FollowPlayer);
                        closeToPlayerTimer=0f;
                        break;
                    }else if(closeToPlayerTimer>=wanderToFollowLongTime){
                        ChangeMovementBehavior(MovementBehavior.FollowPlayer);
                        closeToPlayerTimer=0f;
                        break;
                    }
                }else{
                    closeToPlayerTimer-=Time.deltaTime;
                }
                if(distanceFromPlayer<=maxDistanceFromPlayer){
                    closeToPlayerTimer+=Time.deltaTime;
                    if(closeToPlayerTimer>wanderToRunTime){
                        ChangeMovementBehavior(MovementBehavior.RunFromPlayer);
                        awayFromPlayerTimer=0f;
                        closeToPlayerTimer=0f;
                    }
                }
                break;
            case MovementBehavior.RunFromPlayer:
                if((pastMovementBehavior==MovementBehavior.FollowLeader || 
                pastMovementBehavior==MovementBehavior.Wander) && distanceFromPlayer<=maxDistanceFromPlayer){
                    awayFromPlayerTime+=Time.deltaTime;
                }
                if(distanceFromPlayer>=minDistanceFromPlayer){
                    awayFromPlayerTimer+=Time.deltaTime;
                    if(swimmerSinging.singing){
                        awayFromPlayerTimer-=Time.deltaTime;
                    }
                }
                if(awayFromPlayerTimer>=awayFromPlayerTime){
                    if(pastMovementBehavior==MovementBehavior.FollowLeader){
                        ChangeMovementBehavior(MovementBehavior.FollowLeader);
                    }else{
                        ChangeMovementBehavior(MovementBehavior.Wander);
                    }
                    awayFromPlayerTimer=0f;
                    closeToPlayerTimer=0f;
                }                
                break;
            case MovementBehavior.None:     //Bandaid fix of sometimes fish lose their movement behavior -- not sure what triggers it
                if(leader!=null){
                    ChangeMovementBehavior(MovementBehavior.FollowLeader);
                }else{
                    ChangeMovementBehavior(MovementBehavior.Wander);
                }
                break;
        }


        closeToPlayerTimer=Mathf.Max(closeToPlayerTimer,0f);
        awayFromPlayerTimer=Mathf.Max(awayFromPlayerTimer,0f);
    }

    void OnCollisionEnter(Collision other){
        if(boopable){
            if(other.gameObject.tag=="Player"){
                Debug.Log("Boop player");
                ChangeMovementBehavior(MovementBehavior.RunFromPlayer);
                awayFromPlayerTimer=0f;
                closeToPlayerTimer=0f;
                Sound.Play3DOneShotVolume(boopSound,1f,transform,"",0,pitch);
            }
        }
    }


}
