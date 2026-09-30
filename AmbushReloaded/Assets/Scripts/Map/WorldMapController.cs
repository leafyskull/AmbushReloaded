using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class WorldMapController : MapController<WorldMapTile>
{
    public static WorldMapController Instance { get; private set; }

    [FormerlySerializedAs("_battleMapTilePrefab")]
    [SerializeField] private WorldMapTile _worldMapTilePrefab;
    [SerializeField] private Supplies _suppliesPrefab;
    [SerializeField] private PlayerSquad _playerSquadPrefab;
    [SerializeField] private EnemySquad _enemySquadPrefab;

    private const int ENEMY_SQUAD_COUNT = 2;
    private const int SUPPLIES_COUNT = 3;
    public WorldMapTile PlayerTile { get; private set; }

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

    public override void HandleTileClick(WorldMapTile tile)
    {
        // Only attempt movement for tiles belonging to this map.
        if (tile != null && GetTile(tile.Coordinate) == tile &&
            SelectedTile == PlayerTile && TryMovePlayer(tile)) return;

        base.HandleTileClick(tile);
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
        destination.SetOccupyingSquad(squad);
        PlayerTile = destination;
        SelectTile(destination);
        return true;
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

        PlayerTile = null;
        GenerateTiles(_worldMapTilePrefab);
        SpawnPlayerSquad();
        SpawnEnemySquads();
    }

    // SpawnEnemySquads(): Spawns the enemy squads for the map.
    public void SpawnEnemySquads()
    {
        if (_enemySquadPrefab == null)
        {
            Debug.LogError("Assign an EnemySquad prefab before spawning enemies.", this);
            return;
        }
        for (int x = 0; x < ENEMY_SQUAD_COUNT; x++)
        {
            WorldMapTile tile = GetTile(new Vector2Int(x, MAP_HEIGHT - 1));
            if (tile != null && !tile.IsOccupied)
            {
                tile.SetOccupyingSquad(Instantiate(_enemySquadPrefab, tile.transform));
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
        if (_playerSquadPrefab == null)
        {
            Debug.LogError("Assign a PlayerSquad prefab before spawning the player.", this);
            return;
        }
        WorldMapTile tile = GetTile(new Vector2Int(MAP_WIDTH / 2, 0));
        if (tile != null && !tile.IsOccupied)
        {
            tile.SetOccupyingSquad(Instantiate(_playerSquadPrefab, tile.transform));
            PlayerTile = tile;
        }
    }
}
