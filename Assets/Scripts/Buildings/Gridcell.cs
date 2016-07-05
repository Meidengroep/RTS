using UnityEngine;
using System.Collections;

public struct Gridcell 
{
    public GridcellPassability Passability;
    public float Height;
    public bool LeftExit;
    public bool RightExit;
    public bool UpExit;
    public bool DownExit;

	public Gridcell(float height) 
    {
        this.Passability = GridcellPassability.Ground;
        this.Height = height;
        this.LeftExit = false;
        this.RightExit = false;
        this.UpExit = false;
        this.DownExit = false;
	}
}
