using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerMovement2D))]
public class WaterShield : MonoBehaviour
{
    [Header("Shield Settings")]
    public GameObject waterShieldVFX;   // Het visuele effect
    public GameObject splashEffect;      // Bounce effect
    public float bounceForce = 12f;
    public LayerMask pogoSurfaces;

    private CharacterController controller;
    private PlayerMovement2D playerMovement;
    private bool shieldActive = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement2D>();

        if (waterShieldVFX != null)
            waterShieldVFX.SetActive(false); // start uit
    }

    void Update()
    {
        // Shield activatie: knop ingedrukt
        bool shieldInput = Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.JoystickButton4);

        if (shieldInput && !shieldActive)
        {
            ActivateShield();
        }
        else if (!shieldInput && shieldActive)
        {
            DeactivateShield();
        }

        // Pogo bounce terwijl shield actief is
        if (shieldActive && !controller.isGrounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f, pogoSurfaces))
            {
                DoBounce(hit.point);
            }
        }
    }

    void ActivateShield()
    {
        shieldActive = true;

        if (waterShieldVFX != null)
            waterShieldVFX.SetActive(true);
    }

    void DeactivateShield()
    {
        shieldActive = false;

        if (waterShieldVFX != null)
            waterShieldVFX.SetActive(false);
    }

    void DoBounce(Vector3 hitPoint)
    {
        playerMovement.SetVerticalVelocity(bounceForce);

        if (splashEffect != null)
            Instantiate(splashEffect, hitPoint, Quaternion.identity);
    }
}
