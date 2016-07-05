using UnityEngine;
using System.Collections;

public class AStarNode
{   
    public GridPosition GridPosition;
    public Vector2 ActualPosition;
    public AStarNode Parent;

    // The score to get to this node (so the actual set-in-stone score)
    public float GScore;

    // The predicted score to reach the final goal (GScore + Heuristic)
    public float FScore;

    public AStarNode(GridPosition gridPosition, Vector2 actualPosition, AStarNode parent, float gScore, float fScore)
    {
        this.GridPosition = gridPosition;
        this.ActualPosition = actualPosition;
        this.Parent = parent;
        this.GScore = gScore;
        this.FScore = fScore;
    }

    public override bool Equals(object obj)
    {
        return this.GridPosition == ((AStarNode)obj).GridPosition;
    }

    public override int GetHashCode()
    {
        return GridPosition.GetHashCode();
    }
}
