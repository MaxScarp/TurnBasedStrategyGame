using System;
using UnityEngine;

public class GridSystem<TGridObject>
{
    private float cellSize;
    private TGridObject[,] gridObjectArray;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        Width = width;
        Height = height;
        this.cellSize = cellSize;

        gridObjectArray = new TGridObject[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjectArray[x, z] = createGridObject(this, gridPosition);
            }
        }
    }

    public void CreateDebugObject(Transform debugPrefab, Transform parent)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity, parent);
                if (!debugTransform)
                {
                    Debug.LogError($"Error: Trying to instantiate {typeof(Transform)}! - {this}.");
                    return;
                }
                if (!debugTransform.TryGetComponent(out GridDebugObject gridDebugObject))
                {
                    Debug.LogError($"Error: Trying to get {typeof(GridDebugObject)}! - {this}.");
                    return;
                }

                gridDebugObject.SetGridObject(GetGridObject(gridPosition) as GridObject);
            }
        }
    }

    public bool IsValidGridPosition(GridPosition gridPosition) => gridPosition.X >= 0 && gridPosition.Z >= 0 && gridPosition.X < Width && gridPosition.Z < Height;

    public Vector3 GetWorldPosition(GridPosition gridPosition) => new Vector3(gridPosition.X, 0.0f, gridPosition.Z) * cellSize;

    public GridPosition GetGridPosition(Vector3 worldPosition) => new GridPosition(Mathf.RoundToInt(worldPosition.x / cellSize), Mathf.RoundToInt(worldPosition.z / cellSize));

    public TGridObject GetGridObject(GridPosition gridPosition) => gridObjectArray[gridPosition.X, gridPosition.Z];
}
