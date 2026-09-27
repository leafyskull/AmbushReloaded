using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class WorldMapController : MonoBehaviour
{
    public static WorldMapController Instance { get; private set; }

    [FormerlySerializedAs("_battleMapTilePrefab")]
    [SerializeField] private WorldMapTile _worldMapTilePrefab;
    [SerializeField] private Supplies _suppliesPrefab;

    private const int MAP_HEIGHT = 8;
    private const int MAP_WIDTH = 8;
    private const int ENEMY_SQUAD_COUNT = 2;
    private const int SUPPLIES_COUNT = 3;
    private WorldMapTile[,] tileGrid = new WorldMapTile[MAP_WIDTH, MAP_HEIGHT];
    public WorldMapTile SelectedTile { get; private set; }
    public WorldMapTile PlayerTile { get; private set; }
    private static readonly Color PlayerColor = new Color(0.2f, 0.65f, 1f);

    private void Update()
    {
        if (Instance != this) return;
        if (Mouse.current == null) return;
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            SelectTile(null);
            return;
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Camera camera = Camera.main;
        if (camera == null) return;
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        WorldMapTile tile = hit.collider != null ? hit.collider.GetComponentInParent<WorldMapTile>() : null;
        HandleTileClick(tile);
    }

    public void HandleTileClick(WorldMapTile tile)
    {
        if (tile == null)
        {
            SelectTile(null);
            return;
        }

        // Only accept tiles belonging to this map.
        if (GetTile(tile.Coordinate) != tile) return;
        if (SelectedTile == PlayerTile && TryMovePlayer(tile)) return;
        SelectTile(tile);
    }

    public bool TryMovePlayer(WorldMapTile destination)
    {
        if (PlayerTile == null || !PlayerTile.IsOccupied || destination == null ||
            GetTile(destination.Coordinate) != destination || destination.IsOccupied ||
            PlayerTile.GetManhattanDistance(destination) != 1)
        {
            return false;
        }

        Squad squad = PlayerTile.OccupyingSquad;
        PlayerTile.SetOccupyingSquad(null);
        destination.SetOccupyingSquad(squad);
        destination.ShowSquadMarker(PlayerColor);
        PlayerTile = destination;
        SelectTile(destination);
        return true;
    }

    private void SelectTile(WorldMapTile tile)
    {
        if (SelectedTile != null) SelectedTile.SetSelected(false);
        SelectedTile = tile;
        if (SelectedTile != null) SelectedTile.SetSelected(true);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (Instance == this)
        {
            GenerateWorldMap();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void GenerateWorldMap()
    {
        if (_worldMapTilePrefab == null)
        {
            Debug.LogError("Assign a WorldMapTile prefab before generating the world map.", this);
            return;
        }

        SelectTile(null);
        PlayerTile = null;
        // Supplies are children of their tiles, so regeneration also removes them.
        foreach (WorldMapTile tile in tileGrid)
        {
            if (tile != null)
            {
                tile.gameObject.SetActive(false);
                Destroy(tile.gameObject);
            }
        }

        tileGrid = new WorldMapTile[MAP_WIDTH, MAP_HEIGHT];
        for (int y = 0; y < MAP_HEIGHT; y++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                WorldMapTile tile = Instantiate(_worldMapTilePrefab, transform);
                tile.transform.localPosition = new Vector3(x, y, 0);
                tile.SetCoordinate(x, y);
                tileGrid[x, y] = tile;
            }
        }
        SpawnPlayerSquad();
        SpawnEnemySquads();
    }

    // GetTile(): Gets the tile at a specified coordinate.
    //
    // coordinate: The coordinate to get the tile for.
    //
    // Returns the WorldMapTile if found.
    // Returns null for out-of-bounds coordinates or tiles not generated yet.
    public WorldMapTile GetTile(Vector2Int coordinate)
    {
        if (coordinate.x < 0 || coordinate.x >= MAP_WIDTH ||
            coordinate.y < 0 || coordinate.y >= MAP_HEIGHT)
        {
            return null;
        }

        return tileGrid[coordinate.x, coordinate.y];
    }

    // SpawnEnemySquads(): Spawns the enemy squads for the map.
    public void SpawnEnemySquads()
    {
        // Placeholder squad data; squads are not implemented yet.
        for (int x = 0; x < ENEMY_SQUAD_COUNT; x++)
        {
            WorldMapTile tile = GetTile(new Vector2Int(x, MAP_HEIGHT - 1));
            if (tile != null && !tile.IsOccupied)
            {
                tile.SetOccupyingSquad(new Squad());
                tile.ShowSquadMarker(Color.red);
            }
        }
    }

    // SpawnSupplies(): Spawns the supplies for the map.
    public void SpawnSupplies()
    {
        if (_suppliesPrefab == null)
        {
            Debug.LogError("Assign a Supplies prefab before spawning supplies.", this);
            return;
        }

        for (int x = 0; x < SUPPLIES_COUNT; x++)
        {
            WorldMapTile tile = GetTile(new Vector2Int(x, MAP_HEIGHT - 2));
            if (tile == null || tile.Supplies != null)
            {
                continue;
            }

            Supplies supplies = Instantiate(_suppliesPrefab, tile.transform);
            supplies.transform.localPosition = Vector3.zero;
            tile.SetSupplies(supplies);
        }
    }

    // SpawnPlayerSquad(): Spawns the player's squad on the map.
    public void SpawnPlayerSquad()
    {
        if (PlayerTile != null) return;
        WorldMapTile tile = GetTile(new Vector2Int(MAP_WIDTH / 2, 0));
        if (tile != null && !tile.IsOccupied)
        {
            tile.SetOccupyingSquad(new Squad());
            tile.ShowSquadMarker(PlayerColor);
            PlayerTile = tile;
        }
    }
}
