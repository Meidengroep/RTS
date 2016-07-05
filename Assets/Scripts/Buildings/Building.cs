using UnityEngine;
using System.Collections;

/// <summary>
/// Represent a building which can take orders, has health, and can be selected.
/// </summary>
public abstract class Building : SelectableWithHealth 
{
    public int Cost;

    public int PowerConsumption;
    public BlueprintPreset BlueprintPreset;
    public BlinkPreview Blink;
    public Texture Icon;

    protected SelectionMarker selectionMarker;

    protected Unit constructingUnit;
    protected Building constructingBuilding;
    protected int currentConstructionRequiredCredits;
    protected float constructionTimeLeftover;
    protected int constructionCost;
    protected bool isConstructing;
    protected bool buildingReady;
    protected bool wasVisibleLastUpdate;

	new public void Start () 
    {
        base.Start();

        selectionMarker = new SelectionMarker();
        selectionMarker.MarkerText = Type.ToString();
        selectionMarker.WorldPosition = new Vector3(0, 0, 0);

        InitHealth();

        currentConstructionRequiredCredits = 0;
        constructionTimeLeftover = 0;
        constructionCost = 0;
        isConstructing = false;
        ControllingPlayer.IncreasePowerConsumption(PowerConsumption);
        ControllingPlayer.FogOfWar.Uncover(this.gameObject.transform.position, ViewRange);

        wasVisibleLastUpdate = false;
	}

    /// <summary>
    /// Get the amount of credits spent on construction of the current building.
    /// </summary>
    /// <returns>The amount of credits spent on construction of the current building.</returns>
    public int GetConstructionProgress()
    {
        return constructionCost - currentConstructionRequiredCredits;
    }

    /// <summary>
    /// Start the construction of a building. 
    /// </summary>
    /// <param name="building">The building prefab to construct.</param>
    public virtual void StartConstruction(Building building)
    {
        currentConstructionRequiredCredits = building.Cost;
        constructionTimeLeftover = 0;
        constructionCost = building.Cost;
        constructingBuilding = building;
        isConstructing = true;
        buildingReady = false;
    }

    /// <summary>
    /// Start the construction of a unit. 
    /// </summary>
    /// <param name="unit">The unit prefab to construct.</param>
    public virtual void StartConstruction(Unit unit)
    {
        currentConstructionRequiredCredits = unit.Cost;
        constructionTimeLeftover = 0;
        constructionCost = unit.Cost;
        constructingUnit = unit;
        isConstructing = true;
    }

    public void CancelConstruction()
    {
        ControllingPlayer.AddCredits(GetConstructionProgress());
        constructingBuilding = null;
        constructingUnit = null;        
        buildingReady = false;
        isConstructing = false;
    }

    public abstract void FinishConstruction();

    /// <summary>
    /// Check if the currently constructing building is ready for placement.
    /// </summary>
    /// <returns>If the currently constructing building is ready for placement.</returns>
    public bool BuildingReady()
    {
        return buildingReady;
    }

    /// <summary>
    /// Get which building is currently being constructed by this building.
    /// </summary>
    /// <returns>The building that is being constructed.</returns>
    public Building GetCurrentConstructingBuilding()
    {
        return constructingBuilding;
    }

    /// <summary>
    /// Returns which building or unit is being constructed. Returns BuildingsAndUnitsEnum.Count if none is applicable.
    /// </summary>
    /// <returns>Which building or unit is being constructed. Returns BuildingsAndUnitsEnum.Count if none is applicable.</returns>
    public virtual BuildingsAndUnitsEnum GetCurrentConstructingType()
    {
        return BuildingsAndUnitsEnum.Count;
    }

    /// <summary>
    /// Call this when a building is placed, it notifies this building.
    /// </summary>
    public void BuildingPlaced()
    {
        buildingReady = false;
    }

    /// <summary>
    /// Returns if this building is constructing anything.
    /// </summary>
    /// <returns>If the building is constructing anything.</returns>
    public bool IsConstructing()
    {
        return isConstructing;
    }

