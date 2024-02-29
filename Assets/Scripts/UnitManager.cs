using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<Unit> UnitList { get; private set; }
    public List<Unit> FriendlyUnitList { get; private set; }
    public List<Unit> EnemyUnitList { get; private set; }
    public List<Unit> PendingUnitToBeRemoved { get; private set; }

    private void Awake()
    {
        UnitList = new List<Unit>();
        FriendlyUnitList = new List<Unit>();
        EnemyUnitList = new List<Unit>();
        PendingUnitToBeRemoved = new List<Unit>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        GameManager.TurnSystem.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        if (GameManager.TurnSystem.IsPlayerTurn)
        {
            if (PendingUnitToBeRemoved.Count <= 0) return;

            PendingUnitToBeRemoved.Clear();

            if (FriendlyUnitList.Count <= 0) return;

            GameManager.UnitActionSystem.SelectedUnit = FriendlyUnitList[Random.Range(0, FriendlyUnitList.Count)];
        }
    }

    private void Unit_OnAnyUnitDead(object sender, System.EventArgs e)
    {
        Unit unit = sender as Unit;
        if (unit.IsEnemy)
        {
            EnemyUnitList.Remove(unit);
        }
        else
        {
            FriendlyUnitList.Remove(unit);
        }

        UnitList.Remove(unit);

        if (unit == GameManager.UnitActionSystem.SelectedUnit)
        {
            PendingUnitToBeRemoved.Add(unit);
        }
    }

    private void Unit_OnAnyUnitSpawned(object sender, System.EventArgs e)
    {
        Unit unit = sender as Unit;
        if (unit.IsEnemy)
        {
            EnemyUnitList.Add(unit);
        }
        else
        {
            FriendlyUnitList.Add(unit);
        }

        UnitList.Add(unit);
    }

    private void OnDestroy()
    {
        Unit.OnAnyUnitSpawned -= Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead -= Unit_OnAnyUnitDead;
    }
}
