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
    private RuneManager runeManager;
    private bool canDash = true;

    void Start()
    {
        player = GetComponent<PlayerMovement2D>();
        controller = GetComponent<CharacterController>();
        runeManager = FindObjectOfType<RuneManager>();
    }

    void Update()
    {
        if (runeManager != null && runeManager.IsRuneActive(RuneType.Fire))
        {
            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton0)) && canDash)
            {
                StartCoroutine(DoDash());
            }
        }
    }

    private IEnumerator DoDash()
    {
        canDash = false;

        // Fire trail
        if (fireTrailPrefab != null)
        {
            GameObject trail = Instantiate(fireTrailPrefab, transform.position, Quaternion.identity);
            Destroy(trail, particleLifetime);
        }

        // Camera shake
        CameraFollow2D camFollow = Camera.main?.GetComponent<CameraFollow2D>();
        if (camFollow != null)
            StartCoroutine(camFollow.CameraShake(0.2f, 0.3f));

        float startTime = Time.time;
        float verticalVelocity = (!controller.isGrounded) ? airBoostPower : 0f;

        while (Time.time < startTime + dashDuration)
        {
            Vector3 dashDir = Vector3.right * player.facingDirection;
            controller.Move((dashDir * dashSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
