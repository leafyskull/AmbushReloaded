// Unit.cs: Contains the class definition for a Unit.
//
// A unit represents a single 'soldier', and will appear on the BattleMap.

using UnityEngine;

public class Unit : MonoBehaviour
{
    // Sprite rendering
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Sprite _playerSprite;
    [SerializeField] Sprite _enemySprite;

    [SerializeField, Min(1)] private int _shotDamage = 1;
    [SerializeField, Min(1)] private int _grenadeDamage = 3;
    [SerializeField, Min(0)] private int _grenadeRadius = 1;

    // Unit properties
    public int Health { get; private set; }
    public Weapon CurrentWeapon { get; private set; }
    public Team CurrentTeam { get; private set; }
    public Squad CurrentSquad { get; internal set; }
    public BattleMapTile CurrentTile => GetComponentInParent<BattleMapTile>();

    // Unit Supplies
    public int AmmoCount { get; private set; }
    public int GrenadeCount { get; private set; }
    public int HealthPackCount { get; private set; }

    // Action points
    public int ActionPoints { get; private set; }

    const int MAX_HEALTH = 5;
    const int MAX_AMMO_COUNT = 10;
    const int MAX_GRENADE_COUNT = 4;
    const int MAX_MEDKIT_COUNT = 4;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        this.AmmoCount = 4;
        this.GrenadeCount = 4;
        this.HealthPackCount = 4;

        this.SetHealth(MAX_HEALTH);
        // TODO: Set weapon
    }

    // ######## PUBLIC SETTERS ######## //
    public void SetAmmoCount(int newAmmoCount) { this.AmmoCount = newAmmoCount; }
    public void SetGrenadeCount(int newGrenadeCount) { this.GrenadeCount = newGrenadeCount; }
    public void SetHealthPackCount(int newHealthPackCount) { this.HealthPackCount = newHealthPackCount; }

    public void SetHealth(int newHealth) { this.Health = newHealth; }
    public void SetWeapon(Weapon newWeapon) { this.CurrentWeapon = newWeapon; }
    

    // TakeDamage(): Makes the unit take damage.
    // If the player's health reaches zero, the unit will die.
    //
    // damage: The amount of damage to take.
    public void TakeDamage(int damage){
        this.Health -= damage;
        if (this.Health < 0) this.Health = 0;
        if (this.Health <= 0) this.Die();
    }

    // Die(): Destroys the unit.
    public void Die(){
        if (CurrentSquad != null) CurrentSquad.RemoveUnit(this);
        if (BattleMapController.Instance != null && BattleMapController.Instance.SelectedUnit == this)
        {
            BattleMapController.Instance.ClearSelection();
        }
        BattleMapTile tile = CurrentTile;
        if (tile != null && tile.OccupyingUnit == this) tile.SetOccupyingUnit(null);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (CurrentSquad != null) CurrentSquad.RemoveUnit(this);
    }

    public void SetTeam(Team newTeam)
    {
        this.CurrentTeam = newTeam;
        
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = newTeam == Team.Player ? _playerSprite : _enemySprite;
        }
    }


    // ######## ACTIONS ######## // 

    private bool IsValidTarget(BattleMapTile targetTile)
    {
        BattleMapTile source = CurrentTile;
        if (Health <= 0 || source == null || source.OccupyingUnit != this || targetTile == null) return false;
        BattleMapController map = source.GetComponentInParent<BattleMapController>();
        return map != null && map.GetTile(source.Coordinate) == source &&
            map.GetTile(targetTile.Coordinate) == targetTile;
    }

    public bool MoveToTile(BattleMapTile targetTile)
    {
        if (!IsValidTarget(targetTile) || targetTile.IsOccupied ||
            targetTile.Terrain == null || !targetTile.Terrain.IsWalkable ||
            CurrentTile.GetManhattanDistance(targetTile) != 1) return false;

        targetTile.SetOccupyingUnit(this);
        return true;
    }

    public bool Shoot(BattleMapTile targetTile)
    {
        if (!IsValidTarget(targetTile) || AmmoCount <= 0 || targetTile.OccupyingUnit == this) return false;

        AmmoCount--;
        Unit target = targetTile.OccupyingUnit;
        if (target != null)
        {
            if (CurrentWeapon == null || Random.Range(0, 100) < CurrentWeapon.ChanceToHit)
            {
                target.TakeDamage(CurrentWeapon != null ? CurrentWeapon.Damage : _shotDamage);
            }
        }
        return true;
    }

    public bool ThrowGrenade(BattleMapTile targetTile)
    {
        if (!IsValidTarget(targetTile) || GrenadeCount <= 0) return false;

        BattleMapController map = CurrentTile.GetComponentInParent<BattleMapController>();
        GrenadeCount--;
        // Splash damage includes friendly units and the throwing unit.
        for (int y = -_grenadeRadius; y <= _grenadeRadius; y++)
        {
            for (int x = -_grenadeRadius; x <= _grenadeRadius; x++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > _grenadeRadius) continue;
                BattleMapTile tile = map.GetTile(targetTile.Coordinate + new Vector2Int(x, y));
                if (tile != null && tile.OccupyingUnit != null)
                {
                    tile.OccupyingUnit.TakeDamage(_grenadeDamage);
                }
            }
        }
        return true;
    }

    private void HealUnit(Unit targetUnit){
        // TODO: Implement
    }

    private void GiveSuppliesToUnit(Unit targetUnit){
        // TODO: Implement
    }

    private void TakeCover(){
        // TODO: Implement
    }


}
