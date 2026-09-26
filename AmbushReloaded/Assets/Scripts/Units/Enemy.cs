using UnityEngine;

public class Enemy : Unit
{

    // Supplies
    public int AmmoCount { get; private set; }
    public int GrenadeCount { get; private set; }
    public int HealthPackCount { get; private set; }
    
    // Action points
    public int ActionPoints { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.AmmoCount = 4;
        this.GrenadeCount = 4;
        this.HealthPackCount = 4;
    }

    // ######## SETTERS ######## //
    private void SetAmmoCount(int newAmmoCount) { this.AmmoCount = newAmmoCount; }
    private void SetGrenadeCount(int newGrenadeCount) { this.GrenadeCount = newGrenadeCount; }
    

}
