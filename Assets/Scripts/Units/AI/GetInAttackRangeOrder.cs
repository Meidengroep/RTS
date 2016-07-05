using UnityEngine;
using System.Collections;

public class GetInAttackRangeOrder : Order 
{
    private MovementOrder movementOrder;
    private const float hackMultiplier = 0.999f; // To make sure the unit does not barely stay out of range due to rounding errors
    private Vector2 goal;

    public GetInAttackRangeOrder(Unit unit, Vector2 goal)
        : base(unit)
    {
        this.goal = goal;
        ComputeEndGoal();
    }

    private void ComputeEndGoal()
    {
        Vector2 direction = new Vector2(unit.gameObject.transform.position.x - goal.x, unit.transform.position.z - goal.y);

        if (direction.magnitude < unit.AttackRange * 0.999)
        {
            //Debug.Log("what");
            isFinished = true;
            return;
        }

        direction.Normalize();
        movementOrder = new MovementOrder(unit, goal + direction * unit.AttackRange * 0.999f);
    }

    public override void Update()
    {
        if (isFinished)
            return;
        //hier
        Vector2 unitPos2 = new Vector2(unit.gameObject.transform.position.x, unit.gameObject.transform.position.z);
        if (movementOrder.IsFinished() || (unitPos2 - goal).magnitude < unit.AttackRange * 0.999f)
        {
            isFinished = true;
            return;
        }

        movementOrder.Update();

        this.isFinished = movementOrder.IsFinished();
    }

    public override void Destoy()
    {
    }
}
