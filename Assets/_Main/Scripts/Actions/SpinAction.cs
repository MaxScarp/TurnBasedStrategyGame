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

            float rotateSpeed = 100f;
            spinAmountDeg += rotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(spinAmountDeg, Vector3.up);

            if (spinAmountDeg >= totalSpinAmountDeg + spinRotationOffsetDeg)
            {
                isActive = false;
            }
        }
    }

    public void Spin()
    {
        spinRotationOffsetDeg = transform.rotation.eulerAngles.y >= 0f ? transform.rotation.eulerAngles.y : 360f + transform.rotation.eulerAngles.y;
        spinAmountDeg = transform.rotation.eulerAngles.y;
        isActive = true;
    }
}
