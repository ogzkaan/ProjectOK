using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile
{
    public int Number { get; private set; }    // 1-13
    public TileColor Color { get; private set; }  // Red, Black, Blue, Yellow
    public bool IsFake { get; private set; }      // Each color has 2 sets of tiles
    public GameObject TileObject { get; set; }

    public Tile (int number, TileColor color, bool isFake = false)
    {
        Number = number;
        Color = color;
        IsFake = isFake;
    }
    public override string ToString()
    {
        if (IsFake)
        {
            return $"Tile: Joker";
        }
        else
        {
            return $"Tile: {Color} {Number}";
        }
    }

}
