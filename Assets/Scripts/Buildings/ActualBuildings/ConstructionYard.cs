using UnityEngine;
using System.Collections;

public class ConstructionYard : Building 
{
    public override BuildingsAndUnitsEnum GetCurrentConstructingType()
    {
        return constructingBuilding.Type;
    }

    public override void StartConstruction(Building building)
    {
        if (isConstructing)
            Debug.Log("Construction Yard received an order to construct while it is still constructing something else!");

        base.StartConstruction(building);
    }

    public override void StartConstruction(Unit unit)
    {
        Debug.Log("Construction Yard received an order to construct a unit!");
    }

    public override void FinishConstruction()
    {
        if (!isConstructing)
            Debug.Log("A Construction Yard finished constructing while it wasn't constructing anything!");

        isConstructing = false;
        buildingReady = true;
    }
}
