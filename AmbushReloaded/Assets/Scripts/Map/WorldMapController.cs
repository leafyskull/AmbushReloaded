using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class WorldMapController : MapController<WorldMapTile>
{
    public static WorldMapController Instance { get; private set; }

    [FormerlySerializedAs("_battleMapTilePrefab")]
    [SerializeField] private WorldMapTile _worldMapTilePrefab;
    [SerializeField] private Supplies _suppliesPrefab;
    [SerializeField] private Squad _squadPrefab;
    [SerializeField] private string _battleSceneName = "BattleMapScene";
    [SerializeField, Min(0)] private int _shotOpeningDamage = 1;
    [SerializeField, Min(1)] private int _grenadeDirectOpeningDamage = 3;
    [SerializeField, Min(0)] private int _grenadeSplashOpeningDamage = 1;

    private const int ENEMY_SQUAD_COUNT = 2;
    private const int SUPPLIES_COUNT = 3;
    public WorldMapTile PlayerTile { get; private set; }
    public UnitAction SelectedAction { get; private set; } = UnitAction.MOVE;
    private bool transitioning;
    private string actionMessage = "Select your squad to act.";
    private Rect ActionPanelRect => new Rect(10, Screen.height - 180, 360, 170);
    private static readonly Vector2Int[] AdjacentOffsets =
    {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };

    private void Update()
    {
        if (Instance != this || transitioning) return;
        Keyboard keyboard = Keyboard.current;
        if ((keyboard != null && keyboard.escapeKey.wasPressedThisFrame) ||
            (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame))
        {
            SelectTile(null);
            return;
        }
        if (SelectedTile == PlayerTile && PlayerTile != null && keyboard != null)
        {
            if (keyboard.mKey.wasPressedThisFrame) SetSelectedAction(UnitAction.MOVE);
            if (keyboard.sKey.wasPressedThisFrame) SetSelectedAction(UnitAction.SHOOT);
            if (keyboard.gKey.wasPressedThisFrame) SetSelectedAction(UnitAction.THROW_GRENADE);
        }
        if (Mouse.current == null) return;

        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        Vector2 position = Mouse.current.position.ReadValue();
        if (SelectedTile == PlayerTile && PlayerTile != null &&
            ActionPanelRect.Contains(new Vector2(position.x, Screen.height - position.y))) return;

        Camera camera = Camera.main;
        if (camera == null) return;
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        WorldMapTile tile = hit.collider != null ? hit.collider.GetComponentInParent<WorldMapTile>() : null;
        HandleTileClick(tile);
    }

    public override void HandleTileClick(WorldMapTile tile)
    {
        if (transitioning || (tile != null && GetTile(tile.Coordinate) != tile)) return;
        if (tile == null) { SelectTile(null); return; }
        if (tile == PlayerTile)
        {
            if (SelectedTile != PlayerTile) SetSelectedAction(UnitAction.MOVE);
            SelectTile(tile);
            return;
        }
        if (SelectedTile != PlayerTile || PlayerTile == null)
        {
            base.HandleTileClick(tile);
            return;
        }

        bool succeeded = SelectedAction == UnitAction.MOVE ? TryMovePlayer(tile) :
            SelectedAction == UnitAction.SHOOT ? TryShoot(tile) : TryThrowGrenade(tile);
        if (succeeded && !transitioning) actionMessage = $"{SelectedAction} at {tile.Coordinate}. No encounter.";
        else if (!succeeded && !transitioning) actionMessage = "Invalid target, or battle scene is unavailable. Check the Console.";
    }

    public void SetSelectedAction(UnitAction action)
    {
        if (action != UnitAction.MOVE && action != UnitAction.SHOOT && action != UnitAction.THROW_GRENADE) return;
        SelectedAction = action;
        actionMessage = action == UnitAction.MOVE ? "Click an empty adjacent tile. Ambush zones are hidden." :
            action == UnitAction.SHOOT ? "Click a tile to shoot an enemy squad." :
            "Click a tile. Direct hits deal more damage than adjacent hits.";
    }

    private bool IsValidTarget(WorldMapTile tile) => !transitioning && PlayerTile != null &&
        PlayerTile.IsOccupied && tile != null && GetTile(tile.Coordinate) == tile;

    public bool TryShoot(WorldMapTile target)
    {
        if (!IsValidTarget(target) || target == PlayerTile) return false;
        if (!target.IsOccupied || target.OccupyingSquad.CurrentTeam != Team.Enemy) return true;
        return TryStartBattle(new List<BattleEncounter.EnemyGroup>
        {
            new BattleEncounter.EnemyGroup(target.Coordinate, _shotOpeningDamage)
        }, UnitAction.SHOOT);
    }

    public bool TryThrowGrenade(WorldMapTile target)
    {
        if (!IsValidTarget(target)) return false;
        var enemies = new List<BattleEncounter.EnemyGroup>();
        for (int y = 0; y < MAP_HEIGHT; y++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                WorldMapTile tile = GetTile(new Vector2Int(x, y));
                if (tile == null || !tile.IsOccupied || tile.OccupyingSquad.CurrentTeam != Team.Enemy) continue;
                int distance = target.GetManhattanDistance(tile);
                if (distance > 1) continue;
                int splashDamage = Mathf.Max(0, _grenadeSplashOpeningDamage);
                int damage = distance == 0 ? Mathf.Max(splashDamage + 1, _grenadeDirectOpeningDamage) : splashDamage;
                enemies.Add(new BattleEncounter.EnemyGroup(tile.Coordinate, damage));
            }
        }
        return enemies.Count == 0 || TryStartBattle(enemies, UnitAction.THROW_GRENADE);
    }

    private bool CanLoadBattle()
    {
        if (Application.CanStreamedLevelBeLoaded(_battleSceneName)) return true;
        Debug.LogError($"Add '{_battleSceneName}' to the enabled scenes in Build Settings before starting an encounter.", this);
        return false;
    }

    private bool TryStartBattle(List<BattleEncounter.EnemyGroup> enemies, UnitAction trigger)
    {
        if (transitioning || enemies.Count == 0 || !CanLoadBattle()) return false;
        BattleEncounterTransfer.Set(new BattleEncounter(PlayerTile.Coordinate, trigger, enemies));
        transitioning = true;
        SceneManager.LoadScene(_battleSceneName, LoadSceneMode.Single);
        return true;
    }

    private void OnGUI()
    {
        if (Instance != this || transitioning || PlayerTile == null || SelectedTile != PlayerTile) return;
        GUILayout.BeginArea(ActionPanelRect, GUI.skin.box);
        GUILayout.Label($"Squad {PlayerTile.Coordinate}");
        GUILayout.BeginHorizontal();
        if (GUILayout.Toggle(SelectedAction == UnitAction.MOVE, "Move (M)", GUI.skin.button) && SelectedAction != UnitAction.MOVE) SetSelectedAction(UnitAction.MOVE);
        if (GUILayout.Toggle(SelectedAction == UnitAction.SHOOT, "Shoot (S)", GUI.skin.button) && SelectedAction != UnitAction.SHOOT) SetSelectedAction(UnitAction.SHOOT);
        if (GUILayout.Toggle(SelectedAction == UnitAction.THROW_GRENADE, "Grenade (G)", GUI.skin.button) && SelectedAction != UnitAction.THROW_GRENADE) SetSelectedAction(UnitAction.THROW_GRENADE);
        GUILayout.EndHorizontal();
        GUILayout.Label(actionMessage);
        GUILayout.Label("Right-click or Esc to deselect.");
        GUILayout.EndArea();
    }

    public bool TryMovePlayer(WorldMapTile destination)
    {
        if (!IsValidTarget(destination) || destination.IsOccupied ||
            PlayerTile.GetManhattanDistance(destination) != 1)
        {
            return false;
        }

        var enemies = new List<BattleEncounter.EnemyGroup>();
        foreach (Squad enemy in destination.AmbushSquads)
        {
            if (enemy == null || enemy.CurrentTeam != Team.Enemy) continue;
            WorldMapTile enemyTile = enemy.GetComponentInParent<WorldMapTile>();
            if (enemyTile != null && GetTile(enemyTile.Coordinate) == enemyTile && enemyTile.OccupyingSquad == enemy)
                enemies.Add(new BattleEncounter.EnemyGroup(enemyTile.Coordinate, 0));
        }
        if (enemies.Count > 0 && !CanLoadBattle()) return false;

        Squad squad = PlayerTile.OccupyingSquad;
        destination.SetOccupyingSquad(squad);
        PlayerTile = destination;
        SelectTile(destination);
        if (enemies.Count > 0) return TryStartBattle(enemies, UnitAction.MOVE);
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
        transitioning = false;
        SelectedAction = UnitAction.MOVE;
        GenerateTiles(_worldMapTilePrefab);
        SpawnPlayerSquad();
        SpawnEnemySquads();
    }

    // Returns null if the prefab is missing or the tile cannot accept a squad.
    public Squad SpawnSquad(WorldMapTile spawnTile, Team team)
    {
        if (_squadPrefab == null)
        {
            Debug.LogError("Assign a Squad prefab before spawning squads.", this);
            return null;
        }

        if (spawnTile == null || GetTile(spawnTile.Coordinate) != spawnTile ||
            spawnTile.IsOccupied)
        {
            return null;
        }

        Squad squad = Instantiate(_squadPrefab, spawnTile.transform);
        squad.SetTeam(team);
        spawnTile.SetOccupyingSquad(squad);
        return squad;
    }

    // SpawnEnemySquads(): Spawns the enemy squads for the map.
    public void SpawnEnemySquads()
    {
        for (int x = 0; x < ENEMY_SQUAD_COUNT; x++)
        {
            // Interior positions leave four possible ambush neighbors per enemy.
            SpawnSquad(GetTile(new Vector2Int(2 + x * 3, MAP_HEIGHT - 2)), Team.Enemy);
        }
        GenerateAmbushTiles();
    }

    public void GenerateAmbushTiles()
    {
        for (int y = 0; y < MAP_HEIGHT; y++)
            for (int x = 0; x < MAP_WIDTH; x++)
                GetTile(new Vector2Int(x, y))?.ClearAmbush();

        for (int y = 0; y < MAP_HEIGHT; y++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                WorldMapTile enemyTile = GetTile(new Vector2Int(x, y));
                if (enemyTile == null || !enemyTile.IsOccupied || enemyTile.OccupyingSquad.CurrentTeam != Team.Enemy) continue;
                var candidates = new List<WorldMapTile>();
                foreach (Vector2Int offset in AdjacentOffsets)
                {
                    WorldMapTile neighbor = GetTile(enemyTile.Coordinate + offset);
                    if (neighbor != null && !neighbor.IsOccupied) candidates.Add(neighbor);
                }
                int count = Mathf.Min(Random.Range(2, 4), candidates.Count);
                for (int i = 0; i < count; i++)
                {
                    int index = Random.Range(0, candidates.Count);
                    candidates[index].AddAmbushSquad(enemyTile.OccupyingSquad);
                    candidates.RemoveAt(index);
                }
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
        if (SpawnSquad(tile, Team.Player) != null)
        {
            PlayerTile = tile;
        }
    }
}
