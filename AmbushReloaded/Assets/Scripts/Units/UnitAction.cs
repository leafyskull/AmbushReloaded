// UnitAction.cs: Contains the enum for UnitAction.
//
// Actions that a unit can take:
// - Move (to a tile)
// - Shoot (at a tile)
// - Throw grenade (at a tile)
// - Heal (this or other unit)
// - Give supplies (other unit)
// - Take cover (reduces chance to be hit and incoming damage if hit)

public enum UnitAction{
    MOVE,
    SHOOT,
    THROW_GRENADE,
    HEAL,
    GIVE_SUPPLIES,
    TAKE_COVER
}
