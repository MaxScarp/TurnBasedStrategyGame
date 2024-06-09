using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public event EventHandler OnAnyUnitMovedGridPosition;

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 2.0f;

    private GridSystem<GridObject> gridSystem;

    private void Awake()
    {
        gridSystem = new GridSystem<GridObject>(width, height, cellSize, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
        //gridSystem.CreateDebugObject(gridDebugObjectPrefab, transform);
    }

    private void Start()
    {
        GameManager.Pathfinding.Setup(width, height, cellSize);
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        gridSystem.GetGridObject(gridPosition).AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) => gridSystem.GetGridObject(gridPosition).GetUnitList();

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        gridSystem.GetGridObject(gridPosition).RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);

        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition) => gridSystem.GetGridObject(gridPosition).HasAnyUnit();
    public Unit GetUnitAtGridPosition(GridPosition gridPosition) => gridSystem.GetGridObject(gridPosition).GetUnit();
    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition) => gridSystem.GetGridObject(gridPosition).GetInteractable();
    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
    {
        gridSystem.GetGridObject(gridPosition).SetInteractable(interactable);
    }

    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);

    public int GetWidth() => gridSystem.Width;
    public int GetHeight() => gridSystem.Height;
}
