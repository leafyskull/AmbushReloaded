using UnityEngine;

public class BattleMapTile : GameTile
{
    public Unit OccupyingUnit { get; private set; }
    public BattleMapTerrain Terrain { get; private set; }

    public void SetOccupyingUnit(Unit newOccupyingUnit)
    {
        OccupyingUnit = newOccupyingUnit;
    }

    public void SetTerrain(BattleMapTerrain newTerrain)
    {
        Terrain = newTerrain;
    }
}