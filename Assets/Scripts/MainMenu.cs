using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * A script to handle the main menu functions.
 * 
 * @author Erin Ratelle
 * Reference: https://www.youtube.com/watch?v=76WOa6IU_s8
 */
public class MainMenu : MonoBehaviour
{
    public GameObject[] buttons;
    public float initialDelay = 6f;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var button in buttons)
        {
            button.SetActive(false);
        }
        Debug.Log("Deactivated");
        Invoke("ReactivateButtons", initialDelay);
    }

    private void ReactivateButtons()
    {
        foreach (var button in buttons)
        {
            button.SetActive(true);
        }
        Debug.Log("Reactivated");
    }

    /**
     * Switches to the gameplay scene.
     */
    public void PlayGame()
    {
        SceneManager.LoadScene("_Level");
    }

    public void OpenOptions()
    {

    }

    public void CloseOptions()
    {

    }

    /**
     * Terminates the program.
     */
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting!");
    }
}
