using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/**
 * A script to handle the options menu functions.
 * 
 * @author Erin Ratelle
 * References: https://www.youtube.com/watch?v=0E1acACY0Oc, https://www.youtube.com/watch?v=eki-6QBtDAg
 */
public class OptionsMenu : MonoBehaviour
{
    public static OptionsMenu SCENE = null; // Because this scene is loaded additively, we need to track its state to modify behavior.
    private static Dictionary<MoveAction, List<KeyCode>> KEYS = new Dictionary<MoveAction, List<KeyCode>>();

    public TMP_Dropdown leveling;
    public Toggle wasdToggle, numsToggle, bombToggle;

    /**
     * Standard Tetris Moves/Actions
     */
    public enum MoveAction
    {
        Left,
        Right,
        Down,
        HardDrop,
        Clockwise,
        Counterclockwise,
        Hold,
        Pause
    }

    /**
     * Called once all scene components are Awake.
     */
    void Start()
    {
        SCENE = this;

        // learn player settings
        wasdToggle.isOn = PlayerPrefs.HasKey("WASD");
        numsToggle.isOn = PlayerPrefs.HasKey("NumKeys");
        bombToggle.isOn = PlayerPrefs.HasKey("BombBlocks");

        if (!PlayerPrefs.HasKey("Leveling"))
        {
            leveling.value = 0;
        } else
        {
            string levelMode = PlayerPrefs.GetString("Leveling");
            for (int i = 0; i < leveling.options.Count; i++)
            {
                if (leveling.options[i].text == levelMode + " Goal")
                {
                    leveling.value = i;
                    break;
                }
            }
        }
    }
    
    /**
     * We can't be sure that a user will open Options before playing, 
     * so this function initializes all the default settings.
     */
    public static void Initialize()
    {
        EnableKeys();
        if (!PlayerPrefs.HasKey("Leveling")) PlayerPrefs.SetString("Leveling", "Variable");
    }

    /**
     * Because this scene is loaded additively without listeners, input is processed by other scenes. 
     * This method prevents multiple options scenes from being loaded on top of each other.
     */
    public static void OpenOptions()
    {
        if (SCENE != null)
        {
            Resume();
        } else
        {
            SceneManager.LoadScene("_Options", LoadSceneMode.Additive);
        }
    }

    /**
     * Handles scene unload.
     */
    public static void Resume()
    {
        SCENE.ApplySettings();
        SCENE = null;
        SceneManager.UnloadSceneAsync("_Options");
    }

    /**
     * Enables key settings based on known player preferences.
     */
    public static void EnableKeys()
    {
        KEYS.Clear();

        // always add Standard Keys
        AddKeys(MoveAction.Left, new List<KeyCode>() { KeyCode.LeftArrow });
        AddKeys(MoveAction.Right, new List<KeyCode>() { KeyCode.RightArrow });
        AddKeys(MoveAction.Down, new List<KeyCode>() { KeyCode.DownArrow });
        AddKeys(MoveAction.HardDrop, new List<KeyCode>() { KeyCode.Space });
        AddKeys(MoveAction.Clockwise, new List<KeyCode>() { KeyCode.UpArrow, KeyCode.X });
        AddKeys(MoveAction.Counterclockwise, new List<KeyCode>() { KeyCode.LeftControl, KeyCode.RightControl, KeyCode.Z });
        AddKeys(MoveAction.Hold, new List<KeyCode>() { KeyCode.LeftShift, KeyCode.RightShift, KeyCode.C });
        AddKeys(MoveAction.Pause, new List<KeyCode>() { KeyCode.Escape, KeyCode.F1 });

        if (PlayerPrefs.HasKey("WASD"))
        {
            // add WASD Keys
            AddKeys(MoveAction.Left, new List<KeyCode>() { KeyCode.A });
            AddKeys(MoveAction.Right, new List<KeyCode>() { KeyCode.D });
            AddKeys(MoveAction.Down, new List<KeyCode>() { KeyCode.S });
            AddKeys(MoveAction.Clockwise, new List<KeyCode>() { KeyCode.W, KeyCode.E });
            AddKeys(MoveAction.Counterclockwise, new List<KeyCode>() { KeyCode.Q });
        }

        if (PlayerPrefs.HasKey("NumKeys"))
        {
            // add Num Keys
            AddKeys(MoveAction.Left, new List<KeyCode>() { KeyCode.Keypad4 });
            AddKeys(MoveAction.Right, new List<KeyCode>() { KeyCode.Keypad6 });
            AddKeys(MoveAction.Down, new List<KeyCode>() { KeyCode.Keypad2 });
            AddKeys(MoveAction.HardDrop, new List<KeyCode>() { KeyCode.Keypad8 });
            AddKeys(MoveAction.Clockwise, new List<KeyCode>() { KeyCode.Keypad1, KeyCode.Keypad5, KeyCode.Keypad9 });
            AddKeys(MoveAction.Counterclockwise, new List<KeyCode>() { KeyCode.Keypad3, KeyCode.Keypad7 });
            AddKeys(MoveAction.Hold, new List<KeyCode>() { KeyCode.Keypad0 });
        }
    }

    /**
     * Apply changes based on the current state of this Options Menu.
     */
    public void ApplySettings()
    {
        if (wasdToggle.isOn)
        {
            PlayerPrefs.SetString("WASD", "Enabled");
        } else
        {
            PlayerPrefs.DeleteKey("WASD");
        }

        if (numsToggle.isOn)
        {
            PlayerPrefs.SetString("NumKeys", "Enabled");
        }
        else
        {
            PlayerPrefs.DeleteKey("NumKeys");
        }

        if (bombToggle.isOn)
        {
            PlayerPrefs.SetString("BombBlocks", "Enabled");
        }
        else
        {
            PlayerPrefs.DeleteKey("BombBlocks");
        }

        PlayerPrefs.SetString("Leveling", leveling.options[leveling.value].text.Replace(" Goal", ""));

        Initialize();
    }

    /**
     * Adds a particular movement - KeyCode pair to the enabled user controls.
     * 
     * @param move      The move which will be activated by the enabled keys.
     * @param keyCodes  The keys which can be pressed to activated the move.
     */
    private static void AddKeys(MoveAction move, List<KeyCode> keyCodes)
    {
        if (!KEYS.ContainsKey(move)) KEYS.Add(move, new List<KeyCode>());
        foreach (KeyCode keyCode in keyCodes)
        {
            if (!KEYS[move].Contains(keyCode)) KEYS[move].Add(keyCode);
        }
    }

    /**
     * Checks if any of the currently-enabled KeyCodes for the given move have been pressed.
     * 
     * @param move      The move to be tested.
     * @return          True if one of the enabled keys was pressed, False otherwise.
     */
    public static bool CheckMove(MoveAction move)
    {
        if (KEYS.ContainsKey(move))
        {
            foreach (KeyCode keyCode in KEYS[move])
            {
                if (Input.GetKeyDown(keyCode)) return true;
            }
        }
        return false;
    }
}
