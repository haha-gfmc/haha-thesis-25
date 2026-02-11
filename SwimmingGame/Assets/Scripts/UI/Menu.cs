
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Text.RegularExpressions;
using VLB;

public class Menu : MonoBehaviour
{
    public bool active=false;
    public static bool GameIsPaused = false;
    [HideInInspector]
    public bool inSettings=false;
    [HideInInspector]
    public bool inChapterSelect=false;
    [HideInInspector]
    public bool inSceneControl=false;
    public GameObject menuUI;
    public GameObject settingMenuUI;
    public GameObject chapterSelectMenuUI;
    public GameObject sceneControlMenuUI;
    public string mainMenuScene;
    public GameObject[] controls;
    [Tooltip("Overworld 0, mainact 1, climax 2, cuddle 3 -- Not currently using")]
    public int controlsIndex;
    private CinemachineVirtualCamera playerCamera;
    private Transform playerCameraRoot; // The target position and rotation for the player
    [HideInInspector]
    public CursorLockMode myLockState;
    [HideInInspector] 
    public PlayerInput playerInput;
    private bool buttonsLocked=false;

    public GameObject[] buttons;
    public GameObject[] settingsButtons;
    public GameObject[] chapterSelectButtons;
    public GameObject[] sceneControlButtons;
    private GameObject[] currentButtons;
    private float[] currentButtonsAnimationTimes;
    private TMP_Text[][] buttonsText;
    private int buttonIndex=0;
    [Tooltip("If true, gray out buttons when inactive")]
    public bool neutralize=false;

    [Header("Animation Values")]

    public float buttonHighlightedScale=1.1f;

    public Color textButtonHighlightedColor;
    public Color textButtonIdleColor;
    public Color textButtonInteractingColor;

    [Tooltip("If -1 don't change")]
    public float textButtonhighlightedFontSize=-1f;
    public float textButtonIdleFontsize=-1f;
    [Tooltip("If -1, instanteneous")]
    public float textButtonLerpSpeed=-1f;
    [Tooltip("Used with animation curve for when unselected")]
    public float textButtonLerpOutSpeedModifier=-1f;

    public AnimationCurve animationCurve;

    public float submenu_buttonHighlightedScale=1.1f;
    public float submenu_textButtonLerpSpeed=1.1f;

    public float fadeOutSpeed=-1f;
    [HideInInspector]
    public bool fadingOutCanvas;

    [Header("Other Data")]

    public UnityEvent[] buttonEvents;
    public UnityEvent[] settingsEvents;
    public UnityEvent[] chapterSelectEvents;
    public UnityEvent[] sceneControlEvents;
    private UnityEvent[] events;

    FMOD.Studio.Bus masterBus;

    FMOD.Studio.Bus singingBus;

    public TMP_Text desireText;


    [Header("Debug")]
    public TMP_Text debugTMP;

    CanvasGroup canvasGroup;
    public GameObject canvasObject;

    public Animator canvasAnimator;


    // Start is called before the first frame update
    public void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        GameIsPaused = false;
        playerCamera = GameObject.Find("PlayerFollowCamera")?.GetComponent<CinemachineVirtualCamera>();
        playerCameraRoot = GameObject.Find("PlayerCameraRoot")?.GetComponent<Transform>();
        //controls[controlsIndex].gameObject.SetActive(true);

        GetButtons();

        //Getting FMOD master bus
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");

        singingBus = FMODUnity.RuntimeManager.GetBus("bus:/Singing");



        if (PlayerPrefs.HasKey("soundLevel"))
        {
            masterBus.setVolume(PlayerPrefs.GetFloat("soundLevel"));
        }

        Initiate();

