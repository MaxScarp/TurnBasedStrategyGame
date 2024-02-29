using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType GridVisualType;
        public Material Material;
    }

    public enum GridVisualType
    {
        WHITE,
        BLUE,
        RED,
        YELLOW,
        RED_SOFT,
    }

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Start()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[GameManager.LevelGrid.GetWidth(), GameManager.LevelGrid.GetHeight()];

        for (int x = 0; x < GameManager.LevelGrid.GetWidth(); x++)
        {
            for (int z = 0; z < GameManager.LevelGrid.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, GameManager.LevelGrid.GetWorldPosition(gridPosition), Quaternion.identity, transform);
                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        GameManager.UnitActionSystem.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        GameManager.LevelGrid.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    public void HideAllGridPosition()
    {
        foreach (GridSystemVisualSingle gridSystemVisualSingle in gridSystemVisualSingleArray)
        {
            gridSystemVisualSingle.Hide();
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.X, gridPosition.Z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();

        Unit selectedUnit = GameManager.UnitActionSystem.SelectedUnit;
        BaseAction baseAction = GameManager.UnitActionSystem.SelectedAction;

        GridVisualType gridVisualType;
        switch (baseAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.WHITE;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.BLUE;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.RED;

                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.MaxShootDistance, GridVisualType.RED_SOFT);
                break;
        }

        ShowGridPositionList(baseAction.GetValidActionGridPositionList(), gridVisualType);
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.GridVisualType != gridVisualType) continue;

            return gridVisualTypeMaterial.Material;
        }

        Debug.LogError($"Error: Trying to find {typeof(GridVisualTypeMaterial)} for {gridVisualType}");
        return null;
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);
                if (!GameManager.LevelGrid.IsValidGridPosition(testGridPosition)) continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range) continue;

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void OnDestroy()
    {
        GameManager.UnitActionSystem.OnSelectedActionChanged -= UnitActionSystem_OnSelectedActionChanged;
        GameManager.LevelGrid.OnAnyUnitMovedGridPosition -= LevelGrid_OnAnyUnitMovedGridPosition;
    }
}
