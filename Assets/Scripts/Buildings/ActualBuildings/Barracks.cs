using UnityEngine;
using System.Collections;

public class Barracks : Building 
{ 
    public override BuildingsAndUnitsEnum GetCurrentConstructingType()
    {
        return constructingUnit.Type;
    }

    public override void StartConstruction(Building building)
    {
        Debug.Log("Barracks received an order to construct a building!");
    }

    public override void StartConstruction(Unit unit)
    {
        if (isConstructing)
            Debug.Log("Barracks received an order to construct while it is still constructing something else!");

        base.StartConstruction(unit);
    }

    public override void FinishConstruction()
    {
        if (!isConstructing)
            Debug.Log("A Barracks finished constructing while it wasn't constructing anything!");

        SpawnConstructedUnit();
        isConstructing = false;
    }
}
