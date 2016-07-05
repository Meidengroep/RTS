using UnityEngine;
using System.Collections;

public class PowerPlant : Building
{
    public int PowerSupply;

    new public void Start()
    {
        base.Start();
        ControllingPlayer.IncreasePowerSupply(PowerSupply);
    }

    public override void StartConstruction(Building building)
    {
        Debug.Log("Power Plant received an order to construct a building!");
    }

    public override void StartConstruction(Unit unit)
    {
        Debug.Log("Power Plant received an order to construct while it is still constructing something else!");
    }

    public override void FinishConstruction()
    {
        Debug.Log("A Power Plant somehow finished constructing something!");
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        // Redact Power
        ControllingPlayer.DecreasePowerSupply(PowerSupply);
    }
}
