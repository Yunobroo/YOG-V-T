using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WaterShield : MonoBehaviour
{
    public GameObject waterShieldVFX;
    public GameObject splashEffect;
    public float shieldDuration = 2f;
    public float bounceForce = 12f;
    public LayerMask pogoSurfaces;

    private CharacterController controller;
    private PlayerMovement2D playerMovement;
    private RuneManager runeManager;
    private bool shieldActive = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement2D>();
        runeManager = GetComponent<RuneManager>();

        if (waterShieldVFX != null)
            waterShieldVFX.SetActive(false);
    }

    void Update()
    {
        if (runeManager == null || !runeManager.IsRuneActive(RuneType.Water)) return;

        // WaterShield activeren met Square (PS) of X (Xbox) = JoystickButton2
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton2)) && !shieldActive)
        {
            StartCoroutine(ActivateShield());
        }

        if (shieldActive && !controller.isGrounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f, pogoSurfaces))
            {
                DoBounce(hit.point);
            }
        }
    }

    IEnumerator ActivateShield()
    {
        shieldActive = true;

        if (waterShieldVFX != null)
            waterShieldVFX.SetActive(true);

        yield return new WaitForSeconds(shieldDuration);

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

