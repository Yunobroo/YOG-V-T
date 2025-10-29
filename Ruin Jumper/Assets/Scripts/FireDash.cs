using System.Collections;
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

    [Header("Damage Settings")]
    public int dashDamage = 1;
    public float hitRadius = 1f;
    public LayerMask enemyLayer;

    [Header("Particles")]
    public GameObject fireTrailPrefab;
    public float particleLifetime = 2f;

    private PlayerMovement2D player;
    private CharacterController controller;
    private bool canDash = true;
    private bool isDashing = false;

    void Start()
    {
        player = GetComponent<PlayerMovement2D>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
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
            Vector3 trailScale = trail.transform.localScale;
            trailScale.x = Mathf.Abs(trailScale.x) * player.facingDirection;
            trail.transform.localScale = trailScale;
            Destroy(trail, particleLifetime);
        }

        // 📸 Camera shake
        CameraFollow2D camFollow = null;
        if (Camera.main != null) camFollow = Camera.main.GetComponent<CameraFollow2D>();
        if (camFollow != null) StartCoroutine(camFollow.CameraShake(0.2f, 0.3f));

        float startTime = Time.time;
        float verticalVelocity = controller.isGrounded ? 0f : airBoostPower;

        // 🚀 Dash loop
        while (Time.time < startTime + dashDuration)
        {
            Vector3 dashDir = Vector3.right * player.facingDirection;
            controller.Move((dashDir * dashSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);

            // 💥 Check op enemies
            Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius, enemyLayer);
            foreach (Collider hit in hits)
            {
                EnemyAI2D enemy = hit.GetComponent<EnemyAI2D>();
                if (enemy != null)
                {
                    enemy.TakeDamage(dashDamage);
                    Debug.Log($"🔥 Dash hit {enemy.name}");
                }
            }

            yield return null;
        }

        isDashing = false;

        // ⏳ Cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // (optioneel) visueel laten zien in Scene View wat de dash-hit radius is
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}
