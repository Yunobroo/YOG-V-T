using System.Collections;
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
        // Attack input → Linkermuisknop of B/Circle op controller
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton0)) && canAttack)
        {
            StartCoroutine(DoAttack());
        }
    }

    private IEnumerator DoAttack()
    {
        canAttack = false;

        // 🔥 Spawn VFX
        if (attackVFX != null)
        {
            Vector3 vfxPos = transform.position + Vector3.right * player.facingDirection * 0.8f;
            Quaternion vfxRot = Quaternion.Euler(0, player.facingDirection > 0 ? 0 : 180, 0);
            GameObject vfx = Instantiate(attackVFX, vfxPos, vfxRot);
            Destroy(vfx, 1f);
        }

        // 🔍 Raycast hit-check
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = Vector3.right * player.facingDirection;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, attackRange, enemyLayers))
        {
            Debug.Log($"Hit enemy: {hit.collider.name}");

            // 🩸 Check of enemy een health-script heeft
            var enemy = hit.collider.GetComponent<EnemyCharger>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                Debug.Log($"Did {attackDamage} damage to {hit.collider.name}");
            }
            else
            {
                // fallback → elk script met TakeDamage(int)
                var any = hit.collider.GetComponent<MonoBehaviour>();
                if (any != null)
                {
                    var method = any.GetType().GetMethod("TakeDamage");
                    if (method != null)
                        method.Invoke(any, new object[] { attackDamage });
                }
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Gizmos.DrawLine(origin, origin + Vector3.right * player.facingDirection * attackRange);
        }
    }
}
