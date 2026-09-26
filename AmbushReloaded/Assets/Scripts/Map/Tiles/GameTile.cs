// GameTile.cs: The definition for the GameTile class.
//
// A GameTile is an abstract class that is used by:
// - BattleMapTile
// - WorldMapTile

using UnityEngine;

public abstract class GameTile
{
    // Tile properties
    public Coordinate Coordinate { get; private set; }


    // #### PUBLIC SETTERS #### //
    public void SetCoordinate(Coordinate newCoordinate) { this.Coordinate = newCoordinate; }

}
