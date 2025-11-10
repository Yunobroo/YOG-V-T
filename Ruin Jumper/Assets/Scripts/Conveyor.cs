using UnityEngine;

public class Conveyor : MonoBehaviour
{
 [Header("Conveyor Settings")]
    [SerializeField] private float velocity = 2f;       // Movement speed
    [SerializeField] private bool moveRight = true;     // Direction along X-axis

    public float GetVelocity() => velocity;

    public Vector3 GetDirection()
    {
        // Always move along the global X-axis
        return moveRight ? Vector3.right : Vector3.left;
    }
    public float GetDirSign() => moveRight ? 1f : -1f;
}


