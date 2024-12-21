using System.Collections.Generic;
using UnityEngine;

public class PlayerManager 
{
    public int Id { get; private set; }
    public List<Tile> Tiles { get; private set; }
    public List<List<Tile>> RevealedSets { get; private set; }

    public PlayerManager(int id)
    {
        Id = id;
        Tiles = new List<Tile>();
        RevealedSets = new List<List<Tile>>();
    }

    public void AddTile(Tile tile) => Tiles.Add(tile);
    public bool RemoveTile(Tile tile) => Tiles.Remove(tile);
}
