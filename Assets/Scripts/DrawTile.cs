using UnityEngine;
using UnityEngine.InputSystem;

public class DrawTile : MonoBehaviour
{
    Camera mainCamera;
    Tile drawnTile;
    private void Start()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckDrag();
        }
        
    }
    private void CheckDrag()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (hit.rigidbody.transform.CompareTag("Draw"))
            {
                Tile tile = TileCreation();


                GameObject tileObject = tile.TileObject;
                TileDraggable tileDraggable = tileObject.GetComponent<TileDraggable>();

                // Trigger drag through TileDragManager
                TileDragManager.Instance.StartDragging(tileDraggable);
            }
        }
    }
    private Tile TileCreation()
    {
        drawnTile = OkeyGameManager.instance.DrawTile();

        // Add it to the player's hand
        OkeyGameManager.instance.AddTileToPlayerHand(drawnTile);

        // Place visual tiles and get the tile object
        OkeyGameManager.instance.PlaceVisualTiles();
        return drawnTile; // Ensure drawnTile.TileObject is properly instantiated
    }
}