using UnityEngine;
using System;

public class BattleMapTerrain
{
    public BattleMapTerrainType TerrainType { get; private set; }

    // #### Terrain properties #### //
    public bool IsWalkable { get; private set; }
    public bool BlocksLineOfSight { get; private set; }
    public bool ProvidesCover { get; private set; }
    public int MovementCost { get; private set; }


    // #### PUBLIC SETTERS #### //

    // SetTerrain(): Sets a BattleMapTerrain's TerrainType,
    // and updates it's properties accordingly.
    public void SetTerrain(BattleMapTerrainType newTerrainType)
    {
        TerrainType = newTerrainType;

        // Set properties associated with the terrain.
        switch (newTerrainType)
        {
            case BattleMapTerrainType.OPEN:
                IsWalkable = true;
                BlocksLineOfSight = false;
                ProvidesCover = false;
                MovementCost = 1;
                break;

            case BattleMapTerrainType.TREE:
                IsWalkable = false;
                BlocksLineOfSight = true;
                ProvidesCover = true;
                MovementCost = 0;
                break;

            default:
                Debug.LogWarning($"Terrain type {newTerrainType} has no defined properties.");
                break;
        }
    }


    // SetRandomTerrain(): Sets a random TerrainType
    public void SetRandomTerrain()
    {
        Array terrainTypes = Enum.GetValues(typeof(BattleMapTerrainType));

        int randomIndex = UnityEngine.Random.Range(0, terrainTypes.Length);

        BattleMapTerrainType selectedTerrainType =
            (BattleMapTerrainType)terrainTypes.GetValue(randomIndex);

        SetTerrain(selectedTerrainType);
    }
}