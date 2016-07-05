using UnityEngine;
using System.Collections;

/// <summary>
/// Contains an integer x and z position for use in the building grid.
/// </summary>
[System.Serializable]
public struct GridPosition 
{
    public int X;
    public int Z;

    public GridPosition(int x, int z)
    {
        this.X = x;
        this.Z = z;
    }

    public static bool operator ==(GridPosition o1, GridPosition o2)
    {
        return o1.X == o2.X && o1.Z == o2.Z;
    }

    public static bool operator !=(GridPosition o1, GridPosition o2)
    {
        return !(o1 == o2);
    }

    public static GridPosition operator +(GridPosition o1, GridPosition o2)
    {
        return new GridPosition(o1.X + o2.X, o1.Z + o2.Z);
    }

    public static GridPosition operator -(GridPosition o1, GridPosition o2)
    {
        return new GridPosition(o1.X - o2.X, o1.Z - o2.Z);
    }

    public override bool Equals(object obj)
    {
        GridPosition other = (GridPosition)obj;

        return this == other;
    }

    public override string ToString()
    {
        return "[" + X + ", " + Z + "]";
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() + Z.GetHashCode();
    }
}
