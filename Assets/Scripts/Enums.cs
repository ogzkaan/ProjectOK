using UnityEngine;

// TileColor.cs
public enum TileColor
{
    Red,
    Black,
    Blue,
    Yellow
}

// GameState.cs
public enum GameState
{
    Setup,
    DistributingTiles,
    ChoosingMiddleTile,
    PlayerTurn,
    GameEnd
}
