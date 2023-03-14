using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/**
 * A script for TextMeshPro components which causes their text to appear to be typed/written 
 * character by character.
 * 
 * @author Erin Ratelle
 * Reference: https://github.com/rioter00/UnityExamples/blob/master/typewriterUI.cs
 */
public class Typing : MonoBehaviour
{
    TMP_Text tmpText;
    string writer;

    [SerializeField] float initialDelay = 0f;
    [SerializeField] float timeBtwChars = 0.1f;

    /**
     * Called once all scene components are Awake. 
     * Initializes variables and starts the typing/writing actions.
     */
    void Start()
    {
        tmpText = GetComponent<TMP_Text>();

        writer = tmpText.text;
        tmpText.text = "";

        StartCoroutine("TypeWriter");
    }

    /**
     * Imitates writing/typing by adding each character after the specified delays.
     */
    IEnumerator TypeWriter()
    {
        tmpText.text = "";

        yield return new WaitForSeconds(initialDelay);

        foreach (char c in writer)
        {
            tmpText.text += c;
            yield return new WaitForSeconds(timeBtwChars);
        }
    }
}
