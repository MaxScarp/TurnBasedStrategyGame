using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GridPosition mouseGridPosition = GameManager.LevelGrid.GetGridPosition(GameManager.MousePosition);
            GridPosition startGridPoisiton = new GridPosition(0, 0);

            List<GridPosition> gridPositionList = GameManager.Pathfinding.FindPath(startGridPoisiton, mouseGridPosition, out int pathLength);
            for (int i = 0; i < gridPositionList.Count - 1; i++)
            {
                Debug.DrawLine(GameManager.LevelGrid.GetWorldPosition(gridPositionList[i]), GameManager.LevelGrid.GetWorldPosition(gridPositionList[i + 1]), Color.white, 10.0f);
            }
        }
    }
}
