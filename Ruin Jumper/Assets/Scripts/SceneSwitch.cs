using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    private Transform ColliderTransform;
    public string next_Scene;
    public int exit_ID;
    private PlayerMovement2D playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement2D>();
        Collider collider = ColliderTransform.GetChild(0).GetComponent<Collider>();
    }


    private void OnTriggerEnter(Collider other)
    {
        print("Beep");
        StaticData.exit_Id = exit_ID;
        playerMovement.SaveVelocity();
        SceneManager.LoadScene(next_Scene);
    }


}
