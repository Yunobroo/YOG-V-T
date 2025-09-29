using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpPower = 7f;
    public float gravity = 20f;

    [Header("Fire Rune Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 1.5f;
    public float airBoostPower = 8f;

    [Header("Dash Particles")]
    public ParticleSystem fireTrail; // sleep hier je particle system in (child van player)

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    private float verticalVelocity = 0f;

    private int facingDirection = 1; // 1 = rechts, -1 = links
    private bool canDash = true;
    private bool isDashing = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (fireTrail != null)
        {
            fireTrail.Stop();
            fireTrail.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // blokkeer normale movement tijdens dash
        if (isDashing) return;

        // === BEWEGEN LINKS/RECHTS ===
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;

        if (moveX > 0.1f)
        {
            facingDirection = 1;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveX < -0.1f)
        {
            facingDirection = -1;
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // === SPRINGEN ===
        if (characterController.isGrounded)
        {
            verticalVelocity = -1f;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                verticalVelocity = jumpPower;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        // combineer movement
        moveDirection = new Vector3(moveX, verticalVelocity, 0);
        characterController.Move(moveDirection * Time.deltaTime);

        // === FIRE RUNE DASH ===
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton1)) && canDash)
        {
            StartCoroutine(DoFireRuneDash());
        }
    }

    private IEnumerator DoFireRuneDash()
    {
        canDash = false;
        isDashing = true;

        // start particles
        if (fireTrail != null)
        {
            fireTrail.gameObject.SetActive(true);
            fireTrail.Play();
        }

        float startTime = Time.time;

        // Platforming boost als je in de lucht bent
        if (!characterController.isGrounded)
        {
            verticalVelocity = airBoostPower;
        }

        while (Time.time < startTime + dashDuration)
        {
            Vector3 dashDir = Vector3.right * facingDirection;
            characterController.Move((dashDir * dashSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);
            yield return null;
        }

        // stop particles
        if (fireTrail != null)
        {
            fireTrail.Stop();
            fireTrail.gameObject.SetActive(false);
        }

        isDashing = false;

        // cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
