using UnityEngine;
using System.Collections;

public class ChainTurret : Building 
{
    public override void RightClick(SoftwareMouse mouse, RaycastHit raycastHit)
    {
        if (ControllingPlayer.LowPower())
            return;

        // Handles manual attacks
        Collider other = raycastHit.collider;
        
        if (CanAttackOther(other.gameObject))        
            Attack(other.gameObject.GetComponent<SelectableWithHealth>());
    }

    public override void StartConstruction(Building building)
    {
        Debug.Log("Chain Turret received an order to construct a building!");
    }

    public override void StartConstruction(Unit unit)
    {
        Debug.Log("Chain Turret received an order to construct a unit!");
    }

    public override void FinishConstruction()
    {
        Debug.Log("A Chain Turret somehow finished constructing something!");
    }
}
