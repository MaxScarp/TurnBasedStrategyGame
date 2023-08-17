using System;
using System.Collections.Generic;

public static class UnitManager
{
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    private static List<Unit> unitList = new List<Unit>();
    private static List<Unit> friendlyUnitList = new List<Unit>();
    private static List<Unit> enemyUnitList = new List<Unit>();

    public static void OnAnyUnitSpawnedInvoke(Unit unit)
    {
        unitList.Add(unit);

        if (unit.GetIsEnemy())
        {
            enemyUnitList.Add(unit);
        }
        else
        {
            friendlyUnitList.Add(unit);
        }

        OnAnyUnitSpawned?.Invoke(unit, EventArgs.Empty);
    }

    public static void OnAnyUnitDeadInvoke(Unit unit)
    {
        unitList.Remove(unit);

        if (unit.GetIsEnemy())
        {
            enemyUnitList.Remove(unit);
        }
        else
        {
            friendlyUnitList.Remove(unit);
        }

        OnAnyUnitDead?.Invoke(unit, EventArgs.Empty);
    }

    public static List<Unit> GetUnitList() => unitList;

    public static List<Unit> GetFirendlyUnitList() => friendlyUnitList;

    public static List<Unit> GetEnemyUnitList() => enemyUnitList;
}
