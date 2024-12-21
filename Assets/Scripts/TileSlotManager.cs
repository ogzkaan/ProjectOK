using UnityEngine;

public class TileSlotManager : MonoBehaviour
{
    
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material highlightMaterial;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public bool CanAcceptTile()
    {
        // Check if slot is empty or has special conditions
        return transform.childCount == 0;
    }

    public void Highlight(bool highlight)
    {
       meshRenderer.material = highlight ? highlightMaterial : normalMaterial;
    }
    
}
