using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    private const string MOVE_ACTION_NAME = "Move";

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [SerializeField] private int maxMoveDistance = 4;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    private void Update()
    {
        if (!isActive) return;

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float rotateSpeed = 10.0f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        float stoppingDistance = 0.1f;
        if (Vector3.Distance(transform.position, targetPosition) >= stoppingDistance)
        {
            float moveSpeed = 4.0f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
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
                if (!GameManager.Pathfinding.IsWalkable(testGridPosition)) continue;
                if (!GameManager.Pathfinding.HasPath(unitGridPosition, testGridPosition)) continue;
                if (GameManager.Pathfinding.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * GameManager.Pathfinding.GetMoveStraightCost()) continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName() => MOVE_ACTION_NAME;

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        currentPositionIndex = 0;
        List<GridPosition> pathGridPositionList = GameManager.Pathfinding.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(GameManager.LevelGrid.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);

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
