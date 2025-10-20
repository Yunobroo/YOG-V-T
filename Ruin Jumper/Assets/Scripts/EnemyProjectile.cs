using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 4f;
    public float reflectSpeedMultiplier = 1.2f;

    private Vector3 direction;
    private bool reflected = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Wordt aangeroepen door de EnemyAI
    public void Launch(Vector3 dir, float newSpeed)
    {
        direction = dir.normalized;
        speed = newSpeed;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // --- Raakt WaterShield ---
        if (other.CompareTag("WaterShield") && !reflected)
        {
            reflected = true;

            // Reflecteer richting: van shield vandaan
           // Reflecteer de invalshoek tegen de botsingsnormaal
Vector3 normal = (transform.position - other.transform.position).normalized;
direction = Vector3.Reflect(direction, normal);


            // Eventueel wat sneller bij reflectie
            speed *= reflectSpeedMultiplier;

            // Optioneel kleur veranderen bij reflectie
            var rend = GetComponent<Renderer>();
            if (rend) rend.material.color = Color.cyan;

            Debug.Log("💧 Projectile reflected!");
            return;
        }

        // --- Raakt Player (niet gereflecteerd) ---
        if (other.CompareTag("Player") && !reflected)
        {
            Debug.Log("🔥 Player hit!");
            Destroy(gameObject);
            return;
        }

        // --- Raakt Enemy (na reflectie) ---
        if (other.CompareTag("Enemy") && reflected)
        {
            Debug.Log("💥 Enemy hit by reflected shot!");
            var enemy = other.GetComponent<EnemyAI2D>();
            if (enemy != null)
                enemy.TakeDamage(1);

            Destroy(gameObject);
            return;
        }

        // --- Raakt iets anders (grond/muur/etc) ---
        if (!other.CompareTag("Player") && !other.CompareTag("WaterShield") && !other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
