using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public GameObject deathVFX;

    private int currentHealth;
    private Transform respawnPoint;
    private CharacterController controller;
    private PlayerMovement2D movement;

    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<CharacterController>();
        movement = GetComponent<PlayerMovement2D>();

        // Zoek respawn punt via tag
        GameObject r = GameObject.FindGameObjectWithTag("Respawn");
        if (r != null)
            respawnPoint = r.transform;
        else
        {
            GameObject fallback = new GameObject("RespawnPoint");
            fallback.transform.position = transform.position;
            respawnPoint = fallback.transform;
            Debug.LogWarning("⚠️ Geen Respawn-tag gevonden! Gebruik huidige positie als respawn.");
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player took {amount} damage! HP = {currentHealth}");

        if (currentHealth <= 0)
            InstantRespawn();
    }

    private void InstantRespawn()
    {
        Debug.Log("💀 Player died — instant respawn");

        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);

        // Zet speler direct terug
        controller.enabled = false;
        if (movement != null) movement.enabled = false;

        transform.position = respawnPoint.position + Vector3.up * 1.2f;

        currentHealth = maxHealth;

        controller.enabled = true;
        if (movement != null) movement.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Als we spikes raken → instant dood + respawn
        if (other.CompareTag("Spikes"))
        {
            Debug.Log("☠️ Player touched spikes! Instant respawn!");
            currentHealth = 0;
            InstantRespawn();
        }
    }
}
