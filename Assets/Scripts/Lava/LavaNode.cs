using UnityEngine;
using System.Collections;

public class LavaNode : MonoBehaviour 
{
    public int MaxLava;

    private BuildingGrid Grid;
    private int currentLava;

	void Start () 
	{
        this.Grid = BuildingGrid.GetGrid();
        GridPosition nodePosition = this.Grid.GetGridPosition(this.gameObject.transform.position);
        if (this.Grid.OccupyCell(nodePosition, GridcellPassability.Hover))
            Debug.Log("Trying to occupy cell that is already occupied");
        this.currentLava = 250;
        SetProperHeight();
	}

    private void SetProperHeight()
    {
        float height = (1 - ((float)currentLava / MaxLava)) * -5;
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, height, this.gameObject.transform.position.z);
    }

    public int CurrentLava
    {
        get { return this.currentLava; }
    }

    public int MineLava(int desiredAmount)
    {
        if (currentLava >= desiredAmount)
        {
            currentLava -= desiredAmount;
            SetProperHeight();
            return desiredAmount;
        }
        else
        {
            int trueAmount = currentLava;
            currentLava = 0;
            SetProperHeight();
            return trueAmount;
        }
    }

    public void ReplenishLava(int amount)
    {
        currentLava += amount;
        if (currentLava > MaxLava)
            currentLava = MaxLava;
        SetProperHeight();
    }
}
