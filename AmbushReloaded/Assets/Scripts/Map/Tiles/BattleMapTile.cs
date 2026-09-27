public class BattleMapTile : GameTile
{
    public Unit OccupyingUnit { get; private set; }
    public BattleMapTerrain Terrain { get; private set; }

    public bool IsOccupied => OccupyingUnit != null;

    // Pass null to clear the tile when a unit leaves.
    public void SetOccupyingUnit(Unit newOccupyingUnit)
    {
        OccupyingUnit = newOccupyingUnit;
    }

    public void SetTerrain(BattleMapTerrain newTerrain)
    {
        Terrain = newTerrain;
    }
}
