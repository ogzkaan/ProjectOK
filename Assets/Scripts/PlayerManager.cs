using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class PlayerManager
{
    public int Id { get; private set; }
    public List<Tile> Tiles { get; private set; }
    public List<Tile> orderedTiles { get; private set; }
    public List<Tile> RevealedSets { get; private set; }
    private HashSet<Tile> UsedOkeys { get; set; }

    private const int MIN_SET_SIZE = 3;
    private const float ANIMATION_DURATION = 0.5f;
    private const float ANIMATION_DELAY = 0.05f;
    private const float HOVER_HEIGHT = 0.2f;

    public UnityEvent onSortingComplete = new UnityEvent();

    public PlayerManager(int id)
    {
        Id = id;
        Tiles = new List<Tile>();
        RevealedSets = new List<Tile>();
        UsedOkeys = new HashSet<Tile>();
    }

    public void AddTile(Tile tile)
    {
        Tiles.Add(tile);
    }

    public bool RemoveTile(Tile tile)
    {
        bool removed = Tiles.Remove(tile);
        return removed;
    }
    public void AddTileToRevealedSets(Tile tile)
    {
        RevealedSets.Add(tile);
    }

    public List<Tile> GetTilesOrderedByPosition()
    {
        return Tiles.OrderBy(t => {
            if (t.TileObject != null && t.TileObject.transform.parent != null)
            {
                return t.TileObject.transform.parent.GetSiblingIndex();
            }
            return float.MaxValue;
        }).ToList();
    }

    public void SortTilesByColor()
    {
        Tiles = Tiles.OrderBy(t => t.Color)
                    .ThenBy(t => t.Number)
                    .ToList();
        UpdateTilePositions();
    }
    public void SortTilesByNumber()
    {
        Tiles = Tiles.OrderBy(t => t.Number)
                    .ThenBy(t => t.Color)
                    .ToList();
        UpdateTilePositions();
    }
    private void UpdateTilePositions()
    {
        var tileHolders = GameObject.Find("TileGrid").GetComponentsInChildren<Transform>()
            .Where(t => t.parent != null && t.parent.name == "TileGrid")
            .ToList();

        // Kill any ongoing animations
        DOTween.Kill("TileSort");

        // Create a sequence for the animations
        Sequence sortSequence = DOTween.Sequence();
        sortSequence.SetId("TileSort");

        for (int i = 0; i < Tiles.Count; i++)
        {
            if (Tiles[i].TileObject != null && i < tileHolders.Count)
            {
                GameObject tileObj = Tiles[i].TileObject;
                Transform targetHolder = tileHolders[i];
                bool isOkey = Tiles[i].IsOkey;

                // Store original position
                Vector3 startPos = tileObj.transform.position;

                // Calculate intermediate and final positions
                Vector3 hoverPos = startPos + Vector3.up * HOVER_HEIGHT;
                Vector3 finalPos = targetHolder.position;

                // Create animation sequence for this tile
                float delay = i * ANIMATION_DELAY;

                // Movement animation
                sortSequence.Insert(delay, tileObj.transform.DOMove(hoverPos, ANIMATION_DURATION / 2)
                    .SetEase(Ease.OutQuad));

                sortSequence.Insert(delay + ANIMATION_DURATION / 2, tileObj.transform.DOMove(finalPos, ANIMATION_DURATION / 2)
                    .SetEase(Ease.InQuad));
                /*
                // Rotation animation
                if (!isOkey)
                {
                    // Regular tile rotation
                    sortSequence.Insert(delay, tileObj.transform.DORotate(new Vector3(0, 360, 0), ANIMATION_DURATION, RotateMode.FastBeyond360)
                        .SetEase(Ease.InOutQuad));
                }
                else
                {
                    // Okey tile rotation (maintain 180 degrees Y rotation)
                    sortSequence.Insert(delay, tileObj.transform.DORotate(new Vector3(0, 540, 0), ANIMATION_DURATION, RotateMode.FastBeyond360)
                        .SetEase(Ease.InOutQuad));
                }*/

                // Set the parent and final position at the end of the animation
                sortSequence.InsertCallback(delay + ANIMATION_DURATION, () =>
                {
                    tileObj.transform.SetParent(targetHolder);
                    tileObj.transform.localPosition = Vector3.zero;
                    tileObj.transform.localRotation = isOkey ?
                        Quaternion.Euler(0f, 180f, 0f) :
                        Quaternion.identity;
                });
            }
        }

        // Add completion callback
        sortSequence.OnComplete(() =>
        {
            onSortingComplete?.Invoke();
        });

    }
    public void CancelSorting()
    {
        DOTween.Kill("TileSort");
        var tileHolders = GameObject.Find("TileGrid").GetComponentsInChildren<Transform>()
            .Where(t => t.parent != null && t.parent.name == "TileGrid")
            .ToList();

        for (int i = 0; i < Tiles.Count; i++)
        {
            if (Tiles[i].TileObject != null && i < tileHolders.Count)
            {
                GameObject tileObj = Tiles[i].TileObject;
                Transform targetHolder = tileHolders[i];
                bool isOkey = Tiles[i].IsOkey;

                tileObj.transform.SetParent(targetHolder);
                tileObj.transform.localPosition = Vector3.zero;
                tileObj.transform.localRotation = isOkey ?
                    Quaternion.Euler(0f, 180f, 0f) :
                    Quaternion.identity;
            }
        }
    }
}