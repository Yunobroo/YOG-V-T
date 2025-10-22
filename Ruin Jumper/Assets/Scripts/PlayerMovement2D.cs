using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpPower = 7f;
    public float jumpGravity = 10f;
    public float gravity = 10f;

    [Header("Jump Helpers")]
    public float coyoteTime = 0.2f;       // Tijd dat je nog kunt springen nadat je van de grond valt
    public float jumpBufferTime = 0.1f;   // Tijd dat jump input bewaard blijft

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;

    private float lastGroundedTime = -1f;
    private float lastJumpPressTime = -1f;

    [HideInInspector] public int facingDirection = 1; // 1 = rechts, -1 = links

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;

        // Update facing direction
        if (moveX != 0)
            facingDirection = moveX > 0 ? 1 : -1;

        moveDirection.x = moveX;

        // Gravity
        if (!IsGrounded())
        {
            IsJumping();
        }
        else if (moveDirection.y < 0)
        {
            moveDirection.y = -2f; // kleine negatieve waarde om contact te behouden
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // 🔄 Flip character op basis van facingDirection
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facingDirection;
        transform.localScale = scale;
    }

    void HandleJump()
    {
        // Registreren van jump input
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton1))
            lastJumpPressTime = Time.time;

        // Update grounded timer
        if (IsGrounded())
            lastGroundedTime = Time.time;

        // Jump voorwaarden: coyote time + jump buffer
        if (Time.time - lastJumpPressTime <= jumpBufferTime &&
            Time.time - lastGroundedTime <= coyoteTime)
        {
            moveDirection.y += jumpPower;
            lastJumpPressTime = -1f; // reset buffer
        }
    }

    void IsJumping()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton1))
            moveDirection.y -= jumpGravity * Time.deltaTime;
        else
            moveDirection.y -= gravity * Time.deltaTime;

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.JoystickButton1))
            moveDirection.y = 0;
    }

    // Simpele ground check
    bool IsGrounded()
    {
        return characterController.isGrounded;
    }

    // Voor andere scripts zoals pogo/shield
    public void SetVerticalVelocity(float velocity)
    {
        moveDirection.y = velocity;
    }
}
