using UnityEngine;
using System.Collections;

public class LavaField : MonoBehaviour 
{
    public LavaNode[] LavaNodes;

    public float TimePerDig;
    public int LavaAddedPerDig;

    private BuildingGrid Grid;
    private GridPosition[] LavaNodePositions;
    private float currentDigCooldown;

	void Start () 
	{
        this.Grid = BuildingGrid.GetGrid();
        GridPosition fieldPosition = this.Grid.GetGridPosition(this.gameObject.transform.position);
        if (this.Grid.OccupyCell(fieldPosition, GridcellPassability.Flying))
            Debug.Log("Trying to occupy cell that is already occupied");

        this.currentDigCooldown = TimePerDig;
	}

    /// <summary>
    /// Get a random LavaNode that is not empty. Return null if all nodes are empty.
    /// </summary>
    /// <returns>A LavaNode that is not empty, or null if all are empty.</returns>
    public LavaNode GetRandomNonEmptyNode()
    {
        // Find a non-empty node.
        int randomIndex = Random.Range(0, LavaNodes.Length - 1);

        if (LavaNodes[randomIndex].CurrentLava > 0)
            return LavaNodes[randomIndex];
        else
        {
            // Here we go...
            int upCounter = randomIndex + 1;
            int downCounter = randomIndex - 1;

            while (upCounter < LavaNodes.Length || downCounter >= 0)
            {
                if (upCounter < LavaNodes.Length)
                {
                    if (LavaNodes[upCounter].CurrentLava  > 0)
                    {
                        return LavaNodes[upCounter];
                    }

                    upCounter++;
                }

                if (downCounter >= 0)
                {
                    if (LavaNodes[downCounter].CurrentLava > 0)
                    {
                        return LavaNodes[downCounter];
                    }

                    downCounter--;
                }
            }
        }

        return null;
    }

	void Update () 
	{
        currentDigCooldown -= Time.deltaTime;
        if (currentDigCooldown <= 0)
        {
            currentDigCooldown = TimePerDig;
            Dig();
        }
	}

    /// <summary>
    /// Find a non-full node and add more lava to it.
    /// </summary>
    private void Dig()
    {
        // Find a non-full node.
        int randomIndex = Random.Range(0, LavaNodes.Length - 1);

        if (LavaNodes[randomIndex].CurrentLava < LavaNodes[randomIndex].MaxLava)
            LavaNodes[randomIndex].ReplenishLava(LavaAddedPerDig);
        else
        {
            // Here we go...
            int upCounter = randomIndex + 1;
            int downCounter = randomIndex - 1;

            while (upCounter < LavaNodes.Length || downCounter >= 0)
            {
                if (upCounter < LavaNodes.Length)
                {
                    if (LavaNodes[upCounter].CurrentLava < LavaNodes[upCounter].MaxLava)
                    {
                        LavaNodes[upCounter].ReplenishLava(LavaAddedPerDig);
                        break;
                    }

                    upCounter++;
                }

                if (downCounter >= 0)
                {
                    if (LavaNodes[downCounter].CurrentLava < LavaNodes[downCounter].MaxLava)
                    {
                        LavaNodes[downCounter].ReplenishLava(LavaAddedPerDig);
                        break;
                    }

                    downCounter--;
                }
            }
        }
    }
}
