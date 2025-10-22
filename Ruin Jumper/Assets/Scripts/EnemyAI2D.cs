using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyAI2D : MonoBehaviour
{
    [Header("Grounded & 2.5D")]
    public float gravity = 20f;
    public LayerMask groundMask;         
    public float groundStick = -2f;      

    [Header("Patrol")]
    public Transform patrolLeft;         
    public Transform patrolRight;
    public float moveSpeed = 3f;
    public float arriveThreshold = 0.1f;

    [Header("Player Detection & Attack")]
    public Transform player;             
    public float detectionRange = 10f;
    public float stopToShootDistance = 6f;
    public float fireRate = 1.5f;        
    public Transform firePoint;          
    public GameObject projectilePrefab;
    public float projectileSpeed = 14f;

    [Header("Health Settings")]
    public int maxHealth = 3;
    public GameObject deathVFX;

    // intern
    private CharacterController controller;
    private Vector3 velocity;            
    private int facingDirection = 1;     
    private bool canShoot = true;
    private Transform patrolTarget;      
    private int currentHealth;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentHealth = maxHealth;

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (patrolLeft != null && patrolRight != null)
        {
            float dl = Vector3.Distance(transform.position, patrolLeft.position);
            float dr = Vector3.Distance(transform.position, patrolRight.position);
            patrolTarget = (dl <= dr) ? patrolRight : patrolLeft;
        }
    }

    void Update()
    {
        bool grounded = controller.isGrounded || GroundRaycast();
        if (grounded && velocity.y < 0f)
            velocity.y = groundStick;
        else
            velocity.y -= gravity * Time.deltaTime;

        bool hasPlayer = player != null;
        float distToPlayer = hasPlayer ? Vector3.Distance(Flat(transform.position), Flat(player.position)) : Mathf.Infinity;
        bool seesPlayer = hasPlayer && distToPlayer <= detectionRange;

        if (seesPlayer)
        {
            if (distToPlayer <= stopToShootDistance)
            {
                velocity.x = 0f;
                FaceTowards(player.position);
                TryShoot();
            }
            else
            {
                Vector3 dir = Flat(player.position - transform.position).normalized;
                velocity.x = dir.x * moveSpeed;
                FaceByVelocity();
            }
        }
        else
        {
            Patrol();
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private Vector3 Flat(Vector3 v)
    {
        v.z = 0f;
        return v;
    }

    private bool GroundRaycast()
    {
        Vector3 origin = transform.position + Vector3.up * 0.05f;
        return Physics.Raycast(origin, Vector3.down, 0.2f, groundMask);
    }

    private void Patrol()
    {
        if (patrolLeft == null || patrolRight == null)
        {
            velocity.x = 0f;
            return;
        }

        Vector3 toTarget = Flat(patrolTarget.position - transform.position);
        if (toTarget.magnitude <= arriveThreshold)
        {
            patrolTarget = (patrolTarget == patrolLeft) ? patrolRight : patrolLeft;
            toTarget = Flat(patrolTarget.position - transform.position);
        }

        Vector3 dir = toTarget.normalized;
        velocity.x = dir.x * moveSpeed;
        FaceByVelocity();
    }

    private void FaceByVelocity()
    {
        if (Mathf.Abs(velocity.x) > 0.01f)
        {
            facingDirection = velocity.x > 0f ? 1 : -1;
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * facingDirection;
            transform.localScale = s;
        }
    }

    private void FaceTowards(Vector3 worldPos)
    {
        Vector3 dir = Flat(worldPos - transform.position);
        if (Mathf.Abs(dir.x) > 0.01f)
        {
            facingDirection = dir.x > 0f ? 1 : -1;
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * facingDirection;
            transform.localScale = s;
        }
    }

    private void TryShoot()
    {
        if (!canShoot || projectilePrefab == null || firePoint == null) return;
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        canShoot = false;

        Vector3 dir = Flat((player.position + Vector3.up * 0.5f) - firePoint.position).normalized;

        if (Mathf.Abs(dir.x) > 0.01f)
        {
            facingDirection = dir.x > 0f ? 1 : -1;
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * facingDirection;
            transform.localScale = s;
        }

        GameObject p = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        var proj = p.GetComponent<EnemyProjectile>();
        if (proj != null)
        {
            // ⬅️ FIX: geef ook de snelheid mee
            proj.Launch(dir, projectileSpeed);
        }

        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage (HP: {currentHealth})");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void OnReflectedHit()
    {
        TakeDamage(1);
        Debug.Log($"{name} hit by reflected fireball!");
    }
}
