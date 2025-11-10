using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    private Transform ColliderTransform;
    public int next_Scene;
    public int exit_ID;



    private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Player")
        {
            StaticData.exit_Id = exit_ID;
            SceneManager.LoadScene(next_Scene);   
        }
    }
}
