using System;
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
                isActive = false;
                onActionComplete();
            }
        }
    }

    public void Spin(Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        spinRotationOffsetDeg = transform.rotation.eulerAngles.y >= 0f ? transform.rotation.eulerAngles.y : 360f + transform.rotation.eulerAngles.y;
        spinAmountDeg = transform.rotation.eulerAngles.y;
        isActive = true;
    }
}
