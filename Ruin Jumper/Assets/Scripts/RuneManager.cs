using UnityEngine;

public enum RuneType { Fire, Water }

public class RuneManager : MonoBehaviour
{
    [Header("Current selected rune")]
    public RuneType currentRune = RuneType.Fire; // standaard Fire

    void Update()
    {
        // Switch rune met R1 op controller (JoystickButton5) of R op keyboard
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            ToggleRune();
        }
    }

    void ToggleRune()
    {
        currentRune = (currentRune == RuneType.Fire) ? RuneType.Water : RuneType.Fire;
        Debug.Log("=== Selected Rune: " + currentRune + " ===");
    }

    public bool IsRuneActive(RuneType rune)
    {
        return currentRune == rune;
    }
}
