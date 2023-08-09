using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    public event EventHandler OnShoot;

    private enum State
    {
        AIMING,
        SHOOTING,
        COOLOFF,
    }

    private State state;
    private int maxShootDistance;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;

    protected override void Awake()
    {
        base.Awake();

        maxShootDistance = 7;
    }

    private void Update()
    {
        if (isActive)
        {
            if (!isActive)
            {
                return;
            }

            stateTimer -= Time.deltaTime;
            switch (state)
            {
                case State.AIMING:
                    Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                    float rotateSpeed = 10.0f;
                    transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                    break;
                case State.SHOOTING:
                    if (canShootBullet)
                    {
                        Shoot();
                        canShootBullet = false;
                    }
                    break;
                case State.COOLOFF:
                    break;
            }

            if (stateTimer <= 0f)
            {
                NextState();
            }
        }
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
                state = State.COOLOFF;
                float coolOffStateTime = 0.5f;
                stateTimer = coolOffStateTime;
                break;
            case State.COOLOFF:
                ActionComplete();
                break;
        }
    }

    private void Shoot()
    {
        OnShoot?.Invoke(this, EventArgs.Empty);

        targetUnit.Damage();
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Math.Abs(z);
                if (testDistance > maxShootDistance)
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    //GridPosition is empty, no Unit
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.GetIsEnemy() == unit.GetIsEnemy())
                {
                    //Both Units on same 'team'
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.AIMING;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShootBullet = true;
    }
}
