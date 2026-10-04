using System.Collections.Generic;
using UnityEngine;

// The world-map representation of a group of battle units.
public class Squad : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _playerSprite;
    [SerializeField] private Sprite _enemySprite;
    [SerializeField] private Team _team = Team.Player;
    [SerializeField] private List<Unit> _units = new List<Unit>();

    // Membership changes go through AddUnit/RemoveUnit so a unit has one owner.
    public IReadOnlyList<Unit> Units
    {
        get
        {
            _units.RemoveAll(unit => unit == null);
            return _units.AsReadOnly();
        }
    }
    public IReadOnlyList<Unit> units => Units;
    public int UnitCount => Units.Count;
    public Team CurrentTeam => _team;
    public WorldMapTile CurrentTile => GetComponentInParent<WorldMapTile>();

    private void Awake()
    {
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();

        // Register Inspector-assigned members through the same ownership checks.
        var initialUnits = new List<Unit>(_units);
        _units.Clear();
        foreach (Unit unit in initialUnits) AddUnit(unit);
        SetTeam(_team);
    }

    public bool AddUnit(Unit unit)
    {
        if (unit == null || _units.Contains(unit) ||
            (unit.CurrentSquad != null && unit.CurrentSquad != this)) return false;

        _units.Add(unit);
        unit.CurrentSquad = this;
        unit.SetTeam(CurrentTeam);
        return true;
    }

    // Removing membership does not destroy or reposition the battle unit.
    public bool RemoveUnit(Unit unit)
    {
        if (unit == null || !_units.Remove(unit)) return false;
        if (unit.CurrentSquad == this) unit.CurrentSquad = null;
        return true;
    }

    public void SetTeam(Team newTeam)
    {
        _team = newTeam;
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = newTeam == Team.Player ? _playerSprite : _enemySprite;
        // Keep the prefab's existing sprite when no team sprite is configured.
        if (_spriteRenderer != null && sprite != null) _spriteRenderer.sprite = sprite;

        foreach (Unit unit in Units) unit.SetTeam(newTeam);
    }

    // Use the controller so selection, PlayerTile, and ambush encounters stay in sync.
    public bool MoveToTile(WorldMapTile targetTile)
    {
        WorldMapTile source = CurrentTile;
        if (CurrentTeam != Team.Player || source == null || source.OccupyingSquad != this ||
            targetTile == null) return false;

        WorldMapController map = source.GetComponentInParent<WorldMapController>();
        if (map == null || map.PlayerTile != source || map.GetTile(source.Coordinate) != source ||
            map.GetTile(targetTile.Coordinate) != targetTile) return false;

        return map.TryMovePlayer(targetTile);
    }

    private void OnDestroy()
    {
        // Units may have been deployed under battle tiles and outlive this object.
        foreach (Unit unit in _units)
        {
            if (unit != null && unit.CurrentSquad == this) unit.CurrentSquad = null;
        }
        _units.Clear();
    }
}
