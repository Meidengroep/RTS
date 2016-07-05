using UnityEngine;
using System.Collections;

public class HarvesterAI : MonoBehaviour 
{
    public Harvester ControlledHarvester;

    private LavaField[] lavaFields;
    private HarvesterState state;

    private LavaField currentTargetField;
    private LavaNode currentTargetNode;
    private float leftOverTimeSinceLastHarvest;

    private Refinery currentTargetRefinery;
    private float leftOverTimeSinceLastDeposit;

    private bool aiEnabled;

	void Start () 
	{
        this.aiEnabled = true;
        this.lavaFields = GameObject.FindObjectsOfType<LavaField>();
        this.state = HarvesterState.Idle;
	}

    public void Disable()
    {
        aiEnabled = false;      
        leftOverTimeSinceLastHarvest = 0;
        leftOverTimeSinceLastDeposit = 0;
    }

    public void Enable(Refinery targetRefinery)
    {
        currentTargetRefinery = targetRefinery;
        PlanReturnTrip();
        state = HarvesterState.ReturningToRefinery;

        Enable();
    }

    public void Enable(LavaNode targetNode)
    {
        currentTargetNode = targetNode;
        MoveToCurrentNode();
        state = HarvesterState.MovingToLava;

        Enable();
    }

    public void Enable(HarvesterState newState)
    {
        state = newState;

        Enable();
    }

    private void Enable()
    {
        aiEnabled = true;
    }

	void Update () 
	{
        if (!aiEnabled)
            return;

        switch (state)
        {
            case HarvesterState.Idle:
                IdleAction();
                break;
            case HarvesterState.MovingToLava:
                MovingAction();
                break;
            case HarvesterState.HarvestingLava:
                HarvestingAction();
                break;
            case HarvesterState.ReturningToRefinery:
                ReturningAction();
                break;
            case HarvesterState.Depositing:
                DepositingAction();
                break;
        }
	}

    /// <summary>
    /// Find closest LavaField and send a move command to the unit.
    /// </summary>
    private void IdleAction()
    {
        // Find closest LavaField.
        LavaField closestField = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < lavaFields.Length; i++)
        {
            float distance = (ControlledHarvester.gameObject.transform.position - lavaFields[i].gameObject.transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestField = lavaFields[i];
            }
        }

        currentTargetField = closestField;

        // Check if something went wrong.
        if (currentTargetField == null)
        {
            Debug.Log("Harvester can't find a lava field!");
            return;
        }

        // Check if the field has any lava left, otherwise we move to the field center and wait.
        currentTargetNode = currentTargetField.GetRandomNonEmptyNode();

        if (MoveToCurrentNode())
        {
            state = HarvesterState.MovingToLava;
        }
        else
            return;
    }

    private bool MoveToCurrentNode()
    {
        if (currentTargetNode == null)
            return false;
        else
        {
            ControlledHarvester.MovementOrder(currentTargetNode.gameObject.transform.position);
            return true;
        }
    }

    /// <summary>
    /// Checks if we have arrived at our target node.
    /// </summary>
    private void MovingAction() 
    {
        if (ControlledHarvester.IsIdle())
        {
            // Check if we have truly arrived
            if ((this.gameObject.transform.position - currentTargetNode.gameObject.transform.position).magnitude < ControlledHarvester.MaxHarvestRange)
                state = HarvesterState.HarvestingLava;
            else
                state = HarvesterState.Idle;
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// Harvests the currentTargetNode until the harvester is full or until the node is depleted.
    /// </summary>
    private void HarvestingAction()
    {
        // Check if we can still harvest our current node.
        if (currentTargetNode.CurrentLava > 0)
        {
            // Check how much we can harvest since the laste update.
            float totalHarvestTime = Time.deltaTime + leftOverTimeSinceLastHarvest;

            int harvestThisUpdate = Mathf.FloorToInt(totalHarvestTime * ControlledHarvester.HarvestsPerSecond);
            leftOverTimeSinceLastHarvest = totalHarvestTime - ((float)harvestThisUpdate / (float)ControlledHarvester.HarvestsPerSecond);

            harvestThisUpdate = Mathf.Min(harvestThisUpdate, currentTargetNode.CurrentLava);

            int availableSpace = ControlledHarvester.MaxCapacity - ControlledHarvester.CurrentLava;
            if (availableSpace > harvestThisUpdate)
            {
                ControlledHarvester.CurrentLava += harvestThisUpdate;
                currentTargetNode.MineLava(harvestThisUpdate);
            }
            else
            {
                ControlledHarvester.CurrentLava += availableSpace;
                currentTargetNode.MineLava(availableSpace);
                leftOverTimeSinceLastHarvest = 0;

                if (PlanReturnTrip())
                    state = HarvesterState.ReturningToRefinery;
                else return;
            }
        }
        else
        {
            state = HarvesterState.Idle;
        }
    }
    /// <summary>
    /// Plan our return trip.
    /// </summary>
    /// <returns>If the plan went well.</returns>
    private bool PlanReturnTrip()
    {           
        // Find closest Refinery.
        Refinery[] refineries = GameObject.FindObjectsOfType<Refinery>();
        Refinery closestRefinery = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < refineries.Length; i++)
        {
            Refinery refinery = refineries[i];
            // Check if it is a friendly refinery
            if (!PlayerControlHelper.SamePlayerControl(refinery.gameObject.layer, ControlledHarvester.gameObject.layer))
                continue;

            float distance = (refinery.gameObject.transform.position - ControlledHarvester.gameObject.transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestRefinery = refinery;
            }
        }

        currentTargetRefinery = closestRefinery;

        // Check if we have no refineries
        if (closestRefinery == null)
            return false;

        // Move to the refinery's docking point
        ControlledHarvester.MovementOrder(currentTargetRefinery.HarvesterDockingPoint.position);        
        return true;
    }

    /// <summary>
    /// Checks if we have arrived at the docking point, and puts the harvester in the right position.
    /// </summary>
    private void ReturningAction() 
    {
        if (!ControlledHarvester.IsIdle())
            return;

        ControlledHarvester.gameObject.transform.position = currentTargetRefinery.HarvesterDepositPoint.position;
        ControlledHarvester.gameObject.transform.rotation = currentTargetRefinery.HarvesterDepositPoint.rotation;

        state = HarvesterState.Depositing;
    }

    /// <summary>
    /// Deposits lava into the refinery.
    /// </summary>
    private void DepositingAction() 
    {
        // Check how much we can deposit since the laste update.
        float totalDepositTime = Time.deltaTime + leftOverTimeSinceLastDeposit;

        int depositThisUpdate = Mathf.FloorToInt(totalDepositTime * ControlledHarvester.DepositPerSecond);
        leftOverTimeSinceLastDeposit = totalDepositTime - ((float)depositThisUpdate / (float)ControlledHarvester.DepositPerSecond);

        depositThisUpdate = Mathf.Min(depositThisUpdate, ControlledHarvester.CurrentLava);

        ControlledHarvester.ControllingPlayer.AddCredits(depositThisUpdate);
        ControlledHarvester.CurrentLava -= depositThisUpdate;

        if (ControlledHarvester.CurrentLava == 0)
            state = HarvesterState.Idle;
    }
}

public enum HarvesterState
{
    Idle = 0,
    MovingToLava = 1,
    HarvestingLava = 2,
    ReturningToRefinery = 3,
    Depositing = 4
}