using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float spinAmountDeg;
    private float totalSpinAmountDeg;
    private float spinRotationOffsetDeg;

    protected override void Awake()
    {
        base.Awake();

        totalSpinAmountDeg = 360f;
    }

    private void Update()
    {
        if (isActive)
        {
            if (!isActive)
            {
                return;
            }

            float rotationSpeed = 200f;
            spinAmountDeg += rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(spinAmountDeg, Vector3.up);

            if (spinAmountDeg >= totalSpinAmountDeg + spinRotationOffsetDeg)
            {
                ActionComplete();
            }
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);
        spinRotationOffsetDeg = transform.rotation.eulerAngles.y >= 0f ? transform.rotation.eulerAngles.y : 360f + transform.rotation.eulerAngles.y;
        spinAmountDeg = transform.rotation.eulerAngles.y;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return new List<GridPosition>() { unitGridPosition };
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override int GetActionPointsCost()
    {
        return 2;
    }
}