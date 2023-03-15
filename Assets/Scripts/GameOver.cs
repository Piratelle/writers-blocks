using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    void Awake()
    {
        // grab TMPs
        GameObject scoreGO = GameObject.Find("Final Score");
        TMP_Text scoreText = scoreGO.GetComponent<TMP_Text>();

        GameObject scoreMsgGO = GameObject.Find("Beat Final Score");
        TMP_Text scoreMsgText = scoreMsgGO.GetComponent<TMP_Text>();

        GameObject timeGO = GameObject.Find("Final Time");
        TMP_Text timeText = timeGO.GetComponent<TMP_Text>();

        GameObject timeMsgGO = GameObject.Find("Beat Final Time");
        TMP_Text timeMsgText = timeMsgGO.GetComponent<TMP_Text>();

        // learn previous high score
        int highScore = 0;
        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
        }

        // check for high score
        int score = Level.SCORE;
        scoreText.text = score.ToString("#,0");
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            scoreMsgGO.SetActive(true);
        } else
        {
            scoreMsgGO.SetActive(false);
        }

        // learn previous best time
        int highTime = 0;
        if (PlayerPrefs.HasKey("HighTime"))
        {
            highTime = PlayerPrefs.GetInt("HighTime");
        }

        // check for best time
        int d = (int)Level.DURATION;
        int minutes = d / 60;
        int seconds = d % 60;
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (d > highTime)
        {
            PlayerPrefs.SetInt("HighTime", d);
            timeMsgGO.SetActive(true);
        }
        else
        {
            timeMsgGO.SetActive(false);
        }
    }

    /**
     * Starts a new game for the player.
     */
    public void PlayAgain()
    {
        SceneManager.LoadScene("_Level");
    }

    /**
     * Exits back to Main Menu.
     */
    public void Exit()
    {
        SceneManager.LoadScene("_Title");
    }
}
