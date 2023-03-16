using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

/**
 * A script to handle the pause menu functions.
 * 
 * @author Erin Ratelle
 */
public class PauseMenu : MonoBehaviour
{
    public static bool OPEN = false; // Because this scene is loaded additively, we need to track its state to modify behavior.

    /**
     * Called once all scene components are Awake.
     */
    void Start()
    {
        OPEN = true;
    }

    /**
     * Because this scene is loaded additively without listeners, input is processed by other scenes. 
     * This method prevents multiple options scenes from being loaded on top of each other.
     */
    public static void OpenPause()
    {
        if (OPEN)
        {
            // check if we also opened Options!
            if (OptionsMenu.SCENE != null)
            {
                OptionsMenu.Resume();
            } else
            {
                Resume();
            }
        }
        else
        {
            Time.timeScale = 0;
            SceneManager.LoadScene("_Pause", LoadSceneMode.Additive);
        }
    }

    /**
     * Handles scene unload.
     */
    public static void Resume()
    {
        OPEN = false;
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("_Pause");
    }

    public void Options()
    {
        OptionsMenu.OpenOptions();
    }

    /**
     * Exits back to Main Menu.
     */
    public static void Exit()
    {
        Resume();
        SceneManager.LoadScene("_Title");
    }
}