    public override void RightClick(SoftwareMouse mouse, RaycastHit raycastHit)
    {
        Debug.Log("Building receiving an order event.");
    }

    /// <summary>
    /// Spawns a unit constructed by this building into the world.
    /// </summary>
    protected void SpawnConstructedUnit()
    {
        Unit unit = (Unit)Instantiate(constructingUnit, transform.position + Vector3.left * 10 + Vector3.up * 1, Quaternion.identity);
        unit.AssignPlayer(ControllingPlayer);
    }

    public override SelectionMarker GetSelectionMarker()
    {
        return selectionMarker;
    }

    /// <summary>
    /// Assign a player to this building. Used to for instance color the model of the building, and copy the player's layer.
    /// </summary>
    /// <param name="player">The player to be assigned to.</param>
    public virtual void AssignPlayer(Player player)
    {
        ControllingPlayer = player;
        gameObject.layer = player.gameObject.layer;

        int count = Model.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            GameObject child = Model.transform.GetChild(i).gameObject;
            child.GetComponent<Renderer>().material.color = player.Color;
        }
    }

    new public virtual void Update() 
    {
        base.Update();

        // TODO are doing double isvisible checks
        if (ControllingPlayer.EnemyPlayer.gameObject.activeSelf)
        {
            if (!IsVisible())
            {
                if (wasVisibleLastUpdate)
                {
                    wasVisibleLastUpdate = false;
                    GameObject newModel = (GameObject)GameObject.Instantiate(Model, Model.transform.position, Model.transform.rotation);
                    newModel.SetActive(true);
                    ControllingPlayer.EnemyPlayer.FogOfWar.AddBuildingToFog(this.gameObject.transform.position, newModel);
                }
            }
            else
            {
                wasVisibleLastUpdate = true;
            }
        }

        selectionMarker.WorldPosition = transform.position;
        ContinueConstruction();
	}

    /// <summary>
    /// Updates the current construction if any.
    /// </summary>
    private void ContinueConstruction()
    {
        if (!isConstructing)
            return;

        // 1 second = 100 credits;
        // Calculate how many credit we are allowed to spend in the time that has passed.
        float lowPowerModifier;
        if (ControllingPlayer.LowPower())
            lowPowerModifier = 0.5f;
        else lowPowerModifier = 1f;

        float creditsForTime = (Time.deltaTime * lowPowerModifier + constructionTimeLeftover) * 100f;
        int wholeCreditsForTime = Mathf.FloorToInt(creditsForTime);

        // Calculate the fraction of non-whole credits that is left over.
        float leftOverCreditsForTime = creditsForTime - wholeCreditsForTime;

        // Calculate the time that was left over by the non-whole credits.
        constructionTimeLeftover = leftOverCreditsForTime / 100f;

        int toPay = Mathf.Min(wholeCreditsForTime, currentConstructionRequiredCredits);
        int cantPay = PayForConstruction(toPay);
        int paid = toPay - cantPay;

        currentConstructionRequiredCredits -= paid;
        
        if (currentConstructionRequiredCredits == 0)
            FinishConstruction();
    }

    /// <summary>
    /// Attempts to pay payment with the controlling player's credits. Return the amount that could not be paid.
    /// </summary>
    /// <param name="payment">The amount to pay.</param>
    /// <returns>The amount that could not be payed.</returns>
    private int PayForConstruction(int payment)
    {
        int playerCredits = ControllingPlayer.GetCredits();
        if (playerCredits >= payment)
        {
            ControllingPlayer.Pay(payment);
            return 0;
        }
        else
        {
            ControllingPlayer.Pay(playerCredits);
            return payment - playerCredits;
        }
    }

    protected override void OnDeath()
    {
        // Refund Credits
        if (isConstructing)
            ControllingPlayer.AddCredits(constructionCost - currentConstructionRequiredCredits);

        // Refund Power
        ControllingPlayer.DecreasePowerConsumption(PowerConsumption);

        ControllingPlayer.FogOfWar.CoverUp(this.gameObject.transform.position, ViewRange);
    }
}
