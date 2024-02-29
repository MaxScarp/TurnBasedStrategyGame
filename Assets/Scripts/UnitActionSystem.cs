using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField] private LayerMask unitLayerMask;

    private Unit selectedUnit;
    private BaseAction selectedAction;

    private bool isBusy;

    public Unit SelectedUnit
    {
        get => selectedUnit;
        set
        {
            selectedUnit = value;

            if (!value.TryGetAction(out MoveAction moveAction))
            {
                Debug.LogError($"Error: trying to get the component of type {typeof(MoveAction)}! - {this.name}.");
                return;
            }
            SelectedAction = moveAction;

            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public BaseAction SelectedAction
    {
        get => selectedAction;
        set
        {
            selectedAction = value;

            OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Update()
    {
        if (isBusy) return;
        if (!GameManager.TurnSystem.IsPlayerTurn) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (TryHandleUnitSelection()) return;

        HandleSelectedAction();
    }

    private void SetBusy()
    {
        isBusy = true;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private bool TryHandleUnitSelection()
    {
        if (!Input.GetMouseButtonDown(0)) return false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask)) return false;
        if (!raycastHit.transform.TryGetComponent(out Unit unit)) return false;
        if (unit == SelectedUnit) return false;
        if (unit.IsEnemy) return false;

        SelectedUnit = unit;

        return true;
    }

    private void HandleSelectedAction()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        GridPosition mouseGridPosition = GameManager.LevelGrid.GetGridPosition(GameManager.MousePosition);

        if (!SelectedAction.IsValidActionGridPosition(mouseGridPosition)) return;
        if (!SelectedUnit.TrySpendActionPointsToTakeAction(SelectedAction)) return;

        SetBusy();
        SelectedAction.TakeAction(mouseGridPosition, ClearBusy);

        OnActionStarted?.Invoke(this, EventArgs.Empty);
    }
}
