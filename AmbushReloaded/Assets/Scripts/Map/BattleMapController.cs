// BattleMapController.cs: This contains the class definition for the BattleMap.
//
// The BattleMap is where battles will take place between the player and enemy units.



using UnityEngine;

public class BattleMapController : MonoBehaviour
{
    public static BattleMapController Instance;

    // Prefab references
    [SerializeField] private GameObject _battleMapTilePrefab;

    // Private variables
    private BattleMapTile[,] tileGrid = new BattleMapTile[MAP_WIDTH, MAP_HEIGHT];

    // BattleMap data
    // TODO: Maybe map class that WorldMapController (this) manages?
    const int MAP_HEIGHT = 8;
    const int MAP_WIDTH = 8;
    private const int ENEMY_SQUAD_COUNT = 2;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set reference to Instance
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
        DontDestroyOnLoad(gameObject);

        // TODO: Have map generation handled by a GameController?
        GenerateBattleMap();
    }


    // ######## MAP GENERATION & SPAWNING ######## //

    // GenerateMap(): Generates the map for the level.
    public void GenerateBattleMap(){

        tileGrid = new BattleMapTile[MAP_WIDTH, MAP_HEIGHT];

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
                tileGrid[x, y] = tileObject.GetComponent<BattleMapTile>();
            }
        }
    }

    // SpawnEnemies(): Spawns the enemies for the map.
    public void SpawnEnemyUnits(){
        // TODO: Implement        
    }

    // SpawnPlayer(): Spawns the player on the map.
    public void SpawnPlayerUnits(){
        // TODO: Implement
    }

}