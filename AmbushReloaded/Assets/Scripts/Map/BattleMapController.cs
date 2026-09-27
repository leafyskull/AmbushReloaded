using UnityEngine;

public class BattleMapController : MonoBehaviour
{
    public static BattleMapController Instance { get; private set; }

    [SerializeField] private BattleMapTile _battleMapTilePrefab;

    private const int MAP_HEIGHT = 8;
    private const int MAP_WIDTH = 8;
    private BattleMapTile[,] tileGrid = new BattleMapTile[MAP_WIDTH, MAP_HEIGHT];

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

        // Remove the previous tiles and their child objects when regenerating.
        foreach (BattleMapTile tile in tileGrid)
        {
            if (tile != null)
            {
                tile.gameObject.SetActive(false);
                Destroy(tile.gameObject);
            }
        }

        tileGrid = new BattleMapTile[MAP_WIDTH, MAP_HEIGHT];
        for (int y = 0; y < MAP_HEIGHT; y++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                BattleMapTile tile = Instantiate(_battleMapTilePrefab, transform);
                tile.transform.localPosition = new Vector3(x, y, 0);
                tile.SetCoordinate(x, y);

                BattleMapTerrain terrain = new BattleMapTerrain();
                terrain.SetTerrain(BattleMapTerrainType.OPEN);
                tile.SetTerrain(terrain);
                tileGrid[x, y] = tile;
            }
        }
    }

    // Returns null for out-of-bounds coordinates or tiles not generated yet.
    public BattleMapTile GetTile(Vector2Int coordinate)
    {
        if (coordinate.x < 0 || coordinate.x >= MAP_WIDTH ||
            coordinate.y < 0 || coordinate.y >= MAP_HEIGHT)
        {
            return null;
        }

        return tileGrid[coordinate.x, coordinate.y];
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
