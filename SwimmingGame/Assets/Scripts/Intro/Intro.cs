using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    public Image darkScreen;
    private Tutorial tutorial;
    private int prevTutorialIndex;
    public float fadeInSpeed=20f;

    public float darkScreenOpacityTarget=1f;
    public float darkScreenPulseIntensity=0.1f;
    public float darkScreenPulsePeriod=10f;

    private float timer=0f;
    

    public NPCSequencer exSequencer;

    public RectTransform exRect;
    public float exMoveAcceleration=1f;
    private float exMoveSpeed=0f;

    private Dialogue dialogue;

    public bool swimmerCamOn=false;
    public RawImage swimmerCamImage;
    public float swimmerCamLerpSpeed=1f;

    private bool prevSwimmerCamOn;

    public RectTransform singingWheel;

    private bool loadedCutscene=false;

    public StudioEventEmitter ambiance;
     public PlayableDirector cutsceneDirector;

    [Header("Throbbing Effect")]
    public float throbbingPeriod=5f;
    public float throbbingPeriod2=20f;
    [Tooltip("For every intensity increment subtract this from throbbing periods")]
    public float throbbingPeriodChangePerIntensity=.5f;
    private VolumeProfile profile;

    private Vignette vignette;
    public float vignetteTargetIntensity;
    private float vignetteBaseIntensity;
    private LensDistortion lensDistortion;
    public float lensDistortionTargetIntensity;
    private float lensDistortionBaseIntensity;
    private DepthOfField depthOfField;
    public float depthOfFieldTargetFocalLength;
    private float depthOfFieldBaseFocalLength;
    public Image swimmingNoise;
    public float swimmingNoiseTargetOpacity=.1f;

    private Swimmer swimmer;

    [Header("Showcase Only Stuff")]

    private int prevShowcaseProgress=0;    
    public Canvas staticCanvas;
    public GameObject[] showcaseTeleportLocations;
    [Tooltip("To deactivate")]
    public GameObject[] nonShowcaseDialogueTriggers; 
    public float showcaseTransitionTime=2f;
    

    void Start()
    {
        tutorial=FindObjectOfType<Tutorial>();
        dialogue=FindObjectOfType<Dialogue>();
        Color c=darkScreen.color;
        c.a=1f;
        darkScreen.color=c;

        profile=FindObjectOfType<Volume>().profile;
        if(profile.TryGet<Vignette>(out vignette)){
            vignetteBaseIntensity=vignette.intensity.value;
        }
        if(profile.TryGet<LensDistortion>(out lensDistortion)){
            lensDistortionBaseIntensity=lensDistortion.intensity.value;
        }
        if(profile.TryGet<DepthOfField>(out depthOfField)){
            depthOfFieldBaseFocalLength=depthOfField.focalLength.value;
        }

        swimmer=FindObjectOfType<Swimmer>();
    }

    void Update()
    {
        timer+=Time.deltaTime;
        if(tutorial.index>0){
            Color c=darkScreen.color;
            darkScreenOpacityTarget=darkScreenOpacityTarget-Time.deltaTime/fadeInSpeed;
            darkScreenOpacityTarget=Mathf.Clamp(darkScreenOpacityTarget,darkScreenPulseIntensity,1f);
            c.a=darkScreenOpacityTarget+Mathf.Sin(timer*Mathf.PI*2f/darkScreenPulsePeriod)*darkScreenPulseIntensity;
            darkScreen.color=c;
            if(exSequencer.brainIndex==0){
                exSequencer.nextBrainTrigger=true;
            }
        }
        if(exSequencer.brainIndex>=4){
            exMoveSpeed+=exMoveAcceleration*Time.deltaTime;
            Vector2 pos=exRect.anchoredPosition;
            pos+=exMoveSpeed*new Vector2(.5f,1f)*Time.deltaTime;
            exRect.anchoredPosition=pos;
        }

        if(exSequencer.brainIndex>=4 && (bool)dialogue.story.variablesState["swimmerCamOn"]==true){
            swimmerCamOn=true;
        }

        if(dialogue.inDialogue) ambiance.EventInstance.setParameterByName("intensity",(int)dialogue.story.variablesState["intensity"]);

        if(swimmerCamOn){
            
            FindObjectOfType<Swimmer>().canRotate=false;
            Color c=swimmerCamImage.color;
            c.a=Mathf.Lerp(c.a,1f,swimmerCamLerpSpeed*Time.deltaTime);
            swimmerCamImage.color=c;
            swimmerCamImage.GetComponent<RectTransform>().localScale=Vector3.Lerp(swimmerCamImage.GetComponent<RectTransform>().localScale,Vector3.one,swimmerCamLerpSpeed*Time.deltaTime);
        
            if(!prevSwimmerCamOn){
                FindObjectOfType<Swimmer>().canMove=true;
                
                tutorial.GoToTutorialPart(4);
                Vector2 pos=singingWheel.anchoredPosition;
                pos.x=0;
                singingWheel.anchoredPosition=pos;
            }
        }else{
            swimmer.canMove=false;
            swimmer.canRotate=false;
        }

        if(!loadedCutscene && (bool)dialogue.story.variablesState["loadCutscene"]==true){
            loadedCutscene=true;
            Swimmer swimmer=FindObjectOfType<Swimmer>();
            // lock player movement
            if (swimmer != null){
                swimmer.ToggleMovement();
            }
            StartCoroutine(StartCutscene());
        }


        prevTutorialIndex=tutorial.index;
        prevSwimmerCamOn=swimmerCamOn;

        if(swimmerCamOn) Throb();

        if (PlayerPrefs.GetInt("showcaseMode") == 1)
        {
            if ((int)dialogue.story.variablesState["showcaseProgress"] != prevShowcaseProgress)
            {
                prevShowcaseProgress=(int)dialogue.story.variablesState["showcaseProgress"];
                StartCoroutine(ShowcaseTeleport());
            }

            for(var i = 0; i < nonShowcaseDialogueTriggers.Length; i++)
            {
                if(nonShowcaseDialogueTriggers[i]!=null) nonShowcaseDialogueTriggers[i].SetActive(false);
                if(showcaseTeleportLocations[i]!=null) showcaseTeleportLocations[i].SetActive(true);
            }
        }
        else
        {
            for(var i = 0; i < nonShowcaseDialogueTriggers.Length; i++)
            {
                if(nonShowcaseDialogueTriggers[i]!=null) nonShowcaseDialogueTriggers[i].SetActive(true);
                if(showcaseTeleportLocations[i]!=null) showcaseTeleportLocations[i].SetActive(false);
            }
        }
    }

    IEnumerator ShowcaseTeleport()
    {
        staticCanvas.gameObject.SetActive(true);
        Sound.PlayInstance("event:/Ambience/introNoise","Intro Noise",1f);
        yield return new WaitForSeconds(showcaseTransitionTime);
        swimmer.Transport(showcaseTeleportLocations[prevShowcaseProgress].transform.position,0f);  
        staticCanvas.gameObject.SetActive(false);
        Sound.StopInstance("Intro Noise");
    }

    public int GetIntensity()
{
    if (dialogue != null && dialogue.story != null)
    {
        return (int)dialogue.story.variablesState["intensity"];
    }
    Debug.LogWarning("Dialogue or story is null. Returning default intensity value.");
    return 0; 
}

    IEnumerator StartCutscene(){

        yield return new WaitForSeconds(8f);    // U CAN ADJUST THIS 


        //dialogue.EndDialogue();
        cutsceneDirector.Play();         // START CUTSCENE
    }

    private void Throb(){
        float intensity=(int)dialogue.story.variablesState["intensity"];
        float v=(Mathf.Sin(timer*Mathf.PI*2f/(throbbingPeriod-intensity*throbbingPeriodChangePerIntensity))+1)/2f;
        float v2=(Mathf.Sin(timer*Mathf.PI*2f/(throbbingPeriod2-intensity*throbbingPeriodChangePerIntensity))+1)/2f;
        vignette.intensity.value=v*(vignetteTargetIntensity-vignetteBaseIntensity)*v2+vignetteBaseIntensity;
        lensDistortion.intensity.value=v*(lensDistortionTargetIntensity-lensDistortionBaseIntensity)*v2+lensDistortionBaseIntensity;
        depthOfField.focalLength.value=v*(depthOfFieldTargetFocalLength-depthOfFieldBaseFocalLength)*v2+depthOfFieldBaseFocalLength;
        Color c=swimmingNoise.color;
        c.a=v*swimmingNoiseTargetOpacity;
        swimmingNoise.color=c;
    }

    void OnDestroy()
    {
        vignette.intensity.value=vignetteBaseIntensity;
        lensDistortion.intensity.value=lensDistortionBaseIntensity;
        depthOfField.focalLength.value=depthOfFieldBaseFocalLength;
    }

}
