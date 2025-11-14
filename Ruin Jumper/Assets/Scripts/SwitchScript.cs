using UnityEngine;
using UnityEngine.SceneManagement;

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
            int count = StaticData.switchcount;
            Debug.Log($"Set switch count to: {count}");
            if(count == 4)
            {
                SceneManager.LoadScene(2);
            }
            else
            {
               Destroy(gameObject);
            }
        }
    }
}
