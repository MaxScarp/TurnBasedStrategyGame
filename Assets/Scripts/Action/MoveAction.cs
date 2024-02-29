using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    private const string MOVE_ACTION_NAME = "Move";

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [SerializeField] private int maxMoveDistance = 4;

    private Vector3 targetPositon;

    protected override void Awake()
    {
        base.Awake();
        targetPositon = transform.position;
    }

    private void Update()
    {
        if (!isActive) return;

        Vector3 moveDirection = (targetPositon - transform.position).normalized;

        float stoppingDistance = 0.1f;
        if (Vector3.Distance(transform.position, targetPositon) >= stoppingDistance)
        {
            float moveSpeed = 4.0f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            OnStartMoving?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnStopMoving?.Invoke(this, EventArgs.Empty);
            ActionComplete();
        }

        float rotateSpeed = 10.0f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!GameManager.LevelGrid.IsValidGridPosition(testGridPosition)) continue;
                if (unitGridPosition == testGridPosition) continue;
                if (GameManager.LevelGrid.HasAnyUnitOnGridPosition(testGridPosition)) continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName() => MOVE_ACTION_NAME;

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetPositon = GameManager.LevelGrid.GetWorldPosition(gridPosition);

        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        if (!unit.TryGetAction(out ShootAction shootAction))
        {
            Debug.LogError($"Error: trying to get the component of type {typeof(ShootAction)}! - {this.name}.");
            return null;
        }

        int targetCountAtGridPosition = shootAction.GetTargetCountAtPosition(gridPosition);

        int multiplier = 10;
        return new EnemyAIAction { GridPosition = gridPosition, ActionValue = targetCountAtGridPosition * multiplier };
    }
}
