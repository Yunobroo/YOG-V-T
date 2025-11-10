using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DashResetOrb : MonoBehaviour
{
    [Header("Orb Settings")]
    public float respawnTime = 5f;           // Hoe lang tot hij weer verschijnt
    public GameObject pickupVFX;             // Effect bij oppakken
    public GameObject respawnVFX;            // Effect bij respawn
    public AudioClip pickupSFX;              // Geluid bij pickup

    private Renderer orbRenderer;
    private Collider orbCollider;
    private AudioSource audioSource;
    private bool available = true;

    void Start()
    {
        orbRenderer = GetComponentInChildren<Renderer>();
        orbCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();

        // 🔧 Zorg dat trigger aanstaat
        if (orbCollider != null && !orbCollider.isTrigger)
        {
            orbCollider.isTrigger = true;
            Debug.LogWarning($"{name}: Collider was geen trigger — automatisch aangepast.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!available) return;

        Debug.Log($"💫 Orb triggered by {other.name} (tag={other.tag})");

        if (!other.CompareTag("Player"))
            return;

        // ✅ Reset dash
        FireDash dash = other.GetComponent<FireDash>();
        if (dash != null)
        {
            dash.ResetDash();
            Debug.Log("🔥 Dash reset by orb!");
        }
        else
        {
            Debug.LogWarning("❌ Player heeft geen FireDash component gevonden!");
        }

        // 💥 Effecten
        if (pickupVFX != null)
            Instantiate(pickupVFX, transform.position, Quaternion.identity);

        if (pickupSFX != null)
        {
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(pickupSFX);
        }

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        available = false;

        if (orbRenderer != null) orbRenderer.enabled = false;
        if (orbCollider != null) orbCollider.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        if (respawnVFX != null)
            Instantiate(respawnVFX, transform.position, Quaternion.identity);

        if (orbRenderer != null) orbRenderer.enabled = true;
        if (orbCollider != null) orbCollider.enabled = true;

        available = true;
    }

    // Optioneel: laat hem ronddraaien
    void Update()
    {
        transform.Rotate(0, 100 * Time.deltaTime, 0);
    }
}
