using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject;

    private void Start()
    {
        ActionsEventDispatcher.OnAnyActionStarted += ActionsEventDispatcher_OnAnyActionStarted;
        ActionsEventDispatcher.OnAnyActionCompleted += ActionsEventDispatcher_OnAnyActionCompleted;
    }

    private void ActionsEventDispatcher_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
        }
    }

    private void ActionsEventDispatcher_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();

                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;
                Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;
                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.AngleAxis(90, Vector3.up) * shootDir * shoulderOffsetAmount;

                Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + (shootDir * -1);

                actionCameraGameObject.transform.position = actionCameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);

                ShowActionCamera();
                break;
        }
    }

    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        ActionsEventDispatcher.OnAnyActionStarted -= ActionsEventDispatcher_OnAnyActionStarted;
        ActionsEventDispatcher.OnAnyActionCompleted -= ActionsEventDispatcher_OnAnyActionCompleted;
    }
}
