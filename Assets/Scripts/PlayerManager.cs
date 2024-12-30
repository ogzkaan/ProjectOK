using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager
{
    public int Id { get; private set; }
    public List<Tile> Tiles { get; private set; }
    public List<Tile> orderedTiles { get; private set; }
    public List<List<Tile>> RevealedSets { get; private set; }
    private HashSet<Tile> UsedOkeys { get; set; }

    private const int MIN_SET_SIZE = 3;

    public PlayerManager(int id)
    {
        Id = id;
        Tiles = new List<Tile>();
        RevealedSets = new List<List<Tile>>();
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
}