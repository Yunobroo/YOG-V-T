using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 4f;
    public float reflectMultiplier = 1.2f;

    private Vector3 direction;
    private bool reflected;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // ✅ Nieuwe versie van Launch — accepteert ook snelheid
    public void Launch(Vector3 dir, float newSpeed)
    {
        direction = dir.normalized;
        speed = newSpeed;
    }

    // (optioneel behoud van de oude, voor backwards compatibility)
    public void Launch(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // zie altijd wat geraakt wordt
        Debug.Log($"Projectile trigger with {other.name}, tag={other.tag}");

        // --- reflectie ---
        if (other.CompareTag("WaterShield") && !reflected)
        {
            reflected = true;

            // reflectie vector — projectiel stuitert weg van shield
            direction = (transform.position - other.transform.position).normalized;
            speed *= reflectMultiplier;

            var r = GetComponent<Renderer>();
            if (r) r.material.color = Color.cyan;

            Debug.Log("💧 Reflected!");
            return;
        }

        // --- speler geraakt ---
        if (other.CompareTag("Player") && !reflected)
        {
            Debug.Log("🔥 Player hit!");
            Destroy(gameObject);
            return;
        }

        // --- enemy geraakt na reflectie ---
        if (other.CompareTag("Enemy") && reflected)
        {
            var e = other.GetComponent<EnemyAI2D>();
            if (e) e.TakeDamage(1);

            Debug.Log("💥 Enemy hit by reflected shot!");
            Destroy(gameObject);
            return;
        }

        // --- iets anders geraakt ---
        if (!other.CompareTag("Enemy") && !other.CompareTag("Player") && !other.CompareTag("WaterShield"))
        {
            Destroy(gameObject);
        }
    }
}
