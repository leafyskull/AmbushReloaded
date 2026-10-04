using UnityEngine;

public class BattleMapController : MapController<BattleMapTile>
{
    public static BattleMapController Instance { get; private set; }

    [SerializeField] private BattleMapTile _battleMapTilePrefab;
    [SerializeField] private Unit _unitPrefab;

    const int NUM_ENEMY_UNITS = 3;
    const int NUM_PLAYER_UNITS = 3;

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
            GenerateBattleMap();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void GenerateBattleMap()
    {
        if (_battleMapTilePrefab == null)
        {
            Debug.LogError("Assign a BattleMapTile prefab before generating the battle map.", this);
            return;
        }

        GenerateTiles(_battleMapTilePrefab);
        SpawnPlayerUnits();
        SpawnEnemyUnits();
    }

    protected override void InitializeTile(BattleMapTile tile)
    {
        BattleMapTerrain terrain = new BattleMapTerrain();
        terrain.SetTerrain(BattleMapTerrainType.OPEN);
        tile.SetTerrain(terrain);
    }

    // Returns null if the prefab is missing or the tile cannot accept a unit.
    public Unit SpawnUnit(BattleMapTile spawnTile, Team team)
    {
        if (_unitPrefab == null)
        {
            Debug.LogError("Assign a Unit prefab before spawning units.", this);
            return null;
        }

        if (spawnTile == null || GetTile(spawnTile.Coordinate) != spawnTile ||
            spawnTile.IsOccupied)
        {
            return null;
        }

        Unit unit = Instantiate(_unitPrefab, spawnTile.transform);
        unit.transform.localPosition = Vector3.zero;
        unit.SetTeam(team);
        spawnTile.SetOccupyingUnit(unit);
        return unit;
    }

    public void SpawnEnemyUnits()
    {
        for (int x = 0; x < NUM_ENEMY_UNITS; x++)
        {
            SpawnUnit(GetTile(new Vector2Int(x, MAP_HEIGHT - 1)), Team.Enemy);
        }
    }

    public void SpawnPlayerUnits()
    {
        for (int x = 0; x < NUM_PLAYER_UNITS; x++)
        {
            SpawnUnit(GetTile(new Vector2Int(x, 0)), Team.Player);
        }
    }
}
