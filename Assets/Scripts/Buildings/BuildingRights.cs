using UnityEngine;
using System.Collections;

public static class BuildingRights 
{
    public static BuildingsAndUnitsEnum[] GetRights(BuildingsAndUnitsEnum building)
    {
        switch (building)
        {
            case BuildingsAndUnitsEnum.ConstructionYard:
                return new BuildingsAndUnitsEnum[]
                {
                    BuildingsAndUnitsEnum.PowerPlant, 
                    BuildingsAndUnitsEnum.Barracks,
                    BuildingsAndUnitsEnum.Refinery
                };
            case BuildingsAndUnitsEnum.Barracks:
                return new BuildingsAndUnitsEnum[]
                {
                    BuildingsAndUnitsEnum.WarFactory,
                    BuildingsAndUnitsEnum.ChainTurret,
                    BuildingsAndUnitsEnum.RifleMan, 
                    BuildingsAndUnitsEnum.RocketMan
                };
            case BuildingsAndUnitsEnum.Refinery:
                return new BuildingsAndUnitsEnum[]
                {
                    BuildingsAndUnitsEnum.Harvester
                };
            case BuildingsAndUnitsEnum.WarFactory:
                return new BuildingsAndUnitsEnum[]
                {
                    BuildingsAndUnitsEnum.CannonTurret,
                    BuildingsAndUnitsEnum.Buggy, 
                    BuildingsAndUnitsEnum.LightTank
                };
        }

        return new BuildingsAndUnitsEnum[0]; 
    }
}
