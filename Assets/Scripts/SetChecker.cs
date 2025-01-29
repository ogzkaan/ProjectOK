using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetChecker
{
    public void CheckSets(PlayerManager playerManager, Tile okeyTile)
    {
        ResetSetBool(playerManager);
        List<List<Tile>> allGroups = new List<List<Tile>>();
        List<List<Tile>> allRuns = new List<List<Tile>>();
        var playerTiles = playerManager.GetTilesOrderedByPosition();

        for (int startIdx = 0; startIdx < playerTiles.Count; startIdx++)
        {
            if (playerTiles[startIdx].IsOkey) continue;

            CheckForAdjacentGroup(playerTiles, startIdx, allGroups);
            CheckForAdjacentRun(playerTiles, startIdx, allRuns);
        }
    }
    private void CheckForAdjacentGroup(List<Tile> playerTiles, int startIdx, List<List<Tile>> allGroups)
    {
        List<Tile> currentGroup = new List<Tile> { playerTiles[startIdx] };
        HashSet<TileColor> usedColors = new HashSet<TileColor> { playerTiles[startIdx].Color };
        int targetNumber = playerTiles[startIdx].Number;

        // Check subsequent adjacent tiles
        for (int j = startIdx + 1; j < playerTiles.Count; j++)
        {
            if (!AreAdjacent(currentGroup.Last(), playerTiles[j]))
                break;

            if (playerTiles[j].IsOkey)
            {
                currentGroup.Add(playerTiles[j]);
            }
            else if (playerTiles[j].Number == targetNumber && !usedColors.Contains(playerTiles[j].Color))
            {
                currentGroup.Add(playerTiles[j]);
                usedColors.Add(playerTiles[j].Color);
            }
            else
            {
                break;
            }
        }

        if (currentGroup.Count >= 3 && IsValidGroup(currentGroup))
        {

            for (int i = 0; i < currentGroup.Count; i++)
            {
                Tile tile = currentGroup[i];
                tile.IsInSet = true;
                if (tile.IsOkey)
                {
                    Tile prevTile = currentGroup[i - 1];
                    tile.OkeyNumber = prevTile.Number;
                }
            }
            allGroups.Add(currentGroup);
        }
    }
    private void CheckForAdjacentRun(List<Tile> playerTiles, int startIdx, List<List<Tile>> allRuns)
    {
        List<Tile> currentRun = new List<Tile> { playerTiles[startIdx] };
        TileColor runColor = playerTiles[startIdx].Color;
        int currentNumber = playerTiles[startIdx].Number;

        // Check subsequent adjacent tiles
        for (int j = startIdx + 1; j < playerTiles.Count; j++)
        {
            if (!AreAdjacent(currentRun.Last(), playerTiles[j]))
                break;

            Tile nextTile = playerTiles[j];

            if (nextTile.IsOkey && currentNumber < 13)
            {
                currentRun.Add(nextTile);
                currentNumber++;
            }
            else if (nextTile.Color == runColor && nextTile.Number == currentNumber + 1)
            {
                currentRun.Add(nextTile);
                currentNumber = nextTile.Number;
            }
            else
            {
                break;
            }
        }

        if (currentRun.Count >= 3 && IsValidRun(currentRun))
        {
            for (int i = 0; i < currentRun.Count; i++)
            {
                Tile tile = currentRun[i];
                tile.IsInSet = true;
                if (tile.IsOkey)
                {
                    Tile prevTile=currentRun[i-1];
                    tile.OkeyNumber = prevTile.Number+1;
                    tile.OkeyColor =  prevTile.Color;
                }
            }

            allRuns.Add(currentRun);
        }
    }
    private bool IsValidGroup(List<Tile> tiles)
    {
        if (tiles.Count < 3 || tiles.Count > 4) return false;

        var firstNonOkey = tiles.FirstOrDefault(t => !t.IsOkey);
        if (firstNonOkey == null) return false;

        int number = firstNonOkey.Number;
        HashSet<TileColor> colors = new HashSet<TileColor>();

        foreach (var tile in tiles)
        {
            if (tile.IsOkey) continue;
            if (tile.Number != number) return false;
            if (colors.Contains(tile.Color)) return false;
            colors.Add(tile.Color);
        }

        return true;
    }
    private bool IsValidRun(List<Tile> tiles)
    {
        if (tiles.Count < 3) return false;

        var firstNonOkey = tiles.FirstOrDefault(t => !t.IsOkey);
        if (firstNonOkey == null) return false;

        TileColor runColor = firstNonOkey.Color;
        int expectedNumber = firstNonOkey.Number;

        foreach (var tile in tiles)
        {
            if (tile.IsOkey)
            {
                expectedNumber++;
                continue;
            }

            if (tile.Color != runColor || tile.Number != expectedNumber)
                return false;

            expectedNumber++;
        }

        return true;
    }
    private bool AreAdjacent(Tile tile1, Tile tile2)
    {
        if (tile1.TileObject == null || tile2.TileObject == null)
            return false;

        Transform holder1 = tile1.TileObject.transform.parent;
        Transform holder2 = tile2.TileObject.transform.parent;

        if (holder1 == null || holder2 == null)
            return false;

        int index1 = holder1.GetSiblingIndex();
        int index2 = holder2.GetSiblingIndex();

        return Mathf.Abs(index1 - index2) == 1;
    }
    public void ResetSetBool(PlayerManager playerManager)
    {
        foreach (Tile tile in playerManager.Tiles)
        {
            tile.IsInSet = false;
        }
    }
}