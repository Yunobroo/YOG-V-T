using System.Collections;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // je speler
    public Vector3 offset = new Vector3(0, 2, -10);

    [Header("Follow Settings")]
    public float smoothSpeed = 5f;

    [Header("Camera Bounds (optional)")]
    public bool useBounds = false;
    public Vector2 minBounds; // linksonder hoek van je level
    public Vector2 maxBounds; // rechtsboven hoek van je level

    private void LateUpdate()
    {
        if (target == null) return;

        // gewenste positie
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // grenzen toepassen
        if (useBounds)
        {
            float camHeight = Camera.main.orthographicSize;
            float camWidth = camHeight * Camera.main.aspect;

            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minBounds.x + camWidth, maxBounds.x - camWidth);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minBounds.y + camHeight, maxBounds.y - camHeight);
        }

        transform.position = smoothedPosition;
    }

    // === Camera shake coroutine ===
    public IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
