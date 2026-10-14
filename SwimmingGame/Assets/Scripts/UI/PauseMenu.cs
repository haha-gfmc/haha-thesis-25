using UnityEngine;
using Unity.Cinemachine;
using TMPro;

public class PauseMenu : Menu
{

    public override void Initiate()
    {
        base.Initiate();
    }

    // Update is called once per frame
    public override void Update()
    {
        if (playerInput.pausing && !playerInput.prevPausing)
        {
            myLockState = UnityEngine.Cursor.lockState;
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        active = GameIsPaused;

        base.Update();

    }

    void Pause()
    {
        Activate();
        GameIsPaused = true;
    }

    public void ChangeDesire(string s){
        desireText.text=s;
    }
}
