using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement2D))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackCooldown = 0.4f;
    public float attackRange = 1.5f;
    public float attackHeight = 1.2f;
    public int attackDamage = 10;
    public LayerMask enemyLayers;

    [Header("Debug / Visuals")]
    public GameObject hitboxPrefab; // optioneel zichtbaar blokje
    public float hitboxDuration = 0.15f;

    private PlayerMovement2D player;
    private bool canAttack = true;

    void Start()
    {
        player = GetComponent<PlayerMovement2D>();
    }

    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton0)) && canAttack)
        {
            StartCoroutine(DoAttack());
        }
    }

    private IEnumerator DoAttack()
    {
        canAttack = false;

        // 📦 Hitbox-positie
        Vector3 center = transform.position + Vector3.right * player.facingDirection * (attackRange / 2);
        Vector3 halfExtents = new Vector3(attackRange / 2, attackHeight / 2, 1f);

        // 🎯 Check alle enemies in hitbox
        Collider[] hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity, enemyLayers);
        bool hitSomething = false;

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponentInParent<EnemyCharger>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                hitSomething = true;
                Debug.Log($"🩸 Hit {hit.name} for {attackDamage} damage!");
            }
        }

        // 🟥 Spawn zichtbaar hitbox-blokje (optioneel, debug)
        if (hitboxPrefab != null)
        {
            GameObject hb = Instantiate(hitboxPrefab, center, Quaternion.identity);
            hb.transform.localScale = new Vector3(attackRange, attackHeight, 1f);
            Destroy(hb, hitboxDuration);
        }
        else
        {
            // Als er geen prefab is, teken dan een debug lijn
            Debug.DrawLine(center - Vector3.up * attackHeight / 2,
                           center + Vector3.up * attackHeight / 2,
                           hitSomething ? Color.red : Color.gray, 0.2f);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // 🔲 Teken de hitbox in de Scene view (Editor)
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.red;
        Vector3 center = transform.position + Vector3.right * (player.facingDirection * (attackRange / 2));
        Vector3 size = new Vector3(attackRange, attackHeight, 1f);
        Gizmos.DrawWireCube(center, size);
    }
}
