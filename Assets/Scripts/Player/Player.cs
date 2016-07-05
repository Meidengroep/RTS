using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Contains everything belonging to a single player.
/// </summary>
public class Player : MonoBehaviour 
{
    public Color Color;
    public ModeController ModeController;
    public HUD HUD;
    public FogOfWarCPU FogOfWar;
    public Building FirstConstructionYard;
    public Player EnemyPlayer;

    private List<Unit> units;
    private List<Building> buildings;
    private int credits;
    public int StartingCredits;
    public int CreditPerSecond;
    private float partialCredit;
    private bool haveToAddConyard;
    private int currentPowerSupply;
    private int currentPowerConsumption;
    private Rights rights;

	void Start () 
    {
        units = new List<Unit>();
        buildings = new List<Building>();
        credits = StartingCredits;
        haveToAddConyard = true;
        rights = new Rights();
	}

    #region Credits

    /// <summary>
    /// Get the amount of credits this player currently has.
    /// </summary>
    /// <returns>The amount of credits this player currently has.</returns>
    public int GetCredits()
    {
        return credits;
    }

    public void AddCredits(int amount)
    {
        this.credits += amount;
    }

    /// <summary>
    /// Subtracts a given amount of credits from the player's balance. Allows for negative balance!
    /// </summary>
    /// <param name="credits">The credits to subtract.</param>
    public void Pay(int credits)
    {
        if (credits > this.credits)
        {
            Debug.Log("Payment was made which led to a negative balance for a Player!");
        }
        this.credits -= credits;
    }

    #endregion

    #region Power

    public bool LowPower()
    {
        return this.currentPowerConsumption > this.currentPowerSupply;
    }

    public int GetPowerSupply()
    {
        return this.currentPowerSupply;
    }

    public void IncreasePowerSupply(int amount)
    {
        currentPowerSupply += amount;
    }

    public void DecreasePowerSupply(int amount)
    {
        currentPowerSupply -= amount;
        if (currentPowerSupply < 0)
            Debug.Log("Power Supply went below zero, something is wrong!");
    }

    public int GetPowerConsumption()
    {
        return this.currentPowerConsumption;
    }

    public void IncreasePowerConsumption(int amount)
    {
        currentPowerConsumption += amount;
    }

    public void DecreasePowerConsumption(int amount)
    {
        currentPowerConsumption -= amount;
        if (currentPowerConsumption < 0)
            Debug.Log("Power Consumption went below zero, something is wrong!");
    }

    #endregion

    public bool HasRights(BuildingsAndUnitsEnum item)
    {
        return rights.HasRights(item);
    }

    void Update()
    {
        // First time setup to start with a conyard.
        if (haveToAddConyard)
        {
            AddBuilding(FirstConstructionYard);
            haveToAddConyard = false;
        }

        float addedCreditsF = CreditPerSecond * Time.deltaTime + partialCredit;
        int addedCredits = Mathf.FloorToInt(addedCreditsF);

        partialCredit = addedCreditsF - addedCredits;

        credits += addedCredits;
    }

    /// <summary>
    /// Get the list of units owned by this player.
    /// </summary>
    /// <returns>The list of units owned by this player.</returns>
    public List<Unit> GetUnits()
    {
        return units;
    }

    /// <summary>
    /// Gets the list of buildings owned by this player.
    /// </summary>
    /// <returns>The list of buildings owned by this player.</returns>
    public List<Building> GetBuildings()
    {
        return buildings;
    }

    /// <summary>
    /// Checks if any units or buildings are dead, and destroys them if so.
    /// </summary>
    public void BringOutYourDead()
    {
        for (int u = 0; u < units.Count; u++)
        {
            Unit unit = units[u];
            if (unit.IsDead())
            {
                units.RemoveAt(u);
                u--;

                DestroyUnit(unit);
            }
        }

        for (int b = 0; b < buildings.Count; b++)
        {
            Building building = buildings[b];
            if (building.IsDead())
            {
                buildings.RemoveAt(b);
                b--;

                DestroyBuilding(building);
            }
        }
    }

    /// <summary>
    /// Add a unit to the list of player-owned units.
    /// </summary>
    /// <param name="unit">The unit to add.</param>
    public void AddUnit(Unit unit)
    {
        units.Add(unit);
    }

    /// <summary>
    /// Destroys a unit from the list of player-owned units (does not remove it from the list).
    /// </summary>
    /// <param name="unit">The units to destroy.</param>
    public void DestroyUnit(Unit unit)
    {
        ModeController.SelectionManager.GetSelectedObjects().Remove(unit);
        Destroy(unit.gameObject);
    }

    /// <summary>
    /// Add a building to the list of player-owned building.
    /// </summary>
    /// <param name="unit">The building to add.</param>
    public void AddBuilding(Building building)
    {
        buildings.Add(building);
        building.AssignPlayer(this);
        rights.AddRights(building);

        switch (building.Type)
        {
            case BuildingsAndUnitsEnum.ConstructionYard:
                HUD.AddConstructionYard(building);
                break;
            case BuildingsAndUnitsEnum.Barracks:
                HUD.AddBarracks(building);
                break;
            case BuildingsAndUnitsEnum.WarFactory:
                HUD.AddWarFactory(building);
                break;
        }
    }

    /// <summary>
    /// Destroys a building from the list of player-owned buildings (does not remove it from the list).
    /// </summary>
    /// <param name="unit">The building to destroy.</param>
    public void DestroyBuilding(Building building)
    {
        ModeController.SelectionManager.GetSelectedObjects().Remove(building);

        switch (building.Type)
        {
            case BuildingsAndUnitsEnum.ConstructionYard:
                HUD.RemoveConstructionYard(building);
                break;
            case BuildingsAndUnitsEnum.Barracks:
                HUD.RemoveBarracks(building);
                break;
            case BuildingsAndUnitsEnum.WarFactory:
                HUD.RemoveWarFactory(building);
                break;
        }

        Destroy(building.gameObject);
    }

}
