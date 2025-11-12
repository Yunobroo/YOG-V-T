using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleMenu : MonoBehaviour
{
    [Header("UI References")]
    public Button playButton;
    public TextMeshProUGUI titleText;

    [Header("Settings")]
    public string gameSceneName = "MainScene"; // <-- vervang door jouw echte scene
    public AudioClip startSound;

    private AudioSource audioSource;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        audioSource = GetComponent<AudioSource>();

        if (playButton != null)
            playButton.onClick.AddListener(OnPlayPressed);

        if (titleText != null)
            StartCoroutine(BlinkTitle());
    }

    void OnPlayPressed()
    {
        Debug.Log("> PLAY SELECTED");

        if (startSound != null && audioSource != null)
            audioSource.PlayOneShot(startSound);

        StartCoroutine(LoadGame());
    }

    private System.Collections.IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(gameSceneName);
    }

    private System.Collections.IEnumerator BlinkTitle()
    {
        while (true)
        {
            titleText.alpha = 0.6f;
            yield return new WaitForSeconds(0.3f);
            titleText.alpha = 1f;
            yield return new WaitForSeconds(0.3f);
        }
    }
}
