using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    // Tile properties
    public Coordinate Coordinate { get; private set; }


    // #### PUBLIC SETTERS #### //
    public void SetCoordinate(Coordinate newCoordinate) { this.Coordinate = newCoordinate; }

}
