using UnityEngine;
using System.Collections;

public class Rights 
{
    private bool[] rights;

    public Rights()
    {
        rights = new bool[(int)BuildingsAndUnitsEnum.Count];
    }

    public bool HasRights(BuildingsAndUnitsEnum item)
    {
        return rights[(int)item];
    }

    public void AddRights(Building building)
    {
        AddRights(BuildingRights.GetRights(building.Type)); 
    }

    public void AddRights(BuildingsAndUnitsEnum[] objects)
    {
        for (int i = 0; i < objects.Length; i++)
            rights[(int)objects[i]] = true;
    }

    public void RemoveRights(Building building)
    {
        RemoveRights(BuildingRights.GetRights(building.Type));
    }

    public void RemoveRights(BuildingsAndUnitsEnum[] objects)
    {
        for (int i = 0; i < objects.Length; i++)
            rights[(int)objects[i]] = false;
    }
}
