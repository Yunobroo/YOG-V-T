using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHealthSwitcher : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth player;

    [Tooltip("Health sprites in volgorde: 0 = dood, 1 = 1HP, ..., laatste = volle HP")]
    public GameObject[] healthStates;

    void Awake()
    {
        // ❗ Blijf bestaan tussen scenes
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        // luister naar elke scene load
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        TryFindPlayer();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryFindPlayer();
    }

    private void TryFindPlayer()
    {
        // probeer speler te vinden als er geen referentie is
        if (player == null)
            player = FindObjectOfType<PlayerHealth>();

        if (player != null)
        {
            // eerst oude event afkoppelen om dubbele calls te vermijden
            player.OnHealthChanged.RemoveListener(UpdateHealthUI);
            player.OnHealthChanged.AddListener(UpdateHealthUI);
            UpdateHealthUI(player.maxHealth, player.maxHealth);
            Debug.Log("❤️ Health UI gekoppeld aan player in scene: " + SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.LogWarning("⚠️ Geen PlayerHealth gevonden — wacht op volgende scene load.");
        }
    }

    private void UpdateHealthUI(int current, int max)
    {
        foreach (GameObject g in healthStates)
            if (g != null) g.SetActive(false);

        int index = Mathf.Clamp(current, 0, healthStates.Length - 1);
        if (healthStates[index] != null)
            healthStates[index].SetActive(true);
    }
}
