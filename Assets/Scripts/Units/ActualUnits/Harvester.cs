using UnityEngine;
using System.Collections;

public class Harvester : Unit 
{
    public HarvesterAI AI;

    public float MaxHarvestRange;
    public int MaxCapacity;

    //[HideInInspector]
    public int CurrentLava;

    public int HarvestsPerSecond;
    public int DepositPerSecond;

    public override void RightClick(SoftwareMouse mouse, RaycastHit raycastHit)
    {
        GameObject hit = raycastHit.collider.gameObject;

        // Check if we rightclicked a LavaNode or Refinery
        LavaNode node = hit.GetComponent<LavaNode>();
        if (node != null)
            AI.Enable(node);
        else
        {
            Refinery refinery = hit.GetComponent<Refinery>();
            if (refinery != null)
            {
                // Check for friendly refinery.
                if (PlayerControlHelper.SamePlayerControl(this.gameObject.layer, refinery.gameObject.layer))
                    AI.Enable(refinery);
            }
            else
            {
                AI.Disable();
                base.RightClick(mouse, raycastHit);
            }
        }
    }
}
