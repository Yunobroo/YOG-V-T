using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 4f;
    public float reflectMultiplier = 1.2f;
    public int damage = 1;
    public float enemyHitDelay = 0.05f; // kleine vertraging zodat het niet direct despawnt bij reflectie

    private Vector3 direction;
    private bool reflected;
    private bool canHitEnemy = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector3 dir, float newSpeed)
    {
        direction = dir.normalized;
        speed = newSpeed;
    }

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
        Debug.Log($"Projectile trigger with {other.name}, tag={other.tag}");

        // --- reflectie ---
        if (other.CompareTag("WaterShield") && !reflected)
        {
            reflected = true;
            direction = (transform.position - other.transform.position).normalized;
            speed *= reflectMultiplier;

            var r = GetComponent<Renderer>();
            if (r) r.material.color = Color.cyan;

            Debug.Log("💧 Reflected!");
            Invoke(nameof(EnableEnemyHit), enemyHitDelay); // korte vertraging
            return;
        }

        // --- speler geraakt ---
        if (other.CompareTag("Player") && !reflected)
        {
            Debug.Log("🔥 Player hit!");

            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        // --- enemy geraakt na reflectie ---
        if (other.CompareTag("Enemy") && reflected && canHitEnemy)
        {
            var e = other.GetComponent<EnemyAI2D>();
            if (e != null)
            {
                e.TakeDamage(damage);
                Debug.Log("💥 Enemy hit by reflected shot!");
            }

            Destroy(gameObject);
            return;
        }

        // --- iets anders geraakt ---
        if (!other.CompareTag("Enemy") && !other.CompareTag("Player") && !other.CompareTag("WaterShield"))
        {
            Destroy(gameObject);
        }
    }

    void EnableEnemyHit()
    {
        canHitEnemy = true;
    }
}
