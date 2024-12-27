using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager
{
    public int Id { get; private set; }
    public List<Tile> Tiles { get; private set; }
    public List<List<Tile>> RevealedSets { get; private set; }

    private const int MIN_SET_SIZE = 3;

    public PlayerManager(int id)
    {
        Id = id;
        Tiles = new List<Tile>();
        RevealedSets = new List<List<Tile>>();
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

    public void CheckForSets()
    {
        ResetAllHighlights();

        // Get tiles sorted by their physical position
        var orderedTiles = GetTilesOrderedByPosition();

        // Check for adjacent groups and runs
        FindAdjacentSets(orderedTiles);
    }

    private List<Tile> GetTilesOrderedByPosition()
    {
        return Tiles.OrderBy(t => {
            if (t.TileObject != null && t.TileObject.transform.parent != null)
            {
                return t.TileObject.transform.parent.GetSiblingIndex();
            }
            return float.MaxValue;
        }).ToList();
    }

    private void FindAdjacentSets(List<Tile> orderedTiles)
    {
        for (int startIdx = 0; startIdx < orderedTiles.Count - 2; startIdx++)
        {
            // Try to find a run (same color, consecutive numbers)
            TryHighlightRun(orderedTiles, startIdx);

            // Try to find a group (same number, different colors)
            TryHighlightGroup(orderedTiles, startIdx);
        }
    }

    private void TryHighlightRun(List<Tile> orderedTiles, int startIdx)
    {
        List<Tile> currentRun = new List<Tile>();
        currentRun.Add(orderedTiles[startIdx]);

        TileColor color = orderedTiles[startIdx].Color;
        int currentNumber = orderedTiles[startIdx].Number;

        for (int i = startIdx + 1; i < orderedTiles.Count; i++)
        {
            Tile nextTile = orderedTiles[i];

            // Check if tiles are physically adjacent
            if (!AretilesAdjacent(currentRun.Last().TileObject, nextTile.TileObject))
                break;

            if (nextTile.Color == color && nextTile.Number == currentNumber + 1)
            {
                currentRun.Add(nextTile);
                currentNumber = nextTile.Number;
            }
            else
            {
                break;
            }
        }

        if (currentRun.Count >= MIN_SET_SIZE)
        {
            foreach (var tile in currentRun)
            {
                HighlightTile(tile.TileObject, true);
                SetRunOrSetBool(tile, true);
            }
        }
    }

    private void TryHighlightGroup(List<Tile> orderedTiles, int startIdx)
    {
        List<Tile> currentGroup = new List<Tile>();
        currentGroup.Add(orderedTiles[startIdx]);

        int targetNumber = orderedTiles[startIdx].Number;
        HashSet<TileColor> usedColors = new HashSet<TileColor> { orderedTiles[startIdx].Color };

        // Look ahead for adjacent tiles with same number but different colors
        for (int i = startIdx + 1; i < orderedTiles.Count; i++)
        {
            Tile nextTile = orderedTiles[i];

            // Skip if not adjacent to the last tile in our current group
            if (!AretilesAdjacent(currentGroup.Last().TileObject, nextTile.TileObject))
                break;

            // Check if this tile matches our group criteria
            if (nextTile.Number == targetNumber && !usedColors.Contains(nextTile.Color))
            {
                currentGroup.Add(nextTile);
                usedColors.Add(nextTile.Color);
            }
        }

        // Highlight if we found enough tiles
        if (currentGroup.Count >= MIN_SET_SIZE)
        {
            foreach (var tile in currentGroup)
            {
                HighlightTile(tile.TileObject, true);
                SetRunOrSetBool(tile, true);
            }
        }
    }

    private bool AretilesAdjacent(GameObject tile1, GameObject tile2)
    {
        if (tile1 == null || tile2 == null) return false;

        Transform holder1 = tile1.transform.parent;
        Transform holder2 = tile2.transform.parent;

        if (holder1 == null || holder2 == null) return false;

        // Get the indices in the grid
        int index1 = holder1.GetSiblingIndex();
        int index2 = holder2.GetSiblingIndex();

        // They're adjacent if they're next to each other
        return Mathf.Abs(index1 - index2) == 1;
    }

    private void ResetAllHighlights()
    {
        foreach (var tile in Tiles)
        {
            if (tile.TileObject != null)
            {
                HighlightTile(tile.TileObject, false);
                SetRunOrSetBool(tile, false);
            }
        }
    }

    private void HighlightTile(GameObject tileObject, bool highlight)
    {
        var highlightComponent = tileObject.GetComponent<TileHighlight>();
        if (highlightComponent == null)
        {
            highlightComponent = tileObject.AddComponent<TileHighlight>();
        }
        highlightComponent.SetHighlight(highlight);
    }
    private void SetRunOrSetBool(Tile tile,bool IsInRunOrSet)
    {
        tile.IsInRunOrSet = IsInRunOrSet;
    }
}