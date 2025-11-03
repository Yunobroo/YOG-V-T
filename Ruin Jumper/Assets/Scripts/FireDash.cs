using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement2D))]
[RequireComponent(typeof(CharacterController))]
public class FireDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 1.5f;
    public float airBoostPower = 8f;
    public int dashDamage = 1;
    public float dashHitRadius = 1.2f; // bereik om enemies te raken

    [Header("Particles")]
    public GameObject fireTrailPrefab;
    public float particleLifetime = 2f;

    [Header("Feedback")]
    public GameObject hitVFX;
    public float knockbackForce = 10f;

    [Header("Ability Info")]
    public string abilityName = "FireDash"; // gebruikt door AbilityManager

    private PlayerMovement2D player;
    private CharacterController controller;
    private bool canDash = true;
    private bool isDashing = false;

    // Optionele getter voor andere scripts (zoals EnemyCharger)
    public bool IsDashing => isDashing;

    void Start()
    {
        player = GetComponent<PlayerMovement2D>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 🔒 Check of ability unlocked is
        if (AbilityManager.Instance == null || !AbilityManager.Instance.IsUnlocked(abilityName))
            return;

        // Dash input → LeftShift (keyboard) of B / Circle (controller)
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton7)) && canDash)
        {
            StartCoroutine(DoDash());
        }
    }

    private IEnumerator DoDash()
    {
        canDash = false;
        isDashing = true;

        // 🔥 Spawn vuurtrail
        if (fireTrailPrefab != null)
        {
            GameObject trail = Instantiate(fireTrailPrefab, transform.position, Quaternion.identity);

            // Flip particle afhankelijk van facingDirection
            Vector3 trailScale = trail.transform.localScale;
            trailScale.x = Mathf.Abs(trailScale.x) * player.facingDirection;
            trail.transform.localScale = trailScale;

            Destroy(trail, particleLifetime);
        }

        // 📸 Camera shake
        CameraFollow2D camFollow = null;
        if (Camera.main != null) camFollow = Camera.main.GetComponent<CameraFollow2D>();
        if (camFollow != null)
            StartCoroutine(camFollow.CameraShake(0.2f, 0.3f));

        float startTime = Time.time;
        float verticalVelocity = 0f;

        if (!controller.isGrounded)
            verticalVelocity = airBoostPower;

        // Beweging tijdens dash
        while (Time.time < startTime + dashDuration)
        {
            Vector3 dashDir = Vector3.right * player.facingDirection;
            controller.Move((dashDir * dashSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);

            // 💥 Check hits tijdens dash
            CheckDashHits();

            yield return null;
        }

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void CheckDashHits()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, dashHitRadius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            // 🧠 Zoek elk script met TakeDamage(int)
            var target = hit.GetComponentInParent<MonoBehaviour>();
            if (target == null) continue;

            var method = target.GetType().GetMethod("TakeDamage");
            if (method != null)
            {
                Debug.Log($"🔥 Dash hit enemy: {hit.name} ({target.GetType().Name})");
                method.Invoke(target, new object[] { dashDamage });

                // 💨 Knockback
                Vector3 knockDir = (hit.transform.position - transform.position).normalized;
                knockDir.z = 0;
                var enemyCC = hit.GetComponentInParent<CharacterController>();
                if (enemyCC != null)
                    StartCoroutine(EnemyKnockback(enemyCC, knockDir));

                if (hitVFX != null)
                    Instantiate(hitVFX, hit.transform.position, Quaternion.identity);
            }
        }
    }

    // ✅ Veiligere knockback (vangt verwijderde enemies af)
    private IEnumerator EnemyKnockback(CharacterController enemy, Vector3 dir)
    {
        if (enemy == null) yield break;

        float timer = 0f;
        while (timer < 0.1f)
        {
            if (enemy == null || enemy.gameObject == null)
                yield break;

            try
            {
                enemy.Move(dir * knockbackForce * Time.deltaTime);
            }
            catch (MissingReferenceException)
            {
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dashHitRadius);
    }
}
