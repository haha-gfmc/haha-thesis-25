using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacherSexAI : NPCSexAI
{
    // This should override the function from NPCSexAI (CURRENTLY UNTESTED)
    public override void AIBehavior(){

        //CURRENTLY JUST COPY AND PASTED FROM NPCSEXAI STANDARD BEHAVIOR

        // TELLS US WHEN TO GO TO OTHER BEHAVIORS/CHANGE INTENSITY

        switch(npcSpring.movementBehavior){
            case MovementBehavior.FollowPath:
                //HERE I WOULD TELL NPC TO CHANGE TO OTHER STATE 
                //ONCE IT'S NEAR OBJECTIVE ENOUGH OR WHATEVER
            case MovementBehavior.FollowTarget:
                if(Vector3.Distance(npcSpring.currentTarget.position,transform.position)<=npcSpring.minDistanceFromPlayer){
                    ChangeState(MovementBehavior.Wander);
                }
                break;
            case MovementBehavior.Wander:
                if(distanceMeter>=50f || timeSinceStateChange>=maxTimeBeforeStateChange){
                    //Start follow player when player gets close
                    ChangeState(MovementBehavior.FollowPlayer);
                }
                break;
            case MovementBehavior.FollowPlayer:
                if(timeSinceStateChange>=minTimeBeforeStateChange){
                    if(entanglementMeter>=100f || (timeSinceStateChange>=maxTimeBeforeStateChange)){
                        //Increase intensity the more entangled
                        ChangeIntensity(1);
                    }else if(speedMeter>=50f && npcSpring.currentIntensity<=4f){
                        //Run away and lower intensity if moving too fast
                        ChangeIntensity(-1);
                        //if(npcSpring.currentIntensity<=0){
                            ChangeState(MovementBehavior.RunFromPlayer);
                        //}
                    }
                }
                break;
            case MovementBehavior.RunFromPlayer:
                if(timeSinceStateChange>=minTimeBeforeStateChange){
                    if(entanglementMeter>=100f || timeSinceStateChange>=maxTimeBeforeStateChange || 
                        npcSpring.currentIntensity>=3f){
                        //Go back to following and increase intensity a lot if entangled
                        if(npcSpring.currentIntensity<=2){
                            ChangeIntensity(2);
                        }
                        ChangeState(MovementBehavior.FollowPlayer);
                    }else if(distanceMeter>=100f){
                        ChangeIntensity(1);
                        // Follow/Run if player is very close
                        if(npcSpring.currentIntensity>=2f){
                            ChangeState(MovementBehavior.FollowThenRun);
                        }
                    }
                }
                break;
            case MovementBehavior.FollowThenRun:
                if(timeSinceStateChange>=minTimeBeforeStateChange && (entanglementMeter>=100f || timeSinceStateChange>=maxTimeBeforeStateChange)){
                    // Increase intensity the more entangled
                    ChangeIntensity(1);
                }
                break;
        }

        Rumble.AddRumble("Sex Intensity",Mathf.Clamp(((float)npcSpring.currentIntensity)/((float)intensityToReach),0f,1f));

        // WHEN TO MOVE ON:
        if(npcSpring.currentIntensity>=intensityToReach || stateCounter>=statesToCycleThrough){
            sexGameManager.MoveOn();
        }

    }
}
