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
    public float dashHitRadius = 1.2f;

    [Header("Particles")]
    public GameObject fireTrailPrefab;
    public float particleLifetime = 2f;

    [Header("Feedback")]
    public GameObject hitVFX;
    public float knockbackForce = 10f;

    [Header("Ability Info")]
    public string abilityName = "FireDash";

    private PlayerMovement2D player;
    private CharacterController controller;

    private bool canDash = true;
    private bool isDashing = false;

    // 💡 Nieuw: bijhouden of je al een keer gedashed hebt sinds sprong
    private bool hasDashedInAir = false;

    // Public getter voor andere scripts
    public bool IsDashing => isDashing;

    void Start()
    {
        player = GetComponent<PlayerMovement2D>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 🔒 Ability lock
        if (AbilityManager.Instance == null || !AbilityManager.Instance.IsUnlocked(abilityName))
            return;

        // ✅ Reset dash als speler de grond raakt
        if (controller.isGrounded && hasDashedInAir)
        {
            hasDashedInAir = false;
        }

        // Dash input → LeftShift of B/Circle
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton7)) && canDash)
        {
            // 💡 Check of dash mag — alleen 1x per sprong
            if (!controller.isGrounded && hasDashedInAir)
                return;

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
        {
            verticalVelocity = airBoostPower;
            hasDashedInAir = true; // 💡 dash markeren als gebruikt in de lucht
        }

        // Dash beweging
        while (Time.time < startTime + dashDuration)
        {
            Vector3 dashDir = Vector3.right * player.facingDirection;
            controller.Move((dashDir * dashSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);
            CheckDashHits();
            yield return null;
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // 💫 Nieuw: functie om dash extern te resetten (pickup, power-up, event)
    public void ResetDash()
    {
        hasDashedInAir = false;
        canDash = true;
        Debug.Log("💫 Dash reset!");
    }

 private void CheckDashHits()
{
    // 🎯 Verplaats detectie naar VOOR de speler
    Vector3 hitCenter = transform.position + Vector3.right * player.facingDirection * 1.0f;

    // 🔍 Controleer omgeving op enemies én breakables
    Collider[] hits = Physics.OverlapSphere(hitCenter, dashHitRadius);

    foreach (var hit in hits)
    {
        // 🔥 1️⃣ Enemy-hit
        if (hit.CompareTag("Enemy"))
        {
            var target = hit.GetComponentInParent<MonoBehaviour>();
            if (target == null) continue;

            var method = target.GetType().GetMethod("TakeDamage");
            if (method != null)
            {
                Debug.Log($"🔥 Dash hit enemy: {hit.name} ({target.GetType().Name})");
                method.Invoke(target, new object[] { dashDamage });

                // Knockback
                Vector3 knockDir = (hit.transform.position - transform.position).normalized;
                knockDir.z = 0;
                var enemyCC = hit.GetComponentInParent<CharacterController>();
                if (enemyCC != null)
                    StartCoroutine(EnemyKnockback(enemyCC, knockDir));

                if (hitVFX != null)
                    Instantiate(hitVFX, hit.transform.position, Quaternion.identity);
            }
        }

        // 💥 2️⃣ FireDashBreakable (bijv. vent)
        FireDashBreakable breakable = hit.GetComponent<FireDashBreakable>();
        if (breakable != null)
        {
            Debug.Log($"💥 Dash geraakt breakable: {breakable.name}");
            breakable.SendMessage("BreakVent", SendMessageOptions.DontRequireReceiver);
        }
    }
}

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
