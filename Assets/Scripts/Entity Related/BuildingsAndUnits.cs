using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Contains every buildable unit in the game, used for GUI and for defining what a building can produce.
/// </summary>
public enum BuildingsAndUnitsEnum
{
    ConstructionYard,
    PowerPlant,
    Refinery,
    Barracks,
    WarFactory,
    ChainTurret,
    CannonTurret,
    BuildingCount, // To split into two arrays.
    RifleMan,
    RocketMan,
    Buggy,
    LightTank, // Used to get the total amount of elements in the enum for iteration.
    Harvester,
    Count
}

/// <summary>
/// Contains sets of units and or buildings that fall in a certain tab,
/// </summary>
public static class BuildingsAndUnitsTabs
{
    public static BuildingsAndUnitsEnum[] NormalBuildingsTab = new BuildingsAndUnitsEnum[]
    {
        BuildingsAndUnitsEnum.PowerPlant, 
        BuildingsAndUnitsEnum.Refinery,
        BuildingsAndUnitsEnum.Barracks, 
        BuildingsAndUnitsEnum.WarFactory
    };

    public static BuildingsAndUnitsEnum[] UtilityBuildingsTab = new BuildingsAndUnitsEnum[]
    {
        BuildingsAndUnitsEnum.ChainTurret, 
        BuildingsAndUnitsEnum.CannonTurret 
    };

    public static BuildingsAndUnitsEnum[] InfantryTab = new BuildingsAndUnitsEnum[]
    {
        BuildingsAndUnitsEnum.RifleMan, 
        BuildingsAndUnitsEnum.RocketMan 
    };

    public static BuildingsAndUnitsEnum[] VehiclesTab = new BuildingsAndUnitsEnum[]
    {
        BuildingsAndUnitsEnum.Buggy, 
        BuildingsAndUnitsEnum.LightTank, 
        BuildingsAndUnitsEnum.Harvester
    };
}
