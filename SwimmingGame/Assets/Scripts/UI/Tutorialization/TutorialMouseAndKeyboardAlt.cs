using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialMouseAndKeyboardAlt : MonoBehaviour
{
    PlayerInput playerInput;

    public string altText;
    private string ogText;

    public Sprite[] activeSprites;
    public Sprite[] idleSprites;
    private Sprite[] ogActiveSprites;
    private Sprite[] ogIdleSprites;
    private bool isJoystick;

    TMP_Text text;
    TutorializationIcon tutorializationIcon;

    void Start()
    {
        playerInput=FindObjectOfType<PlayerInput>();

        if(TryGetComponent<TMP_Text>(out text))
        {
            ogText=text.text;
        }
        if(TryGetComponent<TutorializationIcon>(out tutorializationIcon))
        {
            ogActiveSprites=tutorializationIcon.activeSprites;
            ogIdleSprites=tutorializationIcon.idleSprites;
            isJoystick=tutorializationIcon.isJoystick;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            if (text != null)
            {
                text.text=altText;
            }
            if (tutorializationIcon != null)
            {
                tutorializationIcon.activeSprites=activeSprites;
                tutorializationIcon.idleSprites=idleSprites;
                tutorializationIcon.isJoystick=false;
            }
        }
        else
        {
            if (text != null)
            {
                text.text=ogText;
            }
            if (tutorializationIcon != null)
            {
                tutorializationIcon.activeSprites=ogActiveSprites;
                tutorializationIcon.idleSprites=ogIdleSprites;
                tutorializationIcon.isJoystick=isJoystick;
            }
        }
    }
}
