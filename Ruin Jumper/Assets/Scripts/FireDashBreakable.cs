using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FireDashBreakable : MonoBehaviour
{
    [Header("Breakable Settings")]
    public bool respawn = false;
    public float respawnTime = 5f;

    [Header("Effects")]
    public GameObject breakVFX;
    public AudioClip breakSFX;
    public GameObject intactVisual;
    public GameObject brokenVisual;

    private Collider coll;
    private AudioSource audioSource;
    private bool isBroken = false;

    void Start()
    {
        coll = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();

        // Zorg dat vent blokkeert
        coll.isTrigger = false;

        if (intactVisual != null) intactVisual.SetActive(true);
        if (brokenVisual != null) brokenVisual.SetActive(false);
    }

    // ✅ Wordt aangeroepen wanneer de CharacterController tegen de vent botst
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isBroken) return;

        // Check of het de speler is
        var player = hit.collider.GetComponent<FireDash>();
        if (player == null) return;

        // Check of speler aan het dashen is
        if (player.IsDashing)
        {
            BreakVent();
        }
    }

    private void BreakVent()
    {
        isBroken = true;
        Debug.Log($"🔥 {name} is gebroken door FireDash!");

        // 💥 VFX + SFX
        if (breakVFX != null)
            Instantiate(breakVFX, transform.position, Quaternion.identity);

        if (breakSFX != null)
        {
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(breakSFX);
        }

        // Wissel visuals
        if (intactVisual != null) intactVisual.SetActive(false);
        if (brokenVisual != null) brokenVisual.SetActive(true);

        // ❌ Collider uit → speler kan erdoor
        coll.enabled = false;

        // Respawn eventueel
        if (respawn)
            StartCoroutine(RespawnRoutine());
        else
            Destroy(gameObject, 2f);
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);

        if (breakVFX != null)
            Instantiate(breakVFX, transform.position, Quaternion.identity);

        if (intactVisual != null) intactVisual.SetActive(true);
        if (brokenVisual != null) brokenVisual.SetActive(false);

        coll.enabled = true;
        isBroken = false;
    }
}
