using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StaticData.switchcount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StaticData.switchcount++;
            Debug.Log($"Set switch count to: {StaticData.switchcount}");
            Destroy(gameObject);
        }
    }
}
