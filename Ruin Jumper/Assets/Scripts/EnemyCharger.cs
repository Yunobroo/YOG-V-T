using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyCharger : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float gravity = 20f;
    public float patrolRange = 5f;

    [Header("Charge Settings")]
    public float detectionRange = 8f;
    public float chargeSpeed = 10f;
    public float chargeDuration = 1.2f;
    public float chargeCooldown = 2f;
    public int chargeDamage = 1;
    public float knockbackForce = 12f;
    public float knockbackDuration = 0.15f;

    [Header("Health Settings")]
    public int maxHealth = 3;
    public GameObject deathVFX;

    [Header("VFX & Feedback")]
    public GameObject chargeVFX;
    public GameObject hitVFX;
    public GameObject cancelVFX;

    private CharacterController controller;
    private Transform player;
    private Vector3 startPos;
    private Vector3 velocity;
    private int facingDir = 1;
    private bool isCharging = false;
    private bool onCooldown = false;
    private bool chargeCancelled = false;
    private float patrolDir = 1f;
    private int currentHealth;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPos = transform.position;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isCharging)
        {
            ApplyGravity();
            controller.Move(velocity * Time.deltaTime);
            return;
        }

        if (onCooldown)
        {
            ApplyGravity();
            controller.Move(velocity * Time.deltaTime);
            return;
        }

        bool seesPlayer = player != null && Vector3.Distance(Flat(transform.position), Flat(player.position)) <= detectionRange;

        if (seesPlayer)
        {
            StartCoroutine(ChargeAtPlayer());
        }
        else
        {
            Patrol();
        }

        ApplyGravity();
        controller.Move(velocity * Time.deltaTime);
    }

    private void Patrol()
    {
        Vector3 targetPos = startPos + Vector3.right * patrolRange * patrolDir;

        if (Mathf.Abs(transform.position.x - targetPos.x) < 0.2f)
            patrolDir *= -1f;

        velocity.x = moveSpeed * patrolDir;
        FaceByVelocity();
    }

    private IEnumerator ChargeAtPlayer()
    {
        if (onCooldown) yield break;

        isCharging = true;
        chargeCancelled = false;

        if (chargeVFX != null)
            Instantiate(chargeVFX, transform.position, Quaternion.identity);

        Vector3 dir = Flat((player.position - transform.position).normalized);
        facingDir = dir.x > 0 ? 1 : -1;

        float startTime = Time.time;

        while (Time.time < startTime + chargeDuration)
        {
            if (chargeCancelled)
                break;

            velocity.x = dir.x * chargeSpeed;
            FaceByVelocity();
            controller.Move(velocity * Time.deltaTime);

            Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
            foreach (var hit in hits)
            {
                // --- Player geraakt ---
                if (hit.CompareTag("Player"))
                {
                    var playerHealth = hit.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(chargeDamage);
                        if (hitVFX != null)
                            Instantiate(hitVFX, hit.transform.position, Quaternion.identity);
                    }

                    Vector3 knockDir = Flat((hit.transform.position - transform.position).normalized);

                    // 💥 Knockback speler
                    CharacterController playerCC = hit.GetComponent<CharacterController>();
                    if (playerCC != null)
                        StartCoroutine(PlayerKnockback(playerCC, knockDir, knockbackForce, knockbackDuration));

                    // 💥 Knockback enemy zelf
                    StartCoroutine(SelfKnockback(-knockDir, knockbackForce * 0.7f, 0.2f));

                    velocity = Vector3.zero;
                    controller.Move(-knockDir * 0.5f);

                    isCharging = false;
                    yield return StartCoroutine(CooldownAndResume());
                    yield break;
                }

                // --- WaterShield geraakt ---
                if (hit.CompareTag("WaterShield"))
                {
                    if (cancelVFX != null)
                        Instantiate(cancelVFX, transform.position, Quaternion.identity);

                    chargeCancelled = true;

                    Vector3 bounceDir = Flat((transform.position - hit.transform.position).normalized);
                    StartCoroutine(SelfKnockback(bounceDir, knockbackForce * 0.8f, 0.2f));

                    velocity = Vector3.zero;
                    controller.Move(bounceDir * 0.5f);

                    isCharging = false;
                    yield return StartCoroutine(CooldownAndResume());
                    yield break;
                }
            }

            yield return null;
        }

        velocity.x = 0f;
        isCharging = false;
        yield return StartCoroutine(CooldownAndResume());
    }

    private IEnumerator CooldownAndResume()
    {
        onCooldown = true;
        yield return new WaitForSeconds(chargeCooldown);
        onCooldown = false;
        velocity = Vector3.zero;
        patrolDir = facingDir;
    }

    // --- HEALTH LOGIC ---
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage! HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // kleine flinch
            StartCoroutine(SelfKnockback(-transform.right * facingDir, 4f, 0.1f));
        }
    }

    private void Die()
    {
        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // --- Knockback helpers ---
    private IEnumerator PlayerKnockback(CharacterController target, Vector3 dir, float strength, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            target.Move(dir * strength * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SelfKnockback(Vector3 dir, float strength, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            controller.Move(dir * strength * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        velocity = Vector3.zero;
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded)
            velocity.y = -2f;
        else
            velocity.y -= gravity * Time.deltaTime;
    }

    private void FaceByVelocity()
    {
        if (Mathf.Abs(velocity.x) > 0.05f)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (velocity.x > 0 ? 1 : -1);
            transform.localScale = s;
        }
    }

    private Vector3 Flat(Vector3 v)
    {
        v.z = 0;
        return v;
    }

    void LateUpdate()
    {
        // Houd hem geforceerd op 2.5D-as
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
    }

    // ✅ DIT STUK DETECTEERT DASH DAMAGE
    private void OnTriggerEnter(Collider other)
    {
        // check of speler dashing is
        if (other.CompareTag("Player"))
        {
            var fireDash = other.GetComponent<FireDash>();
            if (fireDash != null)
            {
                var dashField = typeof(FireDash).GetField("isDashing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (dashField != null)
                {
                    bool playerIsDashing = (bool)dashField.GetValue(fireDash);
                    if (playerIsDashing)
                    {
                        TakeDamage(1);
                        Debug.Log($"{name} hit by player dash!");
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
