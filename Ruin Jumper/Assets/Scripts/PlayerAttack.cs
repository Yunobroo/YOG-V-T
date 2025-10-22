using System.Collections;   // <-- nodig voor IEnumerator
using UnityEngine;

[RequireComponent(typeof(PlayerMovement2D))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackCooldown = 0.4f;
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayers;
    public GameObject attackVFX;

    private PlayerMovement2D player;
    private bool canAttack = true;

    void Start()
    {
        player = GetComponent<PlayerMovement2D>();
    }

    void Update()
    {
        // attack → JoystickButton0 (B/Circle) of linker muisknop
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton0)) && canAttack)
        {
            StartCoroutine(DoAttack());
        }
    }

    private IEnumerator DoAttack()
    {
        canAttack = false;

        // optioneel visueel effect
        if (attackVFX != null)
        {
            Vector3 vfxPos = transform.position + Vector3.right * player.facingDirection * 0.8f;
            Quaternion vfxRot = Quaternion.Euler(0, player.facingDirection > 0 ? 0 : 180, 0);
            GameObject vfx = Instantiate(attackVFX, vfxPos, vfxRot);
            Destroy(vfx, 1f);
        }

        // eenvoudige hit-check met raycast
        Vector3 origin = transform.position;
        Vector3 dir = Vector3.right * player.facingDirection;
        if (Physics.Raycast(origin, dir, out RaycastHit hit, attackRange, enemyLayers))
        {
            Debug.Log("Hit enemy: " + hit.collider.name);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position,
                            transform.position + Vector3.right * player.facingDirection * attackRange);
        }
    }
}
