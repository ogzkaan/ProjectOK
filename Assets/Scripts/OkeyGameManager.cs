using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class OkeyGameManager : MonoBehaviour
{
    public static OkeyGameManager instance;

    private const int TILE_SET = 1;
    private const int INITIAL_TILES = 14;

    private GameState currentState;
    PlayerManager playerManager;
    VisulizeTile visulizeTile;

    private List<Tile> tilePile;
    private List<Tile> discardPile;

    private List<Transform> TileHolderList = new List<Transform>();
    [SerializeField] private Transform tileGrid;

    private Tile indicatorTile;
    private Tile okeyTile;
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
    private void Start()
    {
        playerManager = new PlayerManager(1);
        visulizeTile = GetComponent<VisulizeTile>();
        InitializeGame();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach(Tile tile in playerManager.Tiles)
            {
                Debug.Log("PlayerTiles: name" + tile.TileObject.name);
                
            }
            
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
    public void AddTileToPlayerHand(Tile tile)
    {
        playerManager.AddTile(tile);
    }
    private void SetupIndicatorAndOkey()
    {
        indicatorTile = DrawTile();
        // Okey is the next number after the indicator
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
                    break;
                }
            }
        }
    }


}
