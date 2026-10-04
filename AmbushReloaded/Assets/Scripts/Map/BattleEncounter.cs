using System.Collections.Generic;
using UnityEngine;

// Only values cross the scene boundary; scene objects are deliberately not retained.
public sealed class BattleEncounter
{
    public Vector2Int PlayerCoordinate { get; }
    public UnitAction Trigger { get; }
    public IReadOnlyList<EnemyGroup> Enemies { get; }

    public readonly struct EnemyGroup
    {
        public Vector2Int Coordinate { get; }
        public int OpeningDamage { get; }

        public EnemyGroup(Vector2Int coordinate, int openingDamage)
        {
            Coordinate = coordinate;
            OpeningDamage = Mathf.Max(0, openingDamage);
        }
    }

    public BattleEncounter(Vector2Int playerCoordinate, UnitAction trigger, List<EnemyGroup> enemies)
    {
        PlayerCoordinate = playerCoordinate;
        Trigger = trigger;
        Enemies = new List<EnemyGroup>(enemies).AsReadOnly();
    }
}

public static class BattleEncounterTransfer
{
    public static BattleEncounter Pending { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset() => Pending = null;

    public static void Set(BattleEncounter encounter) => Pending = encounter;

    public static BattleEncounter Consume()
    {
        BattleEncounter encounter = Pending;
        Pending = null;
        return encounter;
    }
}
