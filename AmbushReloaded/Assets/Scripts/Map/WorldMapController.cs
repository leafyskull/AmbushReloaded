using Unity.VisualScripting;
using UnityEngine;

public class WorldMapController : MonoBehaviour
{
    // WorldMapController Instance
    public static WorldMapController Instance { get; private set; }

    // Prefab references
    [SerializeField] private GameObject _battleMapTilePrefab;

    // Private variables
    private WorldMapTile[,] tileGrid = new WorldMapTile[MAP_WIDTH, MAP_HEIGHT];

    // Map data
    // TODO: Maybe map class that WorldMapController (this) manages?
    const int MAP_HEIGHT = 8;
    const int MAP_WIDTH = 8;
    private const int ENEMY_SQUAD_COUNT = 2;
    private const int SUPPLIES_COUNT = 3;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set reference to Instance
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
        DontDestroyOnLoad(gameObject);

        // TODO: Have map generation handled by a GameController?
        GenerateWorldMap();
    }


    // ######## MAP GENERATION & SPAWNING ######## //

    // GenerateMap(): Generates the map for the level.
    public void GenerateWorldMap(){

        tileGrid = new WorldMapTile[MAP_WIDTH, MAP_HEIGHT];

        // Generate coordinates & tiles
        for (int y = 0; y < MAP_HEIGHT; y++){
            for (int x = 0; x < MAP_WIDTH; x++){

                Coordinate newCoordinate = new Coordinate(x, y);
                Vector3 position = new Vector3(x, y, 0);

                GameObject tileObject = Instantiate(
                    _battleMapTilePrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                tileGrid[x, y].SetCoordinate(newCoordinate);
                tileGrid[x, y] = tileObject.GetComponent<WorldMapTile>();
            }
        }
    }

    // SpawnEnemies(): Spawns the enemies for the map.
    public void SpawnEnemySquads(){
        // TODO: Implement
        // For now, just generate some placeholder enemies.

        Coordinate enemySpawnCoordinate = new Coordinate(0, 9);

        for (int i = 0; i < ENEMY_SQUAD_COUNT; i++){
            Enemy enemyToSpawn = new Enemy();
            enemyToSpawn.SetCoordinate(enemySpawnCoordinate);
            enemySpawnCoordinate = new Coordinate(i, 9);
        }
        
    }

    // SpawnSupplies(): Spawns the supplies for the map.
    public void SpawnSupplies(){
        // TODO: Figure out supplies spawning algorithm
        // For now, just spawn some supplies in front of the enemies.

        Coordinate suppliesSpawnCoordinate = new Coordinate(0, 8);

        for (int i = 0; i < SUPPLIES_COUNT; i++){
            Supplies suppliesToSpawn = new Supplies();

            this.tileGrid[suppliesSpawnCoordinate.X, suppliesSpawnCoordinate.Y].SetSupplies(suppliesToSpawn);
            suppliesSpawnCoordinate = new Coordinate(i, 8);
        }
    }

    // SpawnPlayer(): Spawns the player on the map.
    public void SpawnPlayerSquad(){
        // TODO: Implement
        // For now, just place the player on the bottom row.

        Coordinate playerSpawnCoordinate = new Coordinate(4, 0);

        Player player = new Player();
        player.SetCoordinate(playerSpawnCoordinate);
    }

}
