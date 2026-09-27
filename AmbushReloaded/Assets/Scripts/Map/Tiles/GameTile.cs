// GameTile.cs: The definition for the GameTile class.
//
// A GameTile is an abstract class that is used by:
// - BattleMapTile
// - WorldMapTile

using System;
using UnityEngine;

public abstract class GameTile : MonoBehaviour
{
    // Tile properties
    public Vector2Int Coordinate { get; private set; }
    private SpriteRenderer tileRenderer;
    private Color originalColor;

    protected virtual void Awake()
    {
        // Set a default sprite/color
        tileRenderer = GetComponent<SpriteRenderer>();
        if (tileRenderer != null)
        {
            originalColor = tileRenderer.color;
        }

        // Ensure we have a collider for clicking
        if (GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }
    }

    // SetSelected(): Sets the GameTile to be selected.
    // This is triggered by clicking on the tile.
    public void SetSelected(bool selected)
    {
        if (tileRenderer != null)
        {
            tileRenderer.color = selected ? Color.yellow : originalColor;
        }
    }


    // #### PUBLIC SETTERS #### //
    public void SetCoordinate(int x, int y)
    {
        SetCoordinate(new Vector2Int(x, y));
    }

    public void SetCoordinate(Vector2Int coordinate)
    {
        Coordinate = coordinate;
    }

    // GetManhattanDistance(): Gets the distance between this GameTile and
    // a given GameTile.
    //
    // otherTile: The GameTile to find the distance from.
    //
    // Returns the manhattan distance as an integer.
    public int GetManhattanDistance(GameTile otherTile)
    {
        if (otherTile == null)
        {
            throw new ArgumentNullException(nameof(otherTile));
        }

        return Mathf.Abs(this.Coordinate.x - otherTile.Coordinate.x) + Mathf.Abs(this.Coordinate.y - otherTile.Coordinate.y);
    }
}
