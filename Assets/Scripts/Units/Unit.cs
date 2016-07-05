using UnityEngine;
using System.Collections;

/// <summary>
/// Contains basically all base unit functionality.
/// </summary>
public abstract class Unit : SelectableWithHealth
{
    #region Members

    public int Cost;

    public GridcellPassability Passability;
    public float MaxMovementSpeed;
    public float Acceleration;
    public float Decceleration;
    public float MinMovementTargetRange;

    public bool PlayerControlled;

    public Texture Icon;

    protected Order currentOrder;

    protected SelectionMarker selectionMarker;

    protected Vector3 prevPosition;

    #endregion

    new void Start () 
    {
        base.Start();
        selectionMarker = new SelectionMarker();
        selectionMarker.MarkerText = Type.ToString();
        selectionMarker.WorldPosition = new Vector3(0, 0, 0);

        InitHealth();

        prevPosition = this.gameObject.transform.position;
        ControllingPlayer.FogOfWar.Uncover(this.gameObject.transform.position, ViewRange);
	}

    public bool IsIdle()
    {
        return currentOrder == null || currentOrder.IsFinished();
    }

    #region Team Stuff
    /// <summary>
    /// Make this unit belong to a player by for instance changing color and such.
    /// </summary>
    /// <param name="player">The player that this unit will belong to.</param>
    public virtual void AssignPlayer(Player player)
    {
        ControllingPlayer = player;
        player.AddUnit(this);
        gameObject.layer = player.gameObject.layer;

        int count = Model.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            GameObject child = Model.transform.GetChild(i).gameObject;
            child.GetComponent<Renderer>().material.color = player.Color;
        }
    }
    #endregion

    /// <summary>
    /// Get the marker to show that this unit is selected.
    /// </summary>
    /// <returns>The selection marker for this unit.</returns>
    public override SelectionMarker GetSelectionMarker()
    {
        return selectionMarker;
    }

    /// <summary>
    /// Handles a right-click order event.
    /// </summary>
    /// <param name="mouse">The mouse that made the event.</param>
    /// <param name="worldPosition">The world position that the right click was created with.</param>
    public override void RightClick(SoftwareMouse mouse, RaycastHit raycastHit)
    {
        Collider other = raycastHit.collider;

        if (CanAttackOther(other.gameObject))
        {
            this.AttackOrder(other.gameObject.GetComponent<SelectableWithHealth>());
            return;
        }

        MovementOrder(raycastHit.point);     
    }

    /// <summary>
    /// Set the movement target for this unit.
    /// </summary>
    /// <param name="target">The new movement target.</param>
    public void MovementOrder(Vector3 target)
    {
        currentOrder = new MovementOrder(this, new Vector2(target.x, target.z));
    }

    /// <summary>
    /// Order the unit to attack something.
    /// </summary>
    /// <param name="target">The thing to attack.</param>
    public void AttackOrder(SelectableWithHealth target)
    {
        currentOrder = new AttackOrder(this, target);
    }

    /// <summary>
    /// Order a unit to get in attack range.
    /// </summary>
    /// <param name="target">The thing to get in attack range of.</param>
    public void GetInAttackRangeOrder(Vector3 target)
    {
        currentOrder = new GetInAttackRangeOrder(this, new Vector2(target.x, target.z));
    }

    new public virtual void Update () 
    {
        base.Update();

        if (currentOrder != null)
        {
            currentOrder.Update();

            if (currentOrder.IsFinished())
            {
                currentOrder.Destoy();
                currentOrder = null;
            }
        }

        if (prevPosition != this.transform.position)
        {
            ControllingPlayer.FogOfWar.CoverUp(prevPosition, ViewRange);
            ControllingPlayer.FogOfWar.Uncover(this.gameObject.transform.position, ViewRange);
        }

        prevPosition = gameObject.transform.position;

        selectionMarker.WorldPosition = gameObject.transform.position;
	}

    protected override void OnDeath()
    {
        ControllingPlayer.FogOfWar.CoverUp(this.gameObject.transform.position, ViewRange);
    }
}
