using UnityEngine;

public class BattleMapController : MapController<BattleMapTile>
{
    public static BattleMapController Instance { get; private set; }

    [SerializeField] private BattleMapTile _battleMapTilePrefab;

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
    }

    protected override void InitializeTile(BattleMapTile tile)
    {
        BattleMapTerrain terrain = new BattleMapTerrain();
        terrain.SetTerrain(BattleMapTerrainType.OPEN);
        tile.SetTerrain(terrain);
    }

    public void SpawnEnemyUnits()
    {
        // TODO: Instantiate unit prefabs and assign them with tile.SetOccupyingUnit(unit).
    }

    public void SpawnPlayerUnits()
    {
        // TODO: Instantiate unit prefabs and assign them with tile.SetOccupyingUnit(unit).
    }
}
