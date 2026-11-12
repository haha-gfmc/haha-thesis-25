using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceBreathing : MonoBehaviour
{
    PlayerInput playerInput;
    public Animator swimmerAnimator;

    public float maxTimeBetweenBreaths=3f;
    private float breathTimer=0f;

    public int maxBreathes=10;

    public SpriteAnimator seaSurfaceAnimator, seaSurfaceAnimator2;
    public float fastAnimationSpeed=4f;
    public float slowAnimationSpeed=1f;

    public float fastAnimationSpeed2=4f;
    public float slowAnimationSpeed2=1f;

    private int breathCounter=0;

    void Start()
    {
        playerInput=FindObjectOfType<PlayerInput>();
    }

    void Update()
    {
        breathTimer+=Time.deltaTime;
        if(playerInput.movingForward && breathTimer>=maxTimeBetweenBreaths){
            swimmerAnimator.SetTrigger("breathe");
            Sound.PlayOneShotVolume("event:/Overworld/Surface/Choke",1f);
            breathTimer=0f;
            seaSurfaceAnimator.imageSpeed=fastAnimationSpeed;
            seaSurfaceAnimator2.imageSpeed=fastAnimationSpeed2;
            playerInput.movedForwardTrigger=false;
            breathCounter+=1;
            if(breathCounter==maxBreathes){
                FindObjectOfType<LevelLoader>().LoadLevel();
            }

        }else if(breathTimer>maxTimeBetweenBreaths*.5f){
            seaSurfaceAnimator.imageSpeed=slowAnimationSpeed;
            seaSurfaceAnimator2.imageSpeed=slowAnimationSpeed2;
        }
    }
}
