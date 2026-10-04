using System;
using UnityEngine;

public class BattleMapTile : GameTile
{
    public Unit OccupyingUnit { get; private set; }
    public BattleMapTerrain Terrain { get; private set; }

    public bool IsOccupied => OccupyingUnit != null;

    // Pass null to clear the tile when a unit leaves.
    public void SetOccupyingUnit(Unit newOccupyingUnit)
    {
        if (OccupyingUnit == newOccupyingUnit) return;
        if (newOccupyingUnit != null && IsOccupied)
        {
            throw new InvalidOperationException("The destination tile already has a unit.");
        }

        if (newOccupyingUnit == null)
        {
            if (OccupyingUnit != null)
            {
                OccupyingUnit.transform.SetParent(transform.parent, true);
            }
            OccupyingUnit = null;
            return;
        }

        BattleMapTile previousTile = newOccupyingUnit.CurrentTile;
        if (previousTile != null && previousTile != this && previousTile.OccupyingUnit == newOccupyingUnit)
        {
            previousTile.SetOccupyingUnit(null);
        }

        OccupyingUnit = newOccupyingUnit;
        newOccupyingUnit.transform.SetParent(transform, false);
        newOccupyingUnit.transform.localPosition = Vector3.zero;
    }

    public void SetTerrain(BattleMapTerrain newTerrain)
    {
        Terrain = newTerrain;
    }
}
