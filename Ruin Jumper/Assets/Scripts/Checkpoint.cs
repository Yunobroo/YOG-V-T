using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // ✨ Deze variabele wordt door PlayerHealth gebruikt:
    public static Transform activeCheckpoint;

    [Header("Feedback (optioneel)")]
    public GameObject activateVFX;
    public AudioClip activateSFX;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return; // al geactiveerd
        if (!other.CompareTag("Player")) return;

        activeCheckpoint = transform;
        activated = true;

        Debug.Log($"⭐ New checkpoint activated: {name}");

        // visuele feedback
        if (activateVFX != null)
            Instantiate(activateVFX, transform.position, Quaternion.identity);

        if (activateSFX != null)
            AudioSource.PlayClipAtPoint(activateSFX, transform.position);
    }
}
