using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class TileVisualManager : MonoBehaviour
{
    [SerializeField] private Material rainbow;
    [SerializeField] private Material defaultMat;
    public void SetHighlight(PlayerManager playerManager)
    {
        ResetHighlight(playerManager);
        foreach (Tile tile in playerManager.Tiles)
        {
      
            if (tile.IsInSet)
            {
                GameObject tileObject = tile.TileObject;
                Renderer tileRenderer = tileObject.GetComponent<Renderer>();
                Material[] materials = tileRenderer.materials;
                DOTween.To(
                    () => 0f,
                    x => {
                        materials[1] = MaterialLerp(defaultMat, rainbow, x);
                        tileRenderer.materials = materials;
                    },
                    1f,
                    0.3f
                ).SetEase(Ease.OutQuad);

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
    private Material MaterialLerp(Material from, Material to, float t)
    {
        // Create a new material that interpolates between two materials
        Material lerpedMaterial = new Material(from);
        lerpedMaterial.Lerp(from, to, t);
        return lerpedMaterial;
    }
}