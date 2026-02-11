using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowcaseModeSetup : MonoBehaviour
{
    public void ShowcaseMode()
    {
        PlayerPrefs.SetInt("showcaseMode", 1);
        ResetManager.reset=(PlayerPrefs.GetInt("showcaseMode")==1);
        FindObjectOfType<LevelLoader>().LoadLevel();
        Menu menu=FindObjectOfType<Menu>();
        menu.active=false;
    }

    public void NormalMode()
    {
        PlayerPrefs.SetInt("showcaseMode", 0);
        ResetManager.reset=(PlayerPrefs.GetInt("showcaseMode")==1);
        FindObjectOfType<LevelLoader>().LoadLevel();
        Menu menu=FindObjectOfType<Menu>();
        menu.active=false;
    }
}
