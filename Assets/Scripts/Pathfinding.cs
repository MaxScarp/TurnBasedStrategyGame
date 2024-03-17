using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstacleLayerMask;

    private int width;
    private int height;
    private float cellSize;

    private GridSystem<PathNode> gridSystem;

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(this.width, this.height, this.cellSize, (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        //gridSystem.CreateDebugObject(gridDebugObjectPrefab, transform);

        for (int x = 0; x < this.width; x++)
        {
            for (int z = 0; z < this.height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = GameManager.LevelGrid.GetWorldPosition(gridPosition);
                float raycastOffsetDistance = 5.0f;
                if (Physics.Raycast(worldPosition + Vector3.down * raycastOffsetDistance, Vector3.up, raycastOffsetDistance * 2.0f, obstacleLayerMask))
                {
                    GetNode(x, z).IsWalkable = false;
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);

        openList.Add(startNode);

        for (int x = 0; x < gridSystem.Width; x++)
        {
            for (int z = 0; z < gridSystem.Height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.GCost = int.MaxValue;
                pathNode.HCost = 0;
                pathNode.CameFromPathNode = null;
            }
        }

        startNode.GCost = 0;
        startNode.HCost = CalculateDistance(startGridPosition, endGridPosition);

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);
            if (currentNode == endNode)
            {
                pathLength = endNode.FCost;
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.IsWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GCost + CalculateDistance(currentNode.GridPosition, neighbourNode.GridPosition);
                if (tentativeGCost < neighbourNode.GCost)
                {
                    neighbourNode.CameFromPathNode = currentNode;
                    neighbourNode.GCost = tentativeGCost;
                    neighbourNode.HCost = CalculateDistance(neighbourNode.GridPosition, endGridPosition);

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        pathLength = 0;
        return null;
    }

    private int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;

        int xDistance = Mathf.Abs(gridPositionDistance.X);
        int zDistance = Mathf.Abs(gridPositionDistance.Z);
        int remaining = Mathf.Abs(xDistance - zDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        foreach (PathNode pathNode in pathNodeList)
        {
            if (pathNode.FCost < lowestFCostPathNode.FCost)
            {
                lowestFCostPathNode = pathNode;
            }
        }

        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z) => gridSystem.GetGridObject(new GridPosition(x, z));

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GridPosition;

        //Left
        if (gridPosition.X - 1 >= 0)
        {
            neighbourList.Add(GetNode(gridPosition.X - 1, gridPosition.Z));
            if (gridPosition.Z - 1 >= 0)
            {
                neighbourList.Add(GetNode(gridPosition.X - 1, gridPosition.Z - 1));
            }
            if (gridPosition.Z + 1 < gridSystem.Height)
            {
                neighbourList.Add(GetNode(gridPosition.X - 1, gridPosition.Z + 1));
            }
        }
        //Right
        if (gridPosition.X + 1 < gridSystem.Width)
        {
            neighbourList.Add(GetNode(gridPosition.X + 1, gridPosition.Z));
            if (gridPosition.Z - 1 >= 0)
            {
                neighbourList.Add(GetNode(gridPosition.X + 1, gridPosition.Z - 1));
            }
            if (gridPosition.Z + 1 < gridSystem.Height)
            {
                neighbourList.Add(GetNode(gridPosition.X + 1, gridPosition.Z + 1));
            }
        }
        //Down
        if (gridPosition.Z - 1 >= 0)
        {
            neighbourList.Add(GetNode(gridPosition.X, gridPosition.Z - 1));
        }
        //Up
        if (gridPosition.Z + 1 < gridSystem.Height)
        {
            neighbourList.Add(GetNode(gridPosition.X, gridPosition.Z + 1));
        }

        return neighbourList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        PathNode currentNode = endNode;

        pathNodeList.Add(endNode);
        while (currentNode.CameFromPathNode != null)
        {
            pathNodeList.Add(currentNode.CameFromPathNode);
            currentNode = currentNode.CameFromPathNode;
        }
        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GridPosition);
        }

        return gridPositionList;
    }

    public int GetMoveStraightCost() => MOVE_STRAIGHT_COST;

    public bool IsWalkable(GridPosition gridPosition) => gridSystem.GetGridObject(gridPosition).IsWalkable;

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition) => FindPath(startGridPosition, endGridPosition, out int pathLength) != null;

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
