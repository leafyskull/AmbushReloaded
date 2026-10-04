using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BattleMapController : MapController<BattleMapTile>
{
    public static BattleMapController Instance { get; private set; }

    [SerializeField] private BattleMapTile _battleMapTilePrefab;
    [SerializeField] private Unit _unitPrefab;

    const int NUM_ENEMY_UNITS = 3;
    const int NUM_PLAYER_UNITS = 3;

    public Unit SelectedUnit { get; private set; }
    public UnitAction SelectedAction { get; private set; } = UnitAction.MOVE;
    private string actionMessage = "";
    private Rect ActionPanelRect => new Rect(10, Screen.height - 210, 320, 200);

    private void Update()
    {
        if (Instance != this) return;

        Keyboard keyboard = Keyboard.current;
        if ((keyboard != null && keyboard.escapeKey.wasPressedThisFrame) ||
            (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame))
        {
            ClearSelection();
            return;
        }

        if (SelectedUnit != null && keyboard != null)
        {
            if (keyboard.mKey.wasPressedThisFrame) SetSelectedAction(UnitAction.MOVE);
            if (keyboard.sKey.wasPressedThisFrame) SetSelectedAction(UnitAction.SHOOT);
            if (keyboard.gKey.wasPressedThisFrame) SetSelectedAction(UnitAction.THROW_GRENADE);
        }

        Mouse mouse = Mouse.current;
        if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        Vector2 position = mouse.position.ReadValue();
        if (SelectedUnit != null && ActionPanelRect.Contains(new Vector2(position.x, Screen.height - position.y))) return;

        Camera camera = Camera.main;
        if (camera == null) return;
        RaycastHit2D hit = Physics2D.GetRayIntersection(camera.ScreenPointToRay(position));
        BattleMapTile tile = hit.collider != null ? hit.collider.GetComponentInParent<BattleMapTile>() : null;
        HandleTileClick(tile);
    }

    public override void HandleTileClick(BattleMapTile tile)
    {
        if (tile != null && GetTile(tile.Coordinate) != tile) return;
        if (tile == null)
        {
            ClearSelection();
            return;
        }

        if (tile.IsOccupied && tile.OccupyingUnit.CurrentTeam == Team.Player)
        {
            if (SelectedUnit != tile.OccupyingUnit)
            {
                SelectedUnit = tile.OccupyingUnit;
                SetSelectedAction(UnitAction.MOVE);
            }
            SelectTile(tile);
            return;
        }

        if (SelectedUnit == null) return;
        bool succeeded;
        switch (SelectedAction)
        {
            case UnitAction.MOVE:
                succeeded = SelectedUnit.MoveToTile(tile);
                break;
            case UnitAction.SHOOT:
                succeeded = SelectedUnit.Shoot(tile);
                break;
            case UnitAction.THROW_GRENADE:
                succeeded = SelectedUnit.ThrowGrenade(tile);
                break;
            default:
                return;
        }

        if (SelectedUnit != null)
        {
            SelectTile(SelectedUnit.CurrentTile);
            actionMessage = succeeded ? $"{SelectedAction} at {tile.Coordinate}." : "Invalid target or no supplies remaining.";
        }
    }

    public void SetSelectedAction(UnitAction action)
    {
        if (action != UnitAction.MOVE && action != UnitAction.SHOOT && action != UnitAction.THROW_GRENADE) return;
        SelectedAction = action;
        actionMessage = action == UnitAction.MOVE ? "Click an empty adjacent walkable tile." :
            action == UnitAction.SHOOT ? "Click a tile to shoot." : "Click a tile. Splash damages nearby allies too.";
    }

    public void ClearSelection()
    {
        SelectedUnit = null;
        SelectedAction = UnitAction.MOVE;
        actionMessage = "";
        SelectTile(null);
    }

    private void OnGUI()
    {
        if (Instance != this || SelectedUnit == null) return;
        GUILayout.BeginArea(ActionPanelRect, GUI.skin.box);
        GUILayout.Label($"Unit {SelectedUnit.CurrentTile.Coordinate} | Health: {SelectedUnit.Health}");
        GUILayout.Label($"Ammo: {SelectedUnit.AmmoCount} | Grenades: {SelectedUnit.GrenadeCount}");
        GUILayout.BeginHorizontal();
        if (GUILayout.Toggle(SelectedAction == UnitAction.MOVE, "Move (M)", GUI.skin.button) && SelectedAction != UnitAction.MOVE) SetSelectedAction(UnitAction.MOVE);
        bool previousEnabled = GUI.enabled;
        GUI.enabled = previousEnabled && SelectedUnit.AmmoCount > 0;
        if (GUILayout.Toggle(SelectedAction == UnitAction.SHOOT, "Shoot (S)", GUI.skin.button) && SelectedAction != UnitAction.SHOOT) SetSelectedAction(UnitAction.SHOOT);
        GUI.enabled = previousEnabled && SelectedUnit.GrenadeCount > 0;
        if (GUILayout.Toggle(SelectedAction == UnitAction.THROW_GRENADE, "Grenade (G)", GUI.skin.button) && SelectedAction != UnitAction.THROW_GRENADE) SetSelectedAction(UnitAction.THROW_GRENADE);
        GUI.enabled = previousEnabled;
        GUILayout.EndHorizontal();
        GUILayout.Label(actionMessage, GUI.skin.box);
        GUILayout.Label("Right-click or Esc to deselect.");
        GUILayout.EndArea();
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
        Debug.Log("Spawning battle map...");

        if (_battleMapTilePrefab == null)
        {
            Debug.LogError("Assign a BattleMapTile prefab before generating the battle map.", this);
            return;
        }

        ClearSelection();
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
        Debug.Log("Spawning enemy units...");
        for (int x = 0; x < NUM_ENEMY_UNITS; x++)
        {
            SpawnUnit(GetTile(new Vector2Int(x, MAP_HEIGHT - 1)), Team.Enemy);
        }
    }

    public void SpawnPlayerUnits()
    {
        Debug.Log("Spawning player units...");
        for (int x = 0; x < NUM_PLAYER_UNITS; x++)
        {
            SpawnUnit(GetTile(new Vector2Int(x, 0)), Team.Player);
        }
    }
}
