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

    [Header("Particles")]
    public GameObject fireTrailPrefab;
    public float particleLifetime = 2f;

    private PlayerMovement2D player;
    private CharacterController controller;
    private bool canDash = true;

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
        else
        {
            Debug.LogWarning("FireDash: fireTrailPrefab is niet toegewezen!");
        }

        // 📸 Camera shake
        CameraFollow2D camFollow = null;
        if (Camera.main != null) camFollow = Camera.main.GetComponent<CameraFollow2D>();
        if (camFollow != null)
        {
            StartCoroutine(camFollow.CameraShake(0.2f, 0.3f));
        }

        float startTime = Time.time;
        float verticalVelocity = 0f;

        // Als je in de lucht dashed → kleine boost omhoog
        if (!controller.isGrounded)
        {
            verticalVelocity = airBoostPower;
        }

        // Beweging tijdens dash
        while (Time.time < startTime + dashDuration)
        {
            Vector3 dashDir = Vector3.right * player.facingDirection;
            controller.Move((dashDir * dashSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);
            yield return null;
        }

        // Cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
