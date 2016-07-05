using UnityEngine;
using System.Collections;

public class AttackOrder : Order 
{
    private GetInAttackRangeOrder getInRangeOrder;
    private SelectableWithHealth target;

    public AttackOrder(Unit unit, SelectableWithHealth target)
        : base(unit)
    {
        this.target = target;
        this.getInRangeOrder = new GetInAttackRangeOrder(unit, new Vector2(target.gameObject.transform.position.x, target.gameObject.transform.position.z));
    }

    public override void Update() 
	{
        if (!unit.IsVisible())
        {
            isFinished = true;
            return;
        }
        if (getInRangeOrder.IsFinished())
        {
            if (target == null)
            {
                isFinished = true;
                return;
            }
            if ((unit.gameObject.transform.position - target.gameObject.transform.position).magnitude < unit.AttackRange)
            {
                unit.transform.LookAt(target.gameObject.transform);

                unit.Attack(target.gameObject.GetComponent<SelectableWithHealth>());
            }
            else
                getInRangeOrder = new GetInAttackRangeOrder(unit, new Vector2(target.gameObject.transform.position.x, target.gameObject.transform.position.z));
        }
        else
            getInRangeOrder.Update();
            
	}

    public override void Destoy()
    {
    }
}
