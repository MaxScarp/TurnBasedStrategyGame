using System;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    private const string NAME = "Sword";

    public static event EventHandler OnAnySwordHit;

    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;

    private enum State
    {
        SWINGING_SWORD_BEFORE_HIT,
        SWING_SWORD_AFTER_HIT
    }

    public int MaxSwordDistance { get; private set; }

    private State state;
    private float stateTimer;
    private Unit targetUnit;
    private int damage;
    private float rotateSpeed;

    protected override void Awake()
    {
        base.Awake();

        MaxSwordDistance = 1;
        damage = 100;
        rotateSpeed = 10.0f;
    }

    private void Update()
    {
        if (!isActive) return;

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.SWINGING_SWORD_BEFORE_HIT:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                break;
            case State.SWING_SWORD_AFTER_HIT:
                break;
        }

        if (stateTimer <= 0.0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.SWINGING_SWORD_BEFORE_HIT:
                state = State.SWING_SWORD_AFTER_HIT;
                float afterHitStateTime = 0.5f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(damage);
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SWING_SWORD_AFTER_HIT:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -MaxSwordDistance; x <= MaxSwordDistance; x++)
        {
            for (int z = -MaxSwordDistance; z <= MaxSwordDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!GameManager.LevelGrid.IsValidGridPosition(testGridPosition)) continue;

                if (!GameManager.LevelGrid.HasAnyUnitOnGridPosition(testGridPosition)) continue;

                Unit targetUnit = GameManager.LevelGrid.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.IsEnemy == unit.IsEnemy) continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = GameManager.LevelGrid.GetUnitAtGridPosition(gridPosition);

        state = State.SWINGING_SWORD_BEFORE_HIT;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;

        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public override string GetActionName() => NAME;

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) => new EnemyAIAction { GridPosition = gridPosition, ActionValue = 200 };
}