        if (textButtonhighlightedFontSize != -1f && textButtonIdleFontsize==-1f)
        {
            textButtonIdleFontsize=buttonsText[0][0].fontSize;
        }

    }
    
    public virtual void Initiate(){

    }

    // // Just making every button gray to show it's not active anymore, but without hiding it
    // public void Neutralize()
    // {
    //     for(int i=0;i<currentButtons.Length;i++){            
    //         Color c=textButtonIdleColor;
    //         for(int k=0;k<buttonsText[i].Length;k++){
    //                 buttonsText[i][k].color=c;
    //         }
    //     }
    //     Debug.Log("neutralized!");
    // }

    // Update is called once per frame
    public virtual void Update()
    {
        if (active)
        {
            NavigateButtons();
            CheckButtons();
            ShowButtons();
        }else if (neutralize && canvasObject.activeInHierarchy)
        {
            buttonIndex=-1;
            ShowButtons();
        }

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.S))
        {
            int i = PlayerPrefs.GetInt("showcaseMode");
            if (i == 0) i = 1;
            else i = 0;
            PlayerPrefs.SetInt("showcaseMode", i);
            debugTMP.text = "showcase mode: " + i;
            ResetManager.reset=(PlayerPrefs.GetInt("showcaseMode")==1);
            debugTMP.gameObject.SetActive(true);
            StartCoroutine(HideDebugTMP());
        }

        if (fadingOutCanvas)
        {
            if (canvasGroup == null)
            {
                canvasGroup=GetComponentInChildren<CanvasGroup>();
            }   
            canvasGroup.alpha=Mathf.Lerp(canvasGroup.alpha,0f,fadeOutSpeed*Time.unscaledDeltaTime);
            if (canvasGroup.alpha <= 0.01f)
            {
                canvasGroup.alpha=0f;
                menuUI.SetActive(false);
                settingMenuUI.SetActive(false);
                chapterSelectMenuUI.SetActive(false);
                canvasObject.SetActive(false);
                fadingOutCanvas=false;
            }
        }
    }
    
    IEnumerator HideDebugTMP()
    {
        yield return new WaitForSeconds(2f);
        debugTMP.gameObject.SetActive(false);
    }

    void NavigateButtons(){
        if((playerInput.navigateDown && !playerInput.prevNavigateDown)){
            buttonIndex+=1;
            Sound.PlayOneShotVolume("event:/Non-Diagetic SFX/Select Button",1f);
            Rumble.AddRumbleWithTimeOut("Select Button",1,1f,true);
        }
        if((playerInput.navigateUp && !playerInput.prevNavigateUp)){
            buttonIndex-=1;
            Sound.PlayOneShotVolume("event:/Non-Diagetic SFX/Select Button",1f);
            Rumble.AddRumbleWithTimeOut("Select Button",1,1f,true);
        }
        buttonIndex=(buttonIndex+currentButtons.Length)%currentButtons.Length;
    }

    //What happens if button press
    void CheckButtons(){
        if(!IsSlider(currentButtons[buttonIndex])){
            if(playerInput.prevInteracting && !playerInput.interacting){
                if(buttonIndex>=0 && buttonIndex<events.Length && !buttonsLocked){
                    Sound.PlayInstance("event:/Non-Diagetic SFX/Click Button","Click Button",1f);
                    events[buttonIndex].Invoke();
                    Rumble.AddRumbleWithTimeOut("Choose Button",1,1f,true);
                }else{
                    Debug.LogWarning("Tried to invoke empty event.");
                }
            }
        }else{
            if((playerInput.navigateLeft && !playerInput.prevNavigateLeft)){
                SetValue(buttonIndex,Mathf.Clamp(GetSliderValue(buttonIndex)-1,0,10));
                Rumble.AddRumbleWithTimeOut("Select Button",1,1f,true);
            }
            if((playerInput.navigateRight && !playerInput.prevNavigateRight)){
                SetValue(buttonIndex,Mathf.Clamp(GetSliderValue(buttonIndex)+1,0,10));
                Rumble.AddRumbleWithTimeOut("Select Button",1,1f,true);
            }
        }
    }

    public void Respawn(){
        FindObjectOfType<Swimmer>().Respawn();
        Resume();
    }

    public void SkipScene(){
        Resume();
        FindObjectOfType<LevelLoader>().LoadLevel();
    }

    public void Invert(){
        FindObjectOfType<PlayerInput>().InvertYAxis();
    }

    public void MainMenu(){
        Resume();
        FindObjectOfType<LevelLoader>().LoadLevel(ResetManager.GameStartScene);
    }

    public void LoadLevel(string levelName){
        buttonsLocked = true;
        Resume();
        FindObjectOfType<LevelLoader>().LoadLevel(levelName);
    }

    public void SetSoundLevel(float f){
        masterBus.setVolume(f);
    }

    public void SetPhysicsPerformance(){

    }

    public void SetWaterTemperature(){

    }

    public void Settings(bool b){
        SwitchScreens();
        inSettings=b;
        if(inSettings){
            settingMenuUI.SetActive(true);
            GetButtons(settingsButtons,settingsEvents);
            ResetSliderValues();
        }
        else{
            menuUI.SetActive(true);
            GetButtons();
        }
        buttonIndex=0;
        canvasAnimator.SetBool("inSettings",b);
    }

    public void ChapterSelect(bool b){
        SwitchScreens();
        inChapterSelect=b;       
        if(inChapterSelect){
            chapterSelectMenuUI.SetActive(true);
            GetButtons(chapterSelectButtons,chapterSelectEvents);
        }
        else{
            menuUI.SetActive(true);
            GetButtons();
        }
        buttonIndex=0;
        canvasAnimator.SetBool("inChapterSelect",b);
    }

    public void SceneControl(bool b){
        SwitchScreens();
        inSceneControl=b;
        if(inSceneControl){
            sceneControlMenuUI.SetActive(true);
            GetButtons(sceneControlButtons,sceneControlEvents);
        }
        else{
            menuUI.SetActive(true);
            GetButtons();
        }
        buttonIndex=0;
        canvasAnimator.SetBool("inSceneControl",b);
    }

    public void SwitchScreens()
    {
        menuUI.SetActive(false);
        if(sceneControlMenuUI!=null) sceneControlMenuUI.SetActive(false);
        if(chapterSelectMenuUI!=null)  chapterSelectMenuUI.SetActive(false);
        if(settingMenuUI!=null) settingMenuUI.SetActive(false);
        inSceneControl=false;
        inSettings=false;
        inChapterSelect=false;
        canvasAnimator.SetBool("inSceneControl",false);
        canvasAnimator.SetBool("inChapterSelect",false);
        canvasAnimator.SetBool("inSettings",false);
    }

    public void GetButtons(GameObject[] nextButtons=null, UnityEvent[] nextEvents=null){
        if (nextButtons == null)
        {
            nextButtons=buttons;
            nextEvents=buttonEvents;
        }
        currentButtons=nextButtons;
        events=nextEvents;
        buttonsText=new TMP_Text[currentButtons.Length][];
        currentButtonsAnimationTimes=new float[currentButtons.Length];
        for(int i=0;i<currentButtons.Length;i++){
            buttonsText[i]=currentButtons[i].GetComponentsInChildren<TMP_Text>();
            currentButtonsAnimationTimes[i]=0f;
        }
    }

    bool IsInSubMenu()
    {
        if (currentButtons == buttons)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool IsSlider(GameObject button){
        if(button.name.Contains("Slider")){
            return true;
        }
        else{
            return false;
        }
    }

    void ShowButtons(){
        for(int i=0;i<currentButtons.Length;i++){            
            Color c;
            float targetFontSize;
            float targetScale;
            bool isSlider=IsSlider(currentButtons[i]);
            int value=-1;

            // The following value is used to gauge progress from base to target form (for eg scale).
            // This is used to determine how much sketchiness effect is applied
            float progress=1f;

            float highlightedScale=buttonHighlightedScale;
            float lerpSpeed=textButtonLerpSpeed;
            if(IsInSubMenu()){
                highlightedScale=submenu_buttonHighlightedScale;
                lerpSpeed=submenu_textButtonLerpSpeed;
            }

            if(isSlider) value=GetSliderValue(i);
            if(i==buttonIndex){
                if(!playerInput.interacting || isSlider || buttonsLocked){
                    c=textButtonHighlightedColor;
                }else{
                    c=textButtonInteractingColor;
                }
                targetFontSize=textButtonhighlightedFontSize;
                // Image[] images=buttons[i].GetComponentsInChildren<Image>();
                // for(var k=0;k<images.Length;k++){
                //     Color c=images[k].color;
                //     c.a=1f;
                //     images[k].color=c;
                // }
                // buttons[i].GetComponentInChildren<Animator>().speed=1.25f;
                targetScale=highlightedScale;
                currentButtons[i].transform.SetAsLastSibling();
                currentButtonsAnimationTimes[i]+=Time.unscaledDeltaTime*lerpSpeed;
            }else{
                c=textButtonIdleColor;
                targetFontSize=textButtonIdleFontsize;
                // Image[] images=buttons[i].GetComponentsInChildren<Image>();
                // for(var k=0;k<images.Length;k++){
                //     Color c=images[k].color;
                //     c.a=0.625f;
                //     images[k].color=c;
                // }
                // buttons[i].GetComponentInChildren<Animator>().speed=0.5f;
                targetScale=1f;
                currentButtonsAnimationTimes[i]-=Time.unscaledDeltaTime*lerpSpeed*textButtonLerpOutSpeedModifier;
            }

            currentButtonsAnimationTimes[i]=Mathf.Clamp(currentButtonsAnimationTimes[i],0f,1f);

            RectTransform rectTransform=currentButtons[i].GetComponent<RectTransform>();



            if(textButtonLerpSpeed>-1){
                //rectTransform.localScale=Vector3.Lerp(rectTransform.localScale,Vector3.one*targetScale,textButtonLerpSpeed*Time.unscaledDeltaTime);
                rectTransform.localScale=Vector3.Lerp(Vector3.one,Vector3.one*highlightedScale,animationCurve.Evaluate(currentButtonsAnimationTimes[i]));
            }
            else rectTransform.localScale=Vector3.one*targetScale;

            // if(isSlider){
            //     currentButtons[i].transform.Find("Slider").rectTransform.localScale
            // }


            // if(buttonHighlightedScale!=-1f){
            //     progress=1-Mathf.Abs((rectTransform.localScale.x-targetScale)/(buttonHighlightedScale-1f));
            // }

            for(int k=0;k<buttonsText[i].Length;k++){
                // Change button text color
                // if(textButtonLerpSpeed>-1) buttonsText[i][k].color=Color.Lerp(buttonsText[i][k].color,c,textButtonLerpSpeed*Time.unscaledDeltaTime);
                // else{ 
                    buttonsText[i][k].color=c;
                //}

                if(isSlider){
                    if((buttonsText[i][k].name=="Left" && value==0)||(buttonsText[i][k].name=="Right" && value==10)){
                        buttonsText[i][k].color=textButtonIdleColor;
                    }
                }

                // Change font size
                if (targetFontSize != -1)
                {
                    if(textButtonLerpSpeed>-1){
                        buttonsText[i][k].fontSize=Mathf.Lerp(buttonsText[i][k].fontSize,targetFontSize,textButtonLerpSpeed*Time.unscaledDeltaTime);
                        if (Mathf.Abs(buttonsText[i][k].fontSize - targetFontSize) < 1f)
                        {
                            buttonsText[i][k].fontSize=targetFontSize;
                        }
                    }
                    else buttonsText[i][k].fontSize=targetFontSize;
                    //buttonsText[i][k].fontSize=Mathf.Round(buttonsText[i][k].fontSize);
                }

                //Below: Finding "sketchy" tag and replacing it
                // TO-DO: Make a standardized function for looking up bracketed tags
                for(int n=0; n < buttonsText[i][k].text.Length; n++)
                {
                    int m=n;
                    if (buttonsText[i][k].text[n] == '<')
                    {
                        if(n<buttonsText[i][k].text.Length-1 && buttonsText[i][k].text[n+1]=='/') n++;
                        m=n+1;
                        while (m < buttonsText[i][k].text.Length)
                        {
                            if (buttonsText[i][k].text[m] == '>')
                            {
                                string tagContent=buttonsText[i][k].text.Substring(n+1,m-n-1);
                                if(tagContent=="sketchy" || tagContent=="sketchy2" || tagContent=="sketchy3"){
                                    //Transitional sketchy with more movement
                                    if (progress <= .625)
                                    {
                                        tagContent="sketchy3";
                                    }
                                    // Active sketchy when selected
                                    else if (i == buttonIndex)
                                    {
                                        tagContent="sketchy2";
                                    }
                                    // Inactive sketchy when not selected
                                    else
                                    {
                                        tagContent="sketchy";
                                    }
                                    // There is also sketchy0 which I don't want to ever change
                                }
                                else if(tagContent=="wave1" || tagContent=="wave2"){
                                    if (i == buttonIndex)
                                    {
                                        tagContent="wave2";
                                    }
                                    // Inactive  when not selected
                                    else
                                    {
                                        tagContent="wave1";
                                    }
                                }
                                buttonsText[i][k].text=buttonsText[i][k].text.Substring(0,n+1)+tagContent+buttonsText[i][k].text.Substring(m);
                                break;
                            }
                            m++;
                        }
                        n=m;
                    }
                }

                // if(buttonsText[i][k].text.Contains("<") && buttonsText[i][k].text.Substring(buttonsText[i][k].text.IndexOf("<")).Contains(">")){
                //     int bracketIndex1=buttonsText[i][k].text.IndexOf("<");
                //     int bracketIndex2=buttonsText[i][k].text.IndexOf(">");
                //     string tagContent=buttonsText[i][k].text.Substring(bracketIndex1+1,bracketIndex2-bracketIndex2-1);
                //     if(tagContent=="sketchy" || tagContent=="sketchy2" || tagContent=="sketchy3"){
                //         //Transitional sketchy with more movement
                //         if (progress <= .625)
                //         {
                //             buttonsText[i][k].text="<sketchy3>"+buttonsText[i][k].text.Substring(bracketIndex2+1);
                //         }
                //         // Active sketchy when selected
                //         else if (i == buttonIndex)
                //         {
                //             buttonsText[i][k].text="<sketchy2>"+buttonsText[i][k].text.Substring(bracketIndex2+1);
                //         }
                //         // Inactive sketchy when not selected
                //         else
                //         {
                //             buttonsText[i][k].text="<sketchy>"+buttonsText[i][k].text.Substring(bracketIndex2+1);
                //         }
                //         // There is also sketchy0 which I don't want to ever change
                //     }
                // }
            }
        }
    }

    void ResetSliderValues(){
        for(int i=0;i<currentButtons.Length;i++){ 
            if(IsSlider(currentButtons[i])){
                if(i<events.Length && events[i].GetPersistentEventCount()>0){
                    string eventName=events[i].GetPersistentMethodName(0);
                    int value=10;
                    switch(eventName){
                        case nameof(SetSoundLevel):
                            float v;
                            masterBus.getVolume(out v);
                            value=(int)(v*10);
                            break;
                    }
                    SetSliderValue(i,value);
                }
            }
        }
    }

    int GetSliderValue(int i){
        if(IsSlider(currentButtons[i])){
            for(int k=0;k<buttonsText[i].Length;k++){
                if(buttonsText[i][k].name=="Value"){
                    string txt=buttonsText[i][k].text;
                    if(txt.Contains("<") && txt.Substring(txt.IndexOf("<")).Contains(">"))
                    {
                        txt=txt.Substring(txt.IndexOf(">")+1);
                    }
                    txt=Regex.Replace(txt, "[^0-9]", "");
                    return int.Parse(txt);
                }
            }
        }
        return -1;
    }

    void SetValue(int i,int value){
        if(buttonIndex>=0 && buttonIndex<events.Length && events[buttonIndex].GetPersistentEventCount()>0){
            string eventName=events[buttonIndex].GetPersistentMethodName(0);
            switch(eventName){
                case nameof(SetSoundLevel):
                    SetSoundLevel(((float)value)/10);
                    break;
            }
        }
        SetSliderValue(i,value);
    }

    void SetSliderValue(int i, int value)
    {
        for (int k = 0; k < buttonsText[i].Length; k++)
        {
            if (buttonsText[i][k].name == "Value")
            {
                string txt=buttonsText[i][k].text;
                if(txt.Contains("<") && txt.Substring(txt.IndexOf("<")).Contains(">"))
                {
                    buttonsText[i][k].text=txt.Substring(0,txt.IndexOf(">")+1)+value.ToString();
                }else{
                    buttonsText[i][k].text = value.ToString();
                }
            }
        }
    }
    
    public void QuitGame()
    {
        //Resume();
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void Resume()
    {
        // set timescale back to 1, unlock camera
        if (fadeOutSpeed == -1f)
        {
            menuUI.SetActive(false);
            settingMenuUI.SetActive(false);
            chapterSelectMenuUI.SetActive(false);
            canvasObject.SetActive(false);
        }
        Time.timeScale = 1f;
        GameIsPaused = false;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = myLockState;
        inSettings = false;
        inChapterSelect=false;
        singingBus.setVolume(1f);

        if (fadeOutSpeed != -1f)
        {
            fadingOutCanvas=true;
        }

    }


    public void Activate()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        // set timescale to 0, lock camera
        canvasObject.SetActive(true);
        SwitchScreens();
        //if(inSettings) settingMenuUI.SetActive(true);
        //else 
        menuUI.SetActive(true);
        Time.timeScale = 0f;
        UnityEngine.Cursor.visible = true;
        GetButtons();
        singingBus.setVolume(0f);

        if (fadeOutSpeed != -1f)
        {
            if (canvasGroup == null)
            {
                canvasGroup=GetComponentInChildren<CanvasGroup>();
            }   
            canvasGroup.alpha=1f;
        }

        fadingOutCanvas=false;

        if (buttonIndex == -1)
        {
            buttonIndex=0;
        }

    }

    void OnDestroy(){
        float v;
        masterBus.getVolume(out v);
        PlayerPrefs.SetFloat("soundLevel",v);
        singingBus.setVolume(1f);
    }

}
