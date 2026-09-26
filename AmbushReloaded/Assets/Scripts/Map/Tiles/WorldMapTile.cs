using UnityEngine;

public class WorldMapTile : GameTile
{
    public Unit OccupyingSquad { get; private set; }
    public Supplies Supplies { get; private set; }

    public void SetOccupyingUnit(Unit newOccupyingUnit)
    {
        OccupyingSquad = newOccupyingUnit;
    }

    public void SetSupplies(Supplies newSupplies)
    {
        Supplies = newSupplies;
    }
}