using UnityEngine;
using System.Collections;

/// <summary>
/// Defines preset sets of cells for blueprints.
/// </summary>
[System.Serializable]
public enum BlueprintPreset
{
    OneByOne,
    TwoByTwo,
    ThreeByThree
}

/// <summary>
/// A struct to represent a blueprint of a building, consisting of the cells that the building will take.
/// </summary>
[System.Serializable]
public struct Blueprint 
{
    // The local block of cells that the blueprint can take up.
    public bool[,] Cells;

    // The local hook position in the blueprint's block of cells, used to center the blueprint around something other than cell [0, 0].
    public GridPosition LocalHook;

    // What units can pass over this building.
    public GridcellPassability Passability;

    public Blueprint(bool[,] cells, GridPosition localHook, GridcellPassability passability)
    {
        this.Cells = cells;
        this.LocalHook = localHook;
        this.Passability = passability;
    }

    /// <summary>
    /// Convert a preset blueprint enum entry to an actual blueprint object.
    /// </summary>
    /// <param name="preset">The blueprint enum entry to convert.</param>
    /// <returns></returns>
    public static Blueprint EnumToBlueprint(BlueprintPreset preset)
    {
        switch (preset)
        {
            case BlueprintPreset.OneByOne:
                return Blueprint.OneByOne;
            case BlueprintPreset.TwoByTwo:
                return Blueprint.TwoByTwo;
            case BlueprintPreset.ThreeByThree:
                return Blueprint.ThreeByThree;
            default:
                return Blueprint.OneByOne;
        }
    }

    /// <summary>
    /// Return a blueprint object of a fully occupied 1x1 region.
    /// Localhook = [0,0]
    /// </summary>
    public static Blueprint OneByOne
    {
        get 
        { 
            bool[,] cells = new bool[,] { { true } };
            GridPosition localHook = new GridPosition(0, 0);
            return new Blueprint(cells, localHook, GridcellPassability.Flying);
        }
    }

    /// <summary>
    /// Return a blueprint object of a fully occupied 2x2 region.
    /// Localhook = [0,1]
    /// </summary>
    public static Blueprint TwoByTwo
    {
        get
        {
            bool[,] cells = new bool[,] { { true, true }, { true, true } };
            GridPosition localHook = new GridPosition(0, 1);
            return new Blueprint(cells, localHook, GridcellPassability.Flying);
        }
    }

    /// <summary>
    /// Return a blueprint object of a fully occupied 3x3 region.
    /// Localhook = [0,2]
    /// </summary>
    public static Blueprint ThreeByThree
    {
        get
        {
            bool[,] cells = new bool[,] { { true, true, true }, { true, true, true }, { true, true, true } };
            GridPosition localHook = new GridPosition(0, 2);
            return new Blueprint(cells, localHook, GridcellPassability.Flying);
        }
    }
}
