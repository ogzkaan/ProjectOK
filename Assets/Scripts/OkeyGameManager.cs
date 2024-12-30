using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.Unicode;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class OkeyGameManager : MonoBehaviour
{
    private const int INITIAL_TILES = 15;

    public static OkeyGameManager instance;

    [Header("Managers")]
    private PlayerManager playerManager;
    private SetChecker setChecker;
    [SerializeField] private TileVisualManager tileVisualManager;
    private TilePileManager tilePileManager;
    [SerializeField]private VisulizeTile visulizeTile;
    private GameState currentState;

    [Header("References")]
    [SerializeField] private Transform tileGrid;
    [SerializeField] private TextMeshProUGUI handScoreText;

    private List<Transform> TileHolderList = new List<Transform>();
    private List<Tile> tilePile;
    private Tile indicatorTile;
    private Tile okeyTile;
    private int handScore = 0;
    public PlayerManager PlayerManager => playerManager;
    private void Awake()
    {
        SetupSingleton();

        Application.targetFrameRate = 144;
    }
    private void Start()
    {
        InitializeManagers();
        InitializeGame();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            /*foreach(Tile tile in playerManager.Tiles)
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
            Debug.Log("Player Tile Count" + playerManager.Tiles.Count);*/
            tileVisualManager.SetHighlight(playerManager);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            tileVisualManager.ResetHighlight(playerManager);
        }
    }
    private void SetupSingleton()
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
    private void InitializeGame()
    {
        GetAllTileHolders();
        tilePileManager.CreateTiles();
        tilePileManager.ShuffleTiles();
        DealInitialTiles();
        SetupIndicatorAndOkey();
        PlaceVisualTiles();
        currentState = GameState.PlayerTurn;
    }
    private void InitializeManagers()
    {
        playerManager = new PlayerManager(1);
        tilePileManager = new TilePileManager();
        setChecker = new SetChecker();
    }
    private void GetAllTileHolders()
    {
        foreach (Transform child in tileGrid)
        {
            TileHolderList.Add(child);
        }
    }
    private void DealInitialTiles()
    {

        for (int i = 0; i < INITIAL_TILES; i++)
        {
            Tile tile = tilePileManager.DrawTile();
            if (tile != null)
            {
                playerManager.AddTile(tile);
            }
        }
    }
    public void DiscardTile(TileDraggable tileDraggable)
    {
        Tile tile = tileDraggable.GetTile();
        tileDraggable.ResetCollider();

        // Handle physics and visual updates
        GameObject tileGameObject = tile.TileObject;
        if (tileGameObject != null)
        {
            SetupDiscardedTilePhysics(tileGameObject);
        }

        // Update game state
        tilePileManager.DiscardTile(tile);
        playerManager.RemoveTile(tile);

        // Check for valid sets after discard
        CheckSets();
    }
    public Tile DrawTile()
    {
        Tile tile = tilePileManager.DrawTile();
        if (tile != null)
        {
            CheckSets();
        }
        return tile;
    }
    public void CheckSets()
    {
        setChecker.CheckSets(playerManager, okeyTile);
        tileVisualManager.SetHighlight(playerManager);
    }
    private void SetupDiscardedTilePhysics(GameObject tileGameObject)
    {
        Vector3 worldPosition = tileGameObject.transform.position;
        tileGameObject.transform.SetParent(null);
        tileGameObject.transform.position = worldPosition;

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
    public void AddTileToPlayerHand(Tile tile)
    {
        playerManager.AddTile(tile);
    }
    private void SetupIndicatorAndOkey()
    {
        indicatorTile = tilePileManager.DrawTile();
        int okeyNumber = (indicatorTile.Number % 13) + 1;
        okeyTile = new Tile(okeyNumber, indicatorTile.Color);
    }
    public void PlaceVisualTiles()
    {
        foreach (Tile tile in playerManager.Tiles)
        {
            if (tile.TileObject != null) continue;

            foreach (Transform tileHolder in TileHolderList)
            {
                if (tileHolder.childCount < 1)
                {
                    GameObject createdTile = visulizeTile.CreateTile(tile, tileHolder);
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
    public void CalculateScore()
    {
        handScore = 0;
    }

}