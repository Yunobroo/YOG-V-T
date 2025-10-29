using UnityEngine;

/// <summary>
/// Houdt een object geforceerd op een vaste Z-positie (voor 2.5D games).
/// </summary>
public class Fix2_5DPosition : MonoBehaviour
{
    [Tooltip("De Z-positie waarop dit object vastgezet wordt.")]
    public float fixedZ = 0f;

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.z = fixedZ;
        transform.position = pos;
    }
}
