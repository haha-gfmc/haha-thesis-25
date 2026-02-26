using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialControlSchemeSwitcher : MonoBehaviour
{
    public GameObject gamepadVersion;
    public GameObject keyboardAndMouseVersion;

    private PlayerInput playerInput;
    void Start()
    {
        playerInput=FindObjectOfType<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            gamepadVersion.SetActive(false);
            keyboardAndMouseVersion.SetActive(true);
        }
        else
        {
            keyboardAndMouseVersion.SetActive(false);
            gamepadVersion.SetActive(true);
        }
    }
}
