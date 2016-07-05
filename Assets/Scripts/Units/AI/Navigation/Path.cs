using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path
{
    private BuildingGrid grid;
    private List<GridPosition> path;
    private int currentIndex;

    public Path(BuildingGrid grid, List<GridPosition> path)
    {
        this.grid = grid;
        this.path = path;
        this.currentIndex = 0;
    }

    public GridPosition GetCurrentGridPosition()
    {
        return path[currentIndex];
    }

    public bool HasReachedFinalNode()
    {
        return currentIndex == path.Count - 1;
    }

    public void MoveToPosition(Vector3 position)
    {
        GridPosition gridPosition = grid.GetGridPosition(position);
        UpdateTargetNode(gridPosition);
    }

    public void UpdateTargetNode(GridPosition currentObjectPosition)
    {
        for (int i = currentIndex; i < path.Count; i++)
        {
            if (currentObjectPosition == path[i])
            {
                currentIndex = Mathf.Min(path.Count - 1, i + 1);
                return;
            }
        }

        for (int i = 0; i < currentIndex; i++)
        {
            if (currentObjectPosition == path[i])
            {
                currentIndex = Mathf.Min(path.Count - 1, i + 1);
                return;
            }
        }
    }

    public void DrawPath(Color color, float height = 0, float time = 0)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            //Debug.Log("line");
            Debug.DrawLine(grid.GetWorldPosition(path[i]) + Vector3.up * height,
                           grid.GetWorldPosition(path[i + 1]) + Vector3.up * height,
                           color,
                           time);
        }
    }
}
