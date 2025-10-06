using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement2D : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;

    [HideInInspector] public int facingDirection = 1; // 1 = rechts, -1 = links

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float movementDirectionY = moveDirection.y;

        // Update facing direction
        if (moveX != 0)
            facingDirection = moveX > 0 ? 1 : -1;

        moveDirection = new Vector3(moveX, 0, 0);

        // Jump input: Space (PC) of A / Cross (controller)
        if (characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                moveDirection.y = jumpPower;
            }
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move character
        characterController.Move(moveDirection * Time.deltaTime);
    }

    // Voor andere scripts om verticale snelheid te zetten (bijv. pogo)
    public void SetVerticalVelocity(float velocity)
    {
        moveDirection.y = velocity;
    }
}
