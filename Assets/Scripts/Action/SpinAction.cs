using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private const string SPIN_ACTION_NAME = "Spin";
    private const float ROTATION_ANGLE = 360.0f;

    private float totalSpinAmount;

    protected override void Awake()
    {
        base.Awake();
        actionPointsCost = 2;
    }

    private void Update()
    {
        if (!isActive) return;

        float spinAddAmountDegrees = ROTATION_ANGLE * Time.deltaTime;
        transform.Rotate(new Vector3(0.0f, spinAddAmountDegrees, 0.0f));

        totalSpinAmount += spinAddAmountDegrees;
        if (totalSpinAmount >= ROTATION_ANGLE)
        {
            ActionComplete();
        }
    }

    public override string GetActionName() => SPIN_ACTION_NAME;

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        totalSpinAmount = 0.0f;

        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList() => new List<GridPosition> { unit.GetGridPosition() };

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) => new EnemyAIAction { GridPosition = gridPosition, ActionValue = 0 };
}
