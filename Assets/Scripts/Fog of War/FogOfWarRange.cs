using UnityEngine;
using System.Collections;

public struct FogOfWarRange 
{
    public bool[,] Grid;
    public GridPosition Center;
    public int Width;
    public int Height;

    public FogOfWarRange(bool[,] grid, GridPosition center, int width, int height)
    {
        this.Grid = grid;
        this.Center = center;
        this.Width = width;
        this.Height = height;
    }
}
