using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ConveyorVisual : MonoBehaviour
{
    [SerializeField] private Conveyor conveyor;
    [SerializeField] private float scrollMultiplier = 0.2f;

    private Renderer rend;
    private Material mat;
    private float offsetX = 0f;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("ConveyorVisual: No Renderer found!");
            enabled = false;
            return;
        }

        mat = rend.material; // ensures unique material instance
        if (mat == null)
        {
            Debug.LogError("ConveyorVisual: Renderer has no material!");
            enabled = false;
        }

        if (conveyor == null)
        {
            Debug.LogError("ConveyorVisual: Conveyor reference is missing!");
            enabled = false;
        }
    }

    void Update()
    {
        if (conveyor == null || mat == null) return;

        // Determine direction (+1 right, -1 left)
        float dirSign = conveyor.GetDirSign();

        // Update scroll offset
        offsetX += conveyor.GetVelocity() * scrollMultiplier * Time.deltaTime;

        // Apply to material instance
        mat.SetFloat("_OffsetX", offsetX);
        mat.SetFloat("_FlipX", dirSign);
    }
}
