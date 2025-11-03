using UnityEngine;

public class AbilityUnlock : MonoBehaviour
{
    public string abilityName;
    public GameObject unlockVFX;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AbilityManager.Instance.Unlock(abilityName);

            if (unlockVFX != null)
                Instantiate(unlockVFX, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
