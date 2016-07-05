using UnityEngine;
using System.Collections;

public class Refinery : Building
{
    public Unit HarvesterPrefab;
    public Transform HarvesterDockingPoint;
    public Transform HarvesterDepositPoint;

    new public void Start()
    {
        base.Start();
        base.StartConstruction(HarvesterPrefab);
        this.SpawnConstructedUnit();
        this.isConstructing = false;
    }

    public override void StartConstruction(Building building)
    {
        Debug.Log("Refinery received an order to construct a building!");
    }

    public override void StartConstruction(Unit unit)
    {
        Debug.Log("Refinery received an order to construct a unit!");
    }

    public override void FinishConstruction()
    {
        Debug.Log("A Refinery somehow finished constructing something!");
    }
}
