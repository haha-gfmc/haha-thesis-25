using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CuddleGameManager : MonoBehaviour
{
    public LevelLoader levelLoader;
    public TMP_Text dialogueText; 
    public string dialogueResponse1; 
    public string dialogueResponse2;
    public string dialogueResponse3; 

    public float controllerDeadZone=0.1f;

    private float timer;

    private void Update()
    {
        timer+=Time.unscaledDeltaTime;
        Rumble.AddRumble("Start Menu",(Mathf.Sin(timer*Mathf.PI/2f)+1)*.3f/2f+.3f);
    }

    // for proof of concept, can change later
    public void UpdateDialogueText(string detectedOption)
    {
        switch (detectedOption)
        {
            case "Thigh":
                dialogueText.text = dialogueResponse1;
                break;
            case "Waist":
                dialogueText.text = dialogueResponse2;
                break;
            case "Hand":
                dialogueText.text = dialogueResponse3;
                break;
            default:
                dialogueText.text = "When we all get up there?";
                Debug.LogWarning("Invalid dialogue option!");
                break;
        }
    }
}
