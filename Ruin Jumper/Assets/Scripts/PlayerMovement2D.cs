using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpPower = 7f;
    public float gravity = 20f;

    private CharacterController controller;
    private float verticalVelocity;

    public int facingDirection { get; private set; } = 1;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    { Debug.Log("Raw Horizontal = " + Input.GetAxisRaw("Horizontal"));

        HandleMovement();
    }

    void HandleMovement()
    {
        // --- Input ---
        float inputX = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(inputX) < 0.2f) inputX = 0f; // deadzone

        float moveX = inputX * moveSpeed;

        // --- Facing ---
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

        // --- Jump & Gravity ---
        if (controller.isGrounded)
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

        // --- Move ---
        Vector3 move = new Vector3(moveX, verticalVelocity, 0);
        controller.Move(move * Time.deltaTime);

        // --- Fix: forceer Z = 0 (geen autowalk meer in Z) ---
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
    }
}
