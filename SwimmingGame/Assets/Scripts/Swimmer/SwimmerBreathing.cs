using Unity.Hierarchy.Editor;
using Unity.VisualScripting;
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

    public Light spotlight;
    [Tooltip("Target intensity for spotlight when singing.")]
    public float spotlightTargetIntensity=0.4f;

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


    [Header("Breathing")]
    public float maxTimeBetweenBreaths=1f;
    private float breathTimer=0f;

    public int maxBreathes=10;
    public float minimumBreathingVolume=.8f;
    private bool justBreathed=false;
    private int breathCounter=0;

    [Header("Misc.")]
    public float waterEmergeSoundDelay=3f;
    private float timer=0f;
    private bool emerged=false;
    private Camera camera;
    public float breathingFov=50f;
    private float targetFov;
    private float baseFov;
    public float fovLerpSpeed=5f;
    public Vector3 cameraTargetPosition;
    private Vector3 cameraOgPosition;


    void Start()
    {
        playerInput=FindAnyObjectByType<PlayerInput>();
        camera=FindAnyObjectByType<Camera>();
        baseFov=camera.fieldOfView;
        targetFov=baseFov;

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

        spotlightTargetIntensity=spotlight.intensity;
        spotlight.intensity=0f;

        cameraOgPosition=camera.transform.position;
    }

    void Update()
    {
        timer+=Time.deltaTime;
        if (!emerged && timer > waterEmergeSoundDelay)
        {
            emerged=true;
            Sound.PlayOneShotVolume("event:/Overworld/Surface/Water Emerge",1f);
        }

        if (canBreathe && emerged)
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

            if(spotlight!=null){
                spotlight.intensity=Mathf.Lerp(spotlight.intensity,spotlightTargetIntensity*targetOpacity*breathingVolume,lightIntensityLerpSpeed*Time.deltaTime);
            }

            breathTimer+=Time.deltaTime;

            if (breathingVolume >= minimumBreathingVolume && breathTimer>=maxTimeBetweenBreaths && !justBreathed)
            {
                Sound.PlayOneShotVolume("event:/Overworld/Surface/Choke",1f);
                breathTimer=0f;
                justBreathed=true;
                breathCounter+=1;
                animator.SetTrigger("breath");
                if(breathCounter==maxBreathes){
                    FindObjectOfType<LevelLoader>().LoadLevel();
                }
                targetFov=breathingFov;
            }else if(justBreathed && breathTimer>maxTimeBetweenBreaths && breathingVolume < minimumBreathingVolume)
            {
                justBreathed=false;
                targetFov=baseFov;
            }

            camera.fieldOfView=Mathf.Lerp(camera.fieldOfView,targetFov,fovLerpSpeed*Time.deltaTime);

            Vector3 currentCameraTargetPosition=Vector3.Lerp(cameraOgPosition,cameraTargetPosition,Mathf.Clamp((float)breathCounter/(float)maxBreathes,0f,1f));

            camera.transform.position=Vector3.Lerp(camera.transform.position,currentCameraTargetPosition,Time.deltaTime*fovLerpSpeed);
        }
    }
}
