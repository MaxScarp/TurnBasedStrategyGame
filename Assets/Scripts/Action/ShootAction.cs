using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    private const string SHOOT_ACTION_NAME = "Shoot";

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Unit TargetUnit;
        public Unit ShootingUnit;
    }

    [SerializeField] private int maxShootDistance = 7;
    [SerializeField] private LayerMask obstacleLayerMask;

    private enum State
    {
        AIMING,
        SHOOTING,
        COOL_OFF,
    }

    private State state;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;

    public Unit TargetUnit { get => targetUnit; }
    public int MaxShootDistance { get => maxShootDistance; }

    private void Update()
    {
        if (!isActive) return;

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.AIMING:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotationSpeed = 10.0f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotationSpeed);
                break;
            case State.SHOOTING:
                if (canShootBullet)
                {
                    canShootBullet = false;
                    Shoot();
                }
                break;
            case State.COOL_OFF:
                break;
        }

        if (stateTimer <= 0.0f)
        {
            NextState();
        }
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs { TargetUnit = targetUnit, ShootingUnit = unit });
        OnShoot?.Invoke(this, new OnShootEventArgs { TargetUnit = targetUnit, ShootingUnit = unit });

        int damageAmount = 40;
        targetUnit.Damage(damageAmount);
    }

    private void NextState()
    {
        switch (state)
        {
            case State.AIMING:
                state = State.SHOOTING;
                float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
                break;
            case State.SHOOTING:
                state = State.COOL_OFF;
                float coolOffStateTime = 0.5f;
                stateTimer = coolOffStateTime;
                break;
            case State.COOL_OFF:
                ActionComplete();
                break;
        }
    }

    public override string GetActionName() => SHOOT_ACTION_NAME;

    public override List<GridPosition> GetValidActionGridPositionList() => GetValidActionGridPositionList(unit.GetGridPosition());

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!GameManager.LevelGrid.IsValidGridPosition(testGridPosition)) continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance) continue;

                if (!GameManager.LevelGrid.HasAnyUnitOnGridPosition(testGridPosition)) continue;

                Unit targetUnit = GameManager.LevelGrid.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.IsEnemy == unit.IsEnemy) continue;

                Vector3 unitWorldPosition = GameManager.LevelGrid.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight, shootDir, Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()), obstacleLayerMask)) continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        state = State.AIMING;
        float aimingStateTimer = 1f;
        stateTimer = aimingStateTimer;

        targetUnit = GameManager.LevelGrid.GetUnitAtGridPosition(gridPosition);

        canShootBullet = true;

        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = GameManager.LevelGrid.GetUnitAtGridPosition(gridPosition);

        int multiplier = 100;
        return new EnemyAIAction { GridPosition = gridPosition, ActionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * multiplier) };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition) => GetValidActionGridPositionList(gridPosition).Count;
}
