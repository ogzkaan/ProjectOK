using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class OkeyGameManager : MonoBehaviour
{
    public static OkeyGameManager instance;

    private const int TILE_SET = 1;
    private const int INITIAL_TILES = 21;

    private GameState currentState;
    public PlayerManager PlayerManager => playerManager;
    private PlayerManager playerManager;
    VisulizeTile visulizeTile;

    private List<Tile> tilePile;
    private List<Tile> discardPile;

    private List<Transform> TileHolderList = new List<Transform>();
    [SerializeField] private Transform tileGrid;
    [SerializeField] private TextMeshProUGUI handScoreText;

    private Tile indicatorTile;
    private Tile okeyTile;

    private int handScore = 0;
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

        Application.targetFrameRate = 144;
    }
    private void Start()
    {
        playerManager = new PlayerManager(1);
        visulizeTile = GetComponent<VisulizeTile>();
        InitializeGame();
        discardPile = new List<Tile>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach(Tile tile in playerManager.Tiles)
            {
                Debug.Log("PlayerTiles: name" + tile.TileObject.name);
                
            }
            foreach(Tile tile in discardPile)
            {
                Debug.Log("Discard Pile : " + tile.TileObject.name);
               
            }
            foreach(Tile tile in tilePile)
            {
                Debug.Log("Tile Pile :" + tile.ToString());
                
            }
            Debug.Log("Okey: " + okeyTile.ToString());
            Debug.Log("Indicator: " + indicatorTile.ToString());

            Debug.Log("Tile Pile Tile Count" + tilePile.Count);
            Debug.Log("Discard Tile Count" + discardPile.Count);
            Debug.Log("Player Tile Count" + playerManager.Tiles.Count);
        }
    }
    private void InitializeGame()
    {
        GetAllTileHolders();
        CreateTiles();

        ShuffleTiles();
        DealInitialTiles();

        SetupIndicatorAndOkey();
        PlaceVisualTiles();

        currentState = GameState.PlayerTurn;
    }
    private void GetAllTileHolders()
    {
        foreach (Transform child in tileGrid)
        {
            TileHolderList.Add(child);
        }
    }
    private void CreateTiles()
    {
        tilePile = new List<Tile>();
        foreach (TileColor color in System.Enum.GetValues(typeof(TileColor)))
        {
            // Create two sets of tiles for each color
            for (int set = 0; set < 1; set++)
            {
                for (int number = 1; number <= 13; number++)
                {
                    tilePile.Add(new Tile(number, color, set == 1));
                }
            }
        }
        Debug.Log("Created :" + tilePile.Count +"Tiles");
    }
    private void ShuffleTiles()
    {
        System.Random rng = new System.Random();
        int n = tilePile.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Tile temp = tilePile[k];
            tilePile[k] = tilePile[n];
            tilePile[n] = temp;
        }
    }
    private void DealInitialTiles()
    {
        
        for (int i = 0; i < INITIAL_TILES; i++)
        {
            Tile tile = DrawTile();
            if (tile != null)
            {
                playerManager.AddTile(tile);
            }
        }
    }
    public Tile DrawTile()
    {
        if (tilePile.Count == 0) return null;

        Tile tile = tilePile[0];
        tilePile.RemoveAt(0);
        return tile;
    }
    public void DiscardTile(TileDraggable tileDraggable)
    {
        {
            Tile tile = tileDraggable.GetTile();
            tileDraggable.ResetCollider();

            // Update game state
            discardPile.Add(tile);
            tilePile.Remove(tile);
            playerManager.RemoveTile(tile);

            // Get tile's GameObject
            GameObject tileGameObject = tile.TileObject;
            if (tileGameObject == null)
            {
                Debug.LogError("Tile GameObject is null!");
                return;
            }

            Vector3 worldPosition = tileGameObject.transform.position; // Save position
            tileGameObject.transform.SetParent(null);
            tileGameObject.transform.position = worldPosition; // Restore position

            // Apply physics
            Rigidbody rb = tileGameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
            else
            {
                Debug.LogError("Rigidbody not found on tile!");
            }

        }
    }
    public void AddTileToPlayerHand(Tile tile)
    {
        playerManager.AddTile(tile);
    }
    private void SetupIndicatorAndOkey()
    {
        indicatorTile = DrawTile();
        int okeyNumber = (indicatorTile.Number % 13) + 1;
        okeyTile = new Tile(okeyNumber, indicatorTile.Color);
    }
    public void PlaceVisualTiles()
    {
        foreach (Tile tile in playerManager.Tiles)
        {
            if (tile.TileObject != null)
            {
                continue;
            }
            foreach(Transform tileHolder in TileHolderList)
            {
                if (tileHolder.childCount<1)
                {
                    
                    GameObject createdTile=visulizeTile.CreateTile(tile, tileHolder);
                    tile.TileObject = createdTile;
                    if (tile.ToString() == GetOkeyTile().ToString())
                    {
                        tile.IsOkey = true;
                        tile.TileObject.transform.Rotate(new Vector3(0f, 180f, 0f));
                    }
                    break;
                }
            }
        }
    }
    public Tile GetOkeyTile()
    {
        return okeyTile;
    }
    public void CheckForSets()
    {
        playerManager.CheckForSets();
    }
    public void CalculateScore()
    {
        handScore = 0;
        foreach (Tile tile in PlayerManager.Tiles)
        {
            if (tile.IsInRunOrSet)
            {
                handScore += tile.Number;
            }
        }
        handScoreText.text = handScore.ToString();
    }

}
