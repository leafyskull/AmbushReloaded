using UnityEngine;

// Keeps tile APIs strongly typed for each map while sharing grid behavior.
public abstract class MapController<TTile> : MonoBehaviour where TTile : GameTile
{
    protected const int MAP_HEIGHT = 8;
    protected const int MAP_WIDTH = 8;

    private TTile[,] tileGrid = new TTile[MAP_WIDTH, MAP_HEIGHT];
    public TTile SelectedTile { get; private set; }

    // Returns null for out-of-bounds coordinates or tiles not generated yet.
    public TTile GetTile(Vector2Int coordinate)
    {
        if (coordinate.x < 0 || coordinate.x >= MAP_WIDTH ||
            coordinate.y < 0 || coordinate.y >= MAP_HEIGHT)
        {
            return null;
        }

        return tileGrid[coordinate.x, coordinate.y];
    }

    public virtual void HandleTileClick(TTile tile)
    {
        if (tile != null && GetTile(tile.Coordinate) != tile) return;
        SelectTile(tile);
    }

    protected void SelectTile(TTile tile)
    {
        if (SelectedTile != null) SelectedTile.SetSelected(false);
        SelectedTile = tile;
        if (SelectedTile != null) SelectedTile.SetSelected(true);
    }

    // Call after validating the map's prefab. Child objects are removed with their tiles.
    protected void GenerateTiles(TTile prefab)
    {
        SelectTile(null);
        foreach (TTile tile in tileGrid)
        {
            if (tile != null)
            {
                tile.gameObject.SetActive(false);
                Destroy(tile.gameObject);
            }
        }

        tileGrid = new TTile[MAP_WIDTH, MAP_HEIGHT];
        for (int y = 0; y < MAP_HEIGHT; y++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                TTile tile = Instantiate(prefab, transform);
                tile.transform.localPosition = new Vector3(x, y, 0);
                tile.SetCoordinate(x, y);
                InitializeTile(tile);
                tileGrid[x, y] = tile;
            }
        }
    }

    protected virtual void InitializeTile(TTile tile) { }
}
