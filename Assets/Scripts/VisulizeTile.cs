using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class VisulizeTile : MonoBehaviour
{

    public static VisulizeTile instance;

    [SerializeField] private GameObject tilePrefab;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    public GameObject CreateTile(Tile tile, Transform parent)
    {
        GameObject tileObject = Instantiate(tilePrefab, Vector3.zero,Quaternion.identity);
        tileObject.name= tile.ToString();
        var tileText = tileObject.GetComponentInChildren<TMP_Text>(); // Or TMP
        if (tileText != null)
            tileText.text = tile.Number.ToString();

        Renderer tileRenderer = tileObject.GetComponent<Renderer>();
        Material[] materials = tileRenderer.materials;
        if (tileRenderer != null)
        {
            switch (tile.Color)
            {
                case TileColor.Red:
                    materials[2].color = Color.red;
                    tileText.color = Color.red;
                    break;
                case TileColor.Black:
                    materials[2].color = Color.black;
                    tileText.color = Color.black;
                    break;
                case TileColor.Blue:
                    materials[2].color = Color.blue;
                    tileText.color = Color.blue;
                    break;
                case TileColor.Yellow:
                    materials[2].color = Color.yellow;
                    tileText.color = Color.yellow;
                    break;
            }
        }

        tileObject.transform.SetParent(parent);
        tileObject.transform.localPosition = Vector3.zero;
        tileObject.transform.localRotation = Quaternion.identity;
        TileDraggable tileDraggable = tileObject.GetComponent<TileDraggable>();
        tileDraggable.SetTile(tile);
        
        return tileObject;
    }
}
