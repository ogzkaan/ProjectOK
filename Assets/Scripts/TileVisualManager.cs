using System.Collections.Generic;
using UnityEngine;

public class TileVisualManager : MonoBehaviour
{
    [SerializeField] private Material rainbow;
    [SerializeField] private Material defaultMat;

    public void SetHighlight(PlayerManager playerManager)
    {
        Debug.Log("a");
        ResetHighlight(playerManager);
        foreach (Tile tile in playerManager.Tiles)
        {
      
            if (tile.IsInSet)
            {
                GameObject tileObject = tile.TileObject;
                Renderer tileRenderer = tileObject.GetComponent<Renderer>();
                Material[] materials = tileRenderer.materials;
                materials[1] = rainbow;
                tileRenderer.materials = materials;
            }
        }
    }

    public void ResetHighlight(PlayerManager playerManager)
    {
        foreach (Tile tile in playerManager.Tiles)
        {
            if (tile.TileObject)
            {
                GameObject tileObject = tile.TileObject;
                Renderer tileRenderer = tileObject.GetComponent<Renderer>();
                Material[] materials = tileRenderer.materials;
                materials[1] = defaultMat;
                tileRenderer.materials = materials;
            }
        }
    }
}