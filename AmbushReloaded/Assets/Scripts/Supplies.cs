using UnityEngine;


public enum SuppliesType
{
    AMMO,
    GRENADES,
    HEALTH,
}

public class Supplies : MonoBehaviour
{
    public SuppliesType SuppliesType { get; private set; }
    public int SuppliesAmount {get; private set; }



    void Start()
    {
        this.SuppliesType = SuppliesType.AMMO;
        this.SuppliesAmount = 2;
    }

     

}
