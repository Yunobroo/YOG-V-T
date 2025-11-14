using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHealthSwitcher : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth player;

    [Tooltip("Health sprites in volgorde: 0 = dood, 1 = 1HP, ..., laatste = volle HP")]
    public GameObject[] healthStates;

    private Canvas canvas;

    void Awake()
    {
        // Blijf bestaan tussen scenes
        DontDestroyOnLoad(gameObject);

        // Canvas cache
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogWarning("⚠️ UIHealthSwitcher: geen Canvas gevonden!");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        TryFindPlayer();
        AdjustForResolution();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryFindPlayer();
        AdjustForResolution();
    }

    private void TryFindPlayer()
    {
        if (player == null)
            player = FindObjectOfType<PlayerHealth>();

        if (player != null)
        {
            player.OnHealthChanged.RemoveListener(UpdateHealthUI);
            player.OnHealthChanged.AddListener(UpdateHealthUI);
            UpdateHealthUI(player.maxHealth, player.maxHealth);
        }
        else
        {
            Debug.LogWarning("⚠️ UIHealthSwitcher: PlayerHealth niet gevonden in scene!");
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

    private void AdjustForResolution()
    {
        if (canvas == null) return;

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null) return;

        // Zorg dat hij altijd goed schaalt met scherm
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
    }
}
