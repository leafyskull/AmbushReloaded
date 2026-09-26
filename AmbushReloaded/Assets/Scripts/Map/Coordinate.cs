// Coordinate.cs: Contains the class definition for a Coordinate.

using Unity.Collections;
using UnityEditor.UI;
using UnityEngine;

public class Coordinate
{

    public int X { get; private set; }
    public int Y { get; private set; }

    // #### SETTERS #### //
    private void setX(int newX) { this.X = newX; }
    private void setY(int newY) { this.Y = newY; }

    public Coordinate(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}
