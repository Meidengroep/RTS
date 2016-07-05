using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementOrder : Order 
{    
    private Vector2 goal;
    private Vector2 currentMovementDirection;
    private float currentMovementSpeed;
    private Pathfinder pathfinder;

    public MovementOrder(Unit unit, Vector2 goal)
        : base(unit)
    {
        this.goal = goal;
        this.pathfinder = new Pathfinder();
        this.pathfinder.ReplanPath(unit.gameObject.transform.position, new Vector3(goal.x, 0, goal.y), unit.Passability);
        this.pathfinder.CurrentPath.DrawPath(Color.green, 0, 10);
    }

    public override void Destoy()
    {
        // No classes to dispose yet.
    }

    /// <summary>
    /// Make the unit accelerate.
    /// </summary>
    private void Accelerate()
    {
        currentMovementSpeed += unit.Acceleration * Time.deltaTime;
        currentMovementSpeed = Mathf.Max(Mathf.Min(currentMovementSpeed, unit.MaxMovementSpeed), -unit.MaxMovementSpeed);
    }

    /// <summary>
    /// Make the unit deccelerate.
    /// </summary>
    private void Deccelerate()
    {
        currentMovementSpeed -= unit.Decceleration * Time.deltaTime;
        currentMovementSpeed = Mathf.Max(Mathf.Min(currentMovementSpeed, unit.MaxMovementSpeed), -unit.MaxMovementSpeed);
    }

    /// <summary>
    /// Follow the current path.
    /// </summary>
    private void FollowPath()
    {
        // A vec3 for convenience.
        Vector3 unitPos3 = unit.gameObject.transform.position;

        // Get the grid cell that this unit is currently in.
        GridPosition currentUnitPosition = pathfinder.Grid.GetGridPosition(unitPos3);
        pathfinder.CurrentPath.UpdateTargetNode(currentUnitPosition);

        if (pathfinder.CurrentPath.HasReachedFinalNode())
        {
            SetMovementDirectlyToGoal();
            return;
        }

        GridPosition currentGoalNode = pathfinder.CurrentPath.GetCurrentGridPosition();

        // Set the movement direction towards the center of the current goal node.
        Vector2 currentGoalNode2 = pathfinder.Grid.GetGridCellCenter(currentGoalNode);
        Vector2 unitPos2 = new Vector2(unitPos3.x, unitPos3.z);

        currentMovementDirection = currentGoalNode2 - unitPos2;
        currentMovementDirection.Normalize();
    }

    /// <summary>
    /// Set the movement direction to head directly towards the goal.
    /// </summary>
    private void SetMovementDirectlyToGoal()
    {
        Vector2 unitPos2 = new Vector2(unit.gameObject.transform.position.x, unit.gameObject.transform.position.z);
        currentMovementDirection = (goal - unitPos2).normalized;
    }

	public override void Update() 
    {
        FollowPath();
        Vector2 currenPos2D = new Vector2(unit.transform.position.x, unit.transform.position.z);
        Vector3 lookatPos = new Vector3(unit.transform.position.x + currentMovementDirection.x, unit.transform.position.y, unit.transform.position.z + currentMovementDirection.y);
        unit.transform.LookAt(lookatPos);

        // If this timestep we have enough speed to reach the target, just stand on the target and finish.
        if ((currenPos2D - goal).magnitude < unit.MinMovementTargetRange)
        {
            currentMovementSpeed = 0;
            isFinished = true;
            unit.GetComponent<Rigidbody>().MovePosition(new Vector3(goal.x, unit.transform.position.y, goal.y));
        }
        else
        {
            Accelerate();
            unit.GetComponent<Rigidbody>().MovePosition(unit.transform.position + new Vector3(currentMovementDirection.x * currentMovementSpeed * Time.deltaTime, 0, currentMovementDirection.y * currentMovementSpeed * Time.deltaTime));
        }
	}
}
