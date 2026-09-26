// Weapon.cs: Contains the class definition for a weapon.
//
// WEAPONS THAT WILL BE IN THE MVP:
// - Rifle
// - LMG
// - Grenade launcher
//
// WEAPONS WILL HAVE:
// - Chance to hit
// - Damage

using UnityEngine;


public abstract class Weapon{

    [SerializeField] private int _damage;
    public int Damage
    {
        get => _damage;
        private set => _damage = value;
    }

    [SerializeField] private int _chanceToHit;
    public int ChanceToHit
    {
        get => _chanceToHit;
        private set => _chanceToHit = value;
    }
}