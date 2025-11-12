using UnityEngine;

public class UIHealthSwitcher : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth player;

    [Tooltip("Health sprites in volgorde: 0 = dood, 1 = 1HP, ..., laatste = volle HP")]
    public GameObject[] healthStates; // je losse HP-images

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerHealth>();

        if (player != null)
        {
            player.OnHealthChanged.AddListener(UpdateHealthUI);
            UpdateHealthUI(player.maxHealth, player.maxHealth); // initialiseren
        }
    }

    private void UpdateHealthUI(int current, int max)
    {
        // alles uitzetten
        foreach (GameObject g in healthStates)
            if (g != null) g.SetActive(false);

        // juiste image aan
        int index = Mathf.Clamp(current, 0, healthStates.Length - 1);
        if (healthStates[index] != null)
            healthStates[index].SetActive(true);
    }
}
