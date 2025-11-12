using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public GameObject deathVFX;

    private int currentHealth;
    private CharacterController controller;
    private PlayerMovement2D movement;

    // 🔔 Event voor UI (HP icons)
    public UnityEvent<int, int> OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<CharacterController>();
        movement = GetComponent<PlayerMovement2D>();

        // Eerste update naar UI
        OnHealthChanged.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
       
        OnHealthChanged.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            InstantRespawn();
    }

    public void InstantRespawn()
    {
     

        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);

        // ✅ Kies checkpoint als die bestaat, anders dichtstbijzijnde respawnpunt
        Transform respawnPoint = Checkpoint.activeCheckpoint ?? FindClosestRespawnPoint();

        if (respawnPoint == null)
        {
           
            respawnPoint = transform;
        }

        // Schakel tijdelijk uit zodat CharacterController niet glitcht
        controller.enabled = false;
        if (movement != null) movement.enabled = false;

        // Teleporteer speler iets boven het punt (veilig boven grond)
        transform.position = respawnPoint.position + Vector3.up * 1.2f;

        // Reset health
        currentHealth = maxHealth;

        // Weer aanzetten
        controller.enabled = true;
        if (movement != null) movement.enabled = true;

        // Update UI
        OnHealthChanged.Invoke(currentHealth, maxHealth);
    }

    private Transform FindClosestRespawnPoint()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("Respawn");
        if (points.Length == 0)
            return null;

        Transform closest = points[0].transform;
        float closestDist = Vector3.Distance(transform.position, closest.position);

        foreach (GameObject p in points)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < closestDist)
            {
                closest = p.transform;
                closestDist = dist;
            }
        }

       
        return closest;
    }

    void OnTriggerEnter(Collider other)
    {
        // Instant death bij spikes
        if (other.CompareTag("Spikes"))
        {
         
            currentHealth = 0;
            InstantRespawn();
        }
    }
}
