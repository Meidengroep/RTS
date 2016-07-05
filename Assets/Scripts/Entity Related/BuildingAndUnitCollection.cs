using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingAndUnitCollection : MonoBehaviour 
{
    public Building[] buildings;
    public Unit[] units;    

    private int unitOffset;

	void Start () 
    {
        unitOffset = (int)BuildingsAndUnitsEnum.BuildingCount + 1;   
	}

    /// <summary>
    /// Get a building prefab given a BuildingsAndUnitsEnum entry.
    /// </summary>
    /// <param name="building">The BuildingsAndUnitsEnum entry.</param>
    /// <returns>The building prefab.</returns>
    public Building GetBuilding(BuildingsAndUnitsEnum building)
    {
        int index = (int)building;
        return buildings[index];
    }

    /// <summary>
    /// Get a unit prefab given a BuildingsAndUnitsEnum entry.
    /// </summary>
    /// <param name="unit">The BuildingsAndUnitsEnum entry.</param>
    /// <returns>The unit prefab.</returns>
    public Unit GetUnit(BuildingsAndUnitsEnum unit)
    {
        int index = (int)unit;
        return units[index - unitOffset];
    }
}
