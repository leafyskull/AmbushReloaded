using UnityEngine;


public enum SuppliesType
{
    AMMO,
    GRENADES,
    HEALTH,
}

public class Supplies : MonoBehaviour
{
    public SpriteRenderer _spriteRenderer;
    public SuppliesType _suppliesType { get; private set; }
    public int _suppliesAmount {get; private set; }

    

}
