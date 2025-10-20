using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerMovement2D))]
public class WaterShield : MonoBehaviour
{
    [Header("Shield Settings")]
    public GameObject waterShieldVFX;   // Visuele effect
    public Collider shieldCollider;     // De SphereCollider die reflecteert
    public GameObject splashEffect;
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
            waterShieldVFX.SetActive(false);

        if (shieldCollider != null)
            shieldCollider.enabled = false;
    }

    void Update()
    {
        bool shieldInput = Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.JoystickButton4);

        if (shieldInput && !shieldActive)
        {
            ActivateShield();
        }
        else if (!shieldInput && shieldActive)
        {
            DeactivateShield();
        }

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

        if (shieldCollider != null)
            shieldCollider.enabled = true;

        Debug.Log("🟦 Shield ACTIVATED");
    }

    void DeactivateShield()
    {
        shieldActive = false;

        if (waterShieldVFX != null)
            waterShieldVFX.SetActive(false);

        if (shieldCollider != null)
            shieldCollider.enabled = false;

        Debug.Log("🟥 Shield DEACTIVATED");
    }

    void DoBounce(Vector3 hitPoint)
    {
        playerMovement.SetVerticalVelocity(bounceForce);

        if (splashEffect != null)
            Instantiate(splashEffect, hitPoint, Quaternion.identity);
    }
}
