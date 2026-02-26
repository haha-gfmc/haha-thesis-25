using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class TutorializationIcon : MonoBehaviour
{
    public Sprite[] activeSprites;
    public Sprite[] idleSprites;
    public Sprite[] joystickSprites;

    private float imageIndex=0f;

    public float imageSpeed=4f;

    private Sprite[] sprites;

    public bool active=false;

    [Tooltip("Equivalent of index of of the input in \"values\" array in PlayerInput")]
    public int[] inputNums;

    public bool isButton=true;
    public bool isValue=false;
    public bool isJoystick=false;

    private Image image;
    private PlayerInput playerInput;

    public RectTransform joystick;
    public RectTransform keyboardKey;
    private Vector2 keyboardKeyOgPosition;

    private Vector2 joystickInitialPosition;
    public float joystickRadius;
    public float keyboardKeyDisplacement=8f;

    public bool ignoreYAxisInverted=false;

    [HideInInspector]
    public float masterOpacity=1f;

    private Tutorial tutorial;

    public bool isHarmonySprite=false;
    [Tooltip("For harmony, only show graphic when singing.")]
    public bool onlyWhenSinging=false;


    void Start()
    {
        image=GetComponent<Image>();
        playerInput=FindObjectOfType<PlayerInput>();
        if(isJoystick) joystickInitialPosition=joystick.anchoredPosition;
        tutorial=FindObjectOfType<Tutorial>();

        if (keyboardKey != null)
        {
            keyboardKeyOgPosition=keyboardKey.anchoredPosition;
        }
    }

    void Update()
    {
        masterOpacity=tutorial.opacity;

        if(isButton || isValue){
            active=false;
            foreach(int inputNum in inputNums){
                if(isButton && (bool)playerInput.values[inputNum]){
                    active=true;
                    break;
                }else if(isValue && (float)playerInput.values[inputNum]!=0f){
                    active=true;
                    break;
                }
            }
        }else if(isJoystick){
            Vector2 v=joystickInitialPosition;
            v.x=v.x+Mathf.Clamp(((float)playerInput.values[inputNums[0]]),-1f,1f)*joystickRadius;
            if(!ignoreYAxisInverted && playerInput.yAxisInverted){
                v.y=v.y-Mathf.Clamp(((float)playerInput.values[inputNums[1]]),-1f,1f)*joystickRadius;
            }else{
                v.y=v.y+Mathf.Clamp(((float)playerInput.values[inputNums[1]]),-1f,1f)*joystickRadius;
            }
            joystick.anchoredPosition=v;
            if(Mathf.Abs((float)playerInput.values[inputNums[0]])>0.2f || Mathf.Abs((float)playerInput.values[inputNums[1]])>0.2f){
                active=true;
            }else{
                active=false;
            }
        }


        if(isHarmonySprite){
            sprites=idleSprites;
            Color c=image.color;
            float opacity=c.a;
            float k=1f;
            if(onlyWhenSinging && !FindObjectOfType<SwimmerSinging>().singing){
                k=0f;
            }
            if(active){
                opacity=Mathf.Lerp(opacity,1f*k,imageSpeed*2f*Time.deltaTime);
            }else{
                opacity=Mathf.Lerp(opacity,0f*k,imageSpeed*2f*Time.deltaTime);
            }
            c.a=Mathf.Clamp(opacity,0f,masterOpacity);
            image.color=c;
        }else{
            if(active){
                tutorial.currentlyUsed=true;
                sprites=activeSprites;
                image.color=new Color(1f,1f,1f,0.5f*masterOpacity);
                if (keyboardKey != null)
                {
                    keyboardKey.anchoredPosition=keyboardKeyOgPosition+new Vector2(0f,-keyboardKeyDisplacement);
                }
            }else{
                sprites=idleSprites;
                image.color=new Color(1f,1f,1f,1f*masterOpacity);
                if (keyboardKey != null)
                {
                    keyboardKey.anchoredPosition=keyboardKeyOgPosition;
                }
            }
        }

        imageIndex+=imageSpeed*Time.deltaTime;
        imageIndex=imageIndex%sprites.Length;

        image.sprite=sprites[Mathf.FloorToInt(imageIndex)];
        if(isJoystick){
            joystick.GetComponent<Image>().sprite=joystickSprites[Mathf.FloorToInt(imageIndex)];
            joystick.GetComponent<Image>().color=new Color(1f,1f,1f,1f*masterOpacity);
        }

    }
}
