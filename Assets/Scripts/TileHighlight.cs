using UnityEngine;

public class TileHighlight : MonoBehaviour
{
    private Material[] originalMaterials;
    private Renderer tileRenderer;
    private Color highlightColor = new Color(1f, 1f, 1f, 0.3f); // Subtle white glow
    private const int HIGHLIGHT_MATERIAL_INDEX = 1; // Index of the material we want to change

    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
        if (tileRenderer != null)
        {
            // Store all original materials
            originalMaterials = tileRenderer.materials;
        }
    }

    public void SetHighlight(bool highlight)
    {
        if (tileRenderer != null)
        {
            // Get current materials array
            Material[] materials = tileRenderer.materials;

            if (highlight)
            {
                // Create a new material only for the specific index
                Material highlightMaterial = new Material(materials[HIGHLIGHT_MATERIAL_INDEX]);
                highlightMaterial.EnableKeyword("_EMISSION");
                highlightMaterial.SetColor("_EmissionColor", highlightColor);
                materials[HIGHLIGHT_MATERIAL_INDEX] = highlightMaterial;
            }
            else
            {
                // Restore only the specific material
                materials[HIGHLIGHT_MATERIAL_INDEX] = originalMaterials[HIGHLIGHT_MATERIAL_INDEX];
            }

            // Apply the modified materials array
            tileRenderer.materials = materials;
        }
    }

    private void OnDestroy()
    {
        // Clean up materials
        if (tileRenderer != null)
        {
            Material[] currentMaterials = tileRenderer.materials;
            if (currentMaterials[HIGHLIGHT_MATERIAL_INDEX] != originalMaterials[HIGHLIGHT_MATERIAL_INDEX])
            {
                Destroy(currentMaterials[HIGHLIGHT_MATERIAL_INDEX]);
            }
        }
    }
}