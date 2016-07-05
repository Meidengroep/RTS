using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinder
{
    public BuildingGrid Grid { get; private set; }

    public Path CurrentPath;

    public Pathfinder() 
    {
	    this.Grid = BuildingGrid.GetGrid();
	}

    public void ReplanPath(Vector3 start, Vector3 end, GridcellPassability passability)
    {
        GridPosition startGrid = Grid.GetGridPosition(start);
        List<GridPosition> path;

        // Check if the movement order was given out of bounds. If so, we don't move at all.
        if (!Grid.IsInGridBounds(end))
            path = new List<GridPosition>() { startGrid };
        else
        {
            GridPosition endGrid = Grid.GetGridPosition(end);
            path = AStarPlanner(Grid, startGrid, endGrid, passability, Grid.GetRaycastHeight());
        }

        if (path.Count == 0)
            path.Add(startGrid);

        Debug.Log(path.Count);
        this.CurrentPath = new Path(Grid, path);
    }

    private static List<GridPosition> AStarPlanner(BuildingGrid grid, GridPosition start, GridPosition goal, GridcellPassability passability, float raycastHeight)
    {
        List<AStarNode> open = new List<AStarNode>();
        open.Add(new AStarNode(start, grid.GetGridCellCenter(start), null, 0, GetHeuristic(start, goal)));

        HashSet<AStarNode> closed = new HashSet<AStarNode>();        

        AStarNode current = null;

        while (open.Count > 0)
        {
            float bestF = float.MaxValue;
            int bestIndex = -1;
            for (int i = 0; i < open.Count; i++)
            {
                if (open[i].FScore < bestF)
                {
                    bestF = open[i].FScore;
                    bestIndex = i;
                }
            }

            current = open[bestIndex];
            open.RemoveAt(bestIndex);

            List<GridPosition> neighbours = grid.GetEmptyNeighbours(current.GridPosition, passability);

            for (int i = 0; i < neighbours.Count; i++)
            {
                AStarNode neighbour = new AStarNode(neighbours[i], grid.GetGridCellCenter(neighbours[i]), current, current.GScore + 1, current.GScore + 1 + GetHeuristic(neighbours[i], goal));
                if (!closed.Contains(neighbour))
                {
                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                    else
                    {
                        int index = open.IndexOf(neighbour);
                        if (open[index].GScore > neighbour.GScore)
                        {
                            open.RemoveAt(index);
                            open.Add(neighbour);
                        }
                    }
                }
            }

            closed.Add(current);

            if (current.GridPosition == goal)
                break;
        }

        List<AStarNode> aStarNodePath = new List<AStarNode>();

        while (current.Parent != null)
        {
            aStarNodePath.Add(current);
            current = current.Parent;
        }

        SmoothPath(aStarNodePath, raycastHeight);

        List<GridPosition> path = new List<GridPosition>();
        for (int i = 0; i < aStarNodePath.Count; i++)
            path.Add(aStarNodePath[i].GridPosition);

        path.Reverse();
        //for (int i = 0; i < path.Count; i++)
            //Debug.Log(path[i].ToString());

        return path;
    }

    private static void SmoothPath(List<AStarNode> path, float raycastHeight)
    {
        return;
        // TODO ray heights for ground/hover/flying

        for (int i = 0; i < path.Count; i++)
        {
            int nextNodeIndex = i + 1;
            if (nextNodeIndex >= path.Count)
                break;

            Vector3 currentNodePosition = new Vector3(path[i].ActualPosition.x, raycastHeight, path[i].ActualPosition.y);
            Vector3 nextNodePosition = new Vector3(path[nextNodeIndex].ActualPosition.x, raycastHeight, path[nextNodeIndex].ActualPosition.y);
            float distanceBetweenNodes = Vector3.Distance(currentNodePosition, nextNodePosition);

            // Shoot rays until we hit something.
            while (true)
            {
                RaycastHit hit;
                bool hitSomething = Physics.Raycast(currentNodePosition, nextNodePosition - currentNodePosition, out hit);

                // If we hit nothing, or we hit something too far away, continue the casting. Otherwise break;
                if (!hitSomething)
                {
                    nextNodeIndex = nextNodeIndex + 1;
                    if (nextNodeIndex >= path.Count)
                        break;

                    nextNodePosition = new Vector3(path[nextNodeIndex].ActualPosition.x, raycastHeight, path[nextNodeIndex].ActualPosition.y);
                    distanceBetweenNodes = Vector3.Distance(currentNodePosition, nextNodePosition);
                    continue;
                }
                else
                {
                    if ((hit.collider.gameObject.tag == Tags.Building || LayerMask.LayerToName(hit.collider.gameObject.layer) == Layers.TerrainLayerName) && hit.distance < distanceBetweenNodes)
                    {
                        Debug.Log("hit collider");
                        break;                        
                    }
                    else
                    {
                        nextNodeIndex = nextNodeIndex + 1;
                        if (nextNodeIndex >= path.Count)
                            break;

                        nextNodePosition = new Vector3(path[nextNodeIndex].ActualPosition.x, raycastHeight, path[nextNodeIndex].ActualPosition.y);
                        distanceBetweenNodes = Vector3.Distance(currentNodePosition, nextNodePosition);
                        continue;
                    }                    
                }
            }

            // When we get out of the loop, we take nextNodeIndex - 1 as our new new node. We delete all node between nextNodeIndex and i.
            for (int d = 0; d < nextNodeIndex - i - 1; d++)
            {
                path.RemoveAt(i + 1);
            }
        }
    }

    private static float GetHeuristic(GridPosition current, GridPosition goal)
    {
        return Mathf.Abs(current.X - goal.X) + Mathf.Abs(current.Z - goal.Z);
    }
}
