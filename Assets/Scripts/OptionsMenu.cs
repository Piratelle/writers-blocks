using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * A script to handle the options menu functions.
 * 
 * @author Erin Ratelle
 * References: https://www.youtube.com/watch?v=0E1acACY0Oc, https://www.youtube.com/watch?v=eki-6QBtDAg
 */
public class OptionsMenu : MonoBehaviour
{
    public static bool isOpen = false;

    void Start()
    {
        isOpen = true;
    }

    public static void OpenOptions()
    {
        if (isOpen)
        {
            Resume();
        } else
        {
            Time.timeScale = 0;
            SceneManager.LoadScene("_Options", LoadSceneMode.Additive);
        }
    }

    public static void Resume()
    {
        isOpen = false;
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("_Options");
    }
}
