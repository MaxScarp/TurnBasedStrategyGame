using System;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    private void Start()
    {
        DestructibleCrate.OnAnyDestroied += DestructibleCrate_OnAnyDestroied;
    }

    private void DestructibleCrate_OnAnyDestroied(object sender, EventArgs e)
    {
        DestructibleCrate destructibleCrate = sender as DestructibleCrate;
        GameManager.Pathfinding.SetIsWalkable(destructibleCrate.GetGridPosition(), true);
    }
}
