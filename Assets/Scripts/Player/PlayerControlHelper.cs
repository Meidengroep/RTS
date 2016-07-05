using UnityEngine;
using System.Collections;

/// <summary>
/// Contains static methods to do things with layers and tags and player control.
/// </summary>
public static class PlayerControlHelper 
{
    /// <summary>
    /// Return if a layer is in a team.
    /// </summary>
    public static bool IsPlayerControlled(int layer)
    {
        string name = LayerMask.LayerToName(layer);
        if (name.Length < Layers.PlayerPrefix.Length + 1)
            return false;

        return name.Substring(0, 6) == Layers.PlayerPrefix && (name[Layers.PlayerPrefix.Length] == '1' || name[Layers.PlayerPrefix.Length] == '2');
    }

    /// <summary>
    /// Return true if both layers are on the same team.
    /// </summary>
    public static bool SamePlayerControl(int layer1, int layer2)
    {
        string name1 = LayerMask.LayerToName(layer1);
        string name2 = LayerMask.LayerToName(layer2);

        return name1[Layers.PlayerPrefix.Length] == name2[Layers.PlayerPrefix.Length];
    }
}
