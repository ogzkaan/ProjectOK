using System.Collections.Generic;
using UnityEngine;

public class TilePileManager
{
    private List<Tile> tilePile;
    private List<Tile> discardPile;

    public TilePileManager()
    {
        tilePile = new List<Tile>();
        discardPile = new List<Tile>();
    }

    public void CreateTiles()
    {
        foreach (TileColor color in System.Enum.GetValues(typeof(TileColor)))
        {
            for (int set = 0; set < 1; set++)
            {
                for (int number = 1; number <= 13; number++)
                {
                    tilePile.Add(new Tile(number, color, set == 1));
                }
            }
        }
    }

    public void ShuffleTiles()
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

    public Tile DrawTile()
    {
        if (tilePile.Count == 0) return null;

        Tile tile = tilePile[0];
        tilePile.RemoveAt(0);
        return tile;
    }

    public void DiscardTile(Tile tile)
    {
        discardPile.Add(tile);
        tilePile.Remove(tile);
    }
}