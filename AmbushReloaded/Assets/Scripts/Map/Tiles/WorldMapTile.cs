using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapTile : GameTile
{
    public Squad OccupyingSquad { get; private set; }
    public Supplies Supplies { get; private set; }
    public bool IsOccupied => OccupyingSquad != null;
    private readonly List<Squad> ambushSquads = new List<Squad>();
    public IReadOnlyList<Squad> AmbushSquads => ambushSquads.AsReadOnly();
    public bool IsAmbush => ambushSquads.Exists(squad => squad != null && squad.CurrentTeam == Team.Enemy);

    public void ClearAmbush() => ambushSquads.Clear();

    public void AddAmbushSquad(Squad squad)
    {
        if (squad != null && squad.CurrentTeam == Team.Enemy && !ambushSquads.Contains(squad))
            ambushSquads.Add(squad);
    }


    public void SetOccupyingSquad(Squad squad)
    {
        if (OccupyingSquad == squad) return;
        if (squad != null && IsOccupied)
        {
            throw new InvalidOperationException("The destination tile already has a squad.");
        }

        // If squad is null, clear the tile's occupying squad.
        if (squad == null)
        {
            if (OccupyingSquad != null)
            {
                OccupyingSquad.transform.SetParent(transform.parent, true);
            }
            OccupyingSquad = null;
            return;
        }

        // Remove squad from previous tile.
        WorldMapTile previousTile = squad.GetComponentInParent<WorldMapTile>();
        if (previousTile != null && previousTile != this && previousTile.OccupyingSquad == squad)
        {
            previousTile.SetOccupyingSquad(null);
        }

        // Set squad in this tile.
        OccupyingSquad = squad;
        squad.transform.SetParent(transform, false);
        squad.transform.localPosition = Vector3.zero;
    }

    // SetSupplies(): Sets supplies on the tile.
    //
    // newSupplies: The supplies to assign to the tile.
    public void SetSupplies(Supplies newSupplies)
    {
        Supplies = newSupplies;
    }
}
