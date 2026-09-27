// Unit.cs: Contains the class definition for a Unit.
//
// A unit represents a single 'soldier', and will appear on the BattleMap.

using UnityEngine;

public class Unit : MonoBehaviour
{

    // Unit properties
    public int Health { get; private set; }
    public Weapon CurrentWeapon { get; private set; }

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
    void Start()
    {
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
        Destroy(gameObject);
    }


    // ######## ACTIONS ######## // 

    private void MoveToTile(GameTile targetTile){
        // TODO: Implement
    }

    private void Shoot(GameTile targetTile){
        // TODO: Implement
    }

    private void ThrowGrenade(GameTile targetTile){
        // TODO: Implement
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
