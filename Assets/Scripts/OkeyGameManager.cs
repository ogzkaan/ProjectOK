using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class OkeyGameManager : MonoBehaviour
{
    private const int INITIAL_TILES = 22;

    public static OkeyGameManager instance;

    [Header("Managers")]
    private PlayerManager playerManager;
    private SetChecker setChecker;
    [SerializeField] private TileVisualManager tileVisualManager;
    private TilePileManager tilePileManager;
    [SerializeField]private VisulizeTile visulizeTile;
    public GameState currentState;

    [Header("References")]
    [SerializeField] private Transform tileGrid;
    [SerializeField] private TextMeshProUGUI handScoreText;
    [SerializeField] private TextMeshPro targetScoreText;
    [SerializeField] private TextMeshProUGUI drawText;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private GameObject errorMessage;

    [Header("Deal Animation Settings")]
    [SerializeField] private Transform dealStartPosition;
    private const float DEAL_DURATION = 0.3f;
    private const float DEAL_DELAY = 0.1f;
    private bool isDealing = false;

    [Header("Open Animation Settings")]
    [SerializeField] private Transform openStartPosition;
    private float openOffset = 0.07f;

    [Header("Game Parameters")]
    [SerializeField] private int targetScore = 101;
    [SerializeField] private int drawCount;
    

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
        Cursor.visible = true;
    }
    private void Start()
    {
        InitializeManagers();
        InitializeGame();

        playerManager.onSortingComplete.AddListener(() => {
            Debug.Log("a");
            CheckSets();
        });

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
                
            }*/
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            
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
        SetupIndicatorAndOkey();
        StartDealAnimation();
        currentState = GameState.DistributingTiles;
        targetScoreText.text = targetScore.ToString();
        drawText.text = drawCount.ToString();
        SetCurrentState(currentState);
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
        CalculateScore();
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
        foreach (Tile tile in playerManager.Tiles)
        {
            if(tile.IsInSet)
            {
                if (tile.IsOkey)
                {
                    handScore += tile.OkeyNumber;
                }
                else
                {
                    handScore += tile.Number;
                }
                
            }
        }

        handScoreText.text = handScore.ToString();
        if (handScore >= targetScore)
        {
            targetScoreText.color = Color.green;
        }
    }
    public void SortByColor()
    {
        if (currentState == GameState.DistributingTiles)
        {
            return;
        }
        ResetSetAndHighlight();
        playerManager.SortTilesByColor();
    }
    public void SortByNumber()
    {
        if (currentState == GameState.DistributingTiles)
        {
            return;
        }
        ResetSetAndHighlight();
        playerManager.SortTilesByNumber();
    }
    public void OpenHand()
    {
        if (currentState == GameState.DistributingTiles)
        {
            return;
        }
        if (handScore >= targetScore)
        {
            DOTween.Kill("OpenHandAnimation");
            Sequence openHandSequence = DOTween.Sequence().SetId("OpenHandAnimation");

            foreach (Tile tile in PlayerManager.Tiles)
            {
                if (tile.IsInSet)
                {
                    int tileNumber = tile.Number;
                    TileColor tileColor = tile.Color;
                    if (tile.IsOkey)
                    {
                        tileNumber = tile.OkeyNumber;
                        tileColor = tile.OkeyColor;
                    }

                    tile.TileObject.transform.SetParent(null);

                    Vector3 targetPosition = GetTileOpenPosition(openStartPosition, tileColor, tileNumber);

                    // Animate position and rotation
                    openHandSequence.Insert(0, tile.TileObject.transform.DOJump(targetPosition,0.2f,1,0.5f)
                        .SetEase(Ease.OutQuad));

                    openHandSequence.Insert(0, tile.TileObject.transform.DORotate(new Vector3(0, 90, 0), 0.5f)
                        .SetEase(Ease.OutQuad));

                    PlayerManager.AddTileToRevealedSets(tile);
                    tile.TileObject.GetComponent<BoxCollider>().enabled = false;
                }
            }

            openHandSequence.OnComplete(() => {
                ResetSetAndHighlight();
                foreach (Tile tile in playerManager.RevealedSets)
                {
                    playerManager.RemoveTile(tile);
                }
            });
        }
    }
    private Vector3 GetTileOpenPosition(Transform startPosition, TileColor tileColor, int tileNumber)
    {
        float zOffset = 0;
        switch (tileColor)
        {
            case TileColor.Red:
                zOffset = 0f;
                break;
            case TileColor.Black:
                zOffset = 0.1f;
                break;
            case TileColor.Blue:
                zOffset = 0.2f;
                break;
            case TileColor.Yellow:
                zOffset = 0.3f;
                break;
        }

        return startPosition.transform.position + new Vector3(openOffset * tileNumber, 0, zOffset);
    }
    private void StartDealAnimation()
    {

        isDealing = true;
        DOTween.Kill("DealAnimation");
        Sequence dealSequence = DOTween.Sequence().SetId("DealAnimation");

        var emptyHolders = TileHolderList.Where(holder => holder.childCount == 0).ToList();
        int holderIndex = 0;

        for (int i = 0; i < INITIAL_TILES; i++)
        {
            Tile tile = tilePileManager.DrawTile();
            if (tile != null && holderIndex < emptyHolders.Count)
            {
                // Check if this tile is an okey before adding it
                if (tile.Number == okeyTile.Number && tile.Color == okeyTile.Color)
                {
                    tile.IsOkey = true;
                }

                playerManager.AddTile(tile);

                // Create the visual tile
                GameObject tileObj = visulizeTile.CreateTile(tile, null);
                tile.TileObject = tileObj;

                // Set initial position
                tileObj.transform.position = dealStartPosition.position;
                tileObj.transform.rotation = dealStartPosition.rotation;

                Transform targetHolder = emptyHolders[holderIndex];
                float delay = i * DEAL_DELAY;

                // Create the dealing animation
                dealSequence.Insert(delay, tileObj.transform.DOMove(targetHolder.position, DEAL_DURATION)
                    .SetEase(Ease.OutQuad));

                // Different rotation animation for okey tiles
                if (tile.IsOkey)
                {
                    // For okey tiles, rotate to 180 degrees
                    dealSequence.Insert(delay, tileObj.transform.DORotate(new Vector3(-51, 90, 0), DEAL_DURATION)
                        .SetEase(Ease.OutQuad));
                }
                else
                {
                    // For regular tiles, do a 360 spin
                    dealSequence.Insert(delay, tileObj.transform.DORotate(new Vector3(-51, 90, 0), DEAL_DURATION, RotateMode.Fast)
                        .SetEase(Ease.OutQuad));
                }

                // Set final position and parent
                int currentIndex = holderIndex;
                dealSequence.InsertCallback(delay + DEAL_DURATION, () =>
                {
                    tileObj.transform.SetParent(emptyHolders[currentIndex]);
                    tileObj.transform.localPosition = Vector3.zero;
                    if (tile.IsOkey)
                    {
                        tileObj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                    }
                    else
                    {
                        tileObj.transform.localRotation = Quaternion.identity;
                    }
                });

                holderIndex++;
            }
        }

        dealSequence.OnComplete(() =>
        {
            isDealing = false;
            CheckSets(); // Optional: Check sets after dealing is complete
            SetCurrentState(GameState.Discard);
        });
    }
    public bool IsDealing() => isDealing;
    public void ResetSetAndHighlight()
    {
        setChecker.ResetSetBool(playerManager);
        tileVisualManager.ResetHighlight(playerManager);
    }
    public GameState GetCurrentState()
    {
       return currentState;
    }
    public void SetCurrentState(GameState gameState)
    {
        currentState = gameState;
        turnText.text = currentState.ToString();
    }
    public void ErrorMessageControl()
    {
        Debug.Log("a");
        errorMessage.SetActive(true);
        StartCoroutine(DisableError());
        
    }
    IEnumerator DisableError()
    {
        yield return new WaitForSeconds(2f);
        errorMessage.SetActive(false);
    }
    
}