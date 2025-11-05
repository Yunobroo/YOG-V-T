using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    private HashSet<string> unlockedAbilities = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // abilities blijven bewaard tussen scenes
    }

    public bool IsUnlocked(string abilityName)
    {
        return unlockedAbilities.Contains(abilityName);
    }

    public void Unlock(string abilityName)
    {
        if (!unlockedAbilities.Contains(abilityName))
        {
            unlockedAbilities.Add(abilityName);
            Debug.Log($"🔓 Ability unlocked: {abilityName}");
        }
    }
}
