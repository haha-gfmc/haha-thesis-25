using UnityEngine;
using UnityEngine.UI;

public class SwimmerBreathing : MonoBehaviour
{
    public Animator animator;
    private PlayerInput playerInput;
    private Vector2 breathingNotePosition; // Relative position on the wheel

    public bool canBreathe;
    private Vector2 inputNote;
    private bool breathing;
    public float mouseSensitivity=2f;

    [Header("UI Things")]
    [Tooltip("The dot that moves on the canvas to signify singing")]
    public GameObject[] singingDots;
    private RectTransform[] singingDotRects;
    [Tooltip("Radius of singing circle on canvas.")]
    public float circleRadius=255f;
    private Vector2 rectTargetPosition;
    private Vector2 rectCenter;
    [Tooltip("Images to affect opacity.")]
    public Image[] images;
    private float[] maxOpacities; //max opacity for each image
    public float noteLerpValue=10f;

    private float targetOpacity=0f;
    public float imageOpacityLerpSpeed=1f;
    public float lightIntensityLerpSpeed=1f;

    [Tooltip("Target intensity for spotlight when singing.")]
    public float auraTargetOpacity=.05f;

    void Start()
    {
        playerInput=FindObjectOfType<PlayerInput>();

        singingDotRects=new RectTransform[singingDots.Length];
        for(int i=0;i<singingDots.Length;i++){
            singingDotRects[i]=singingDots[i].GetComponent<RectTransform>();
        }
        rectTargetPosition=singingDotRects[0].anchoredPosition;
        rectCenter=singingDotRects[0].anchoredPosition;

        maxOpacities=new float[images.Length];
        for(int i=0;i<images.Length;i++){
            Image image=images[i];
            Color c=image.color;
            maxOpacities[i]=c.a;
            image.color=new Color(c.r,c.g,c.b,0f);
        }
    }

    void Update()
    {
        if (canBreathe)
        {
            if(playerInput.currentControlScheme=="Gamepad"){
                inputNote=playerInput.singingNote;
                if(inputNote.magnitude>0f){
                    breathing=true;
                }else{
                    breathing=false;
                }
            }else if(playerInput.currentControlScheme=="Keyboard&Mouse" && playerInput.singing){
                inputNote=playerInput.singingNote*Time.deltaTime*mouseSensitivity+inputNote;
                breathing=true;
            }else{
                inputNote=Vector2.zero;
                breathing=false;
            }

            inputNote=Vector2.ClampMagnitude(inputNote,1f);

            breathingNotePosition=Vector2.Lerp(breathingNotePosition,inputNote,noteLerpValue*Time.deltaTime);
            foreach(RectTransform singingDotRect in singingDotRects){
                singingDotRect.anchoredPosition=rectCenter+breathingNotePosition*circleRadius;
            }

            if(breathing){
                targetOpacity=1f;
            }else{
                targetOpacity=0f;
            }

             Color c;

            for(int i=0;i<images.Length;i++){
                Image image=images[i];
                c=image.color;
                float a=Mathf.Lerp(c.a,targetOpacity*maxOpacities[i],imageOpacityLerpSpeed*Time.deltaTime);
                image.color=new Color(c.r,c.g,c.b,a);
            }

            float breathingVolume=breathingNotePosition.magnitude;

        }
    }
}
