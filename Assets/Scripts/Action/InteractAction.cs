using System;
using System.Collections.Generic;

public class InteractAction : BaseAction
{
    private const string NAME = "Interact";

    public int MaxInteractDistance { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        MaxInteractDistance = 1;
    }

    private void OnInteractionComplete()
    {
        ActionComplete();
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -MaxInteractDistance; x <= MaxInteractDistance; x++)
        {
            for (int z = -MaxInteractDistance; z <= MaxInteractDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!GameManager.LevelGrid.IsValidGridPosition(testGridPosition)) continue;

                if (GameManager.LevelGrid.GetInteractableAtGridPosition(testGridPosition) == null) continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        GameManager.LevelGrid.GetInteractableAtGridPosition(gridPosition).Interact(OnInteractionComplete);

        ActionStart(onActionComplete);
    }

    public override string GetActionName() => NAME;

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) => new EnemyAIAction { GridPosition = gridPosition, ActionValue = 0 };
}
