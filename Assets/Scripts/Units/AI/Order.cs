using UnityEngine;
using System.Collections;

public abstract class Order
{
    protected Unit unit;
    protected bool isFinished;

    public Order(Unit unit)
    {
        this.unit = unit;
    }

    public bool IsFinished()
    {
        return isFinished;
    }

    public abstract void Destoy();

    /// <summary>
    /// Updates the order.
    /// </summary>
    public abstract void Update();
}
