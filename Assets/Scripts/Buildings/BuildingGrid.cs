using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// |+ + + + + 
// |+ + + + + 
// |+ + + + +
// |+ + + + +
// 0 - - - -


/// <summary>
/// Contains the grid of the entire map where buildings can be placed.
/// It stretches from (0,0) to the thepositive x and z axis.
/// </summary>
public class BuildingGrid : MonoBehaviour
{
    #region Members

    public int XCount;
    public int ZCount;

    public float CellSize;

    public float maximumIncline;

    public GameObject Marker;

    private List<GameObject> markers;
    private Gridcell[,] grid;
    private float neighbourCheckHeight;
    private int terrainLayerMask;

    #endregion

    void Awake () 
    {
        // Create a layer mask to only shoot rays against the ground.
        terrainLayerMask = LayerMask.GetMask(Layers.TerrainLayerName);

        // Calculate the neighbour check height for the neighbour raycasts. 
        float aTan = Mathf.Atan(maximumIncline);
        neighbourCheckHeight = aTan * CellSize;
        Debug.Log("Neighbour Check Hight: " + neighbourCheckHeight);

        grid = new Gridcell[XCount, ZCount];
        for (int x = 0; x < XCount; x++)
            for (int z = 0; z < ZCount; z++)
            {
                // Shoot a ray from the heavens to earth to find the height of the center.
                Vector2 cellCenter2D = GetGridCellCenter(new GridPosition(x, z));
                Vector3 cellCenter = new Vector3(cellCenter2D.x, 0, cellCenter2D.y);
                Vector3 heaven = cellCenter + Vector3.up * 1000;
                RaycastHit heavenHit;
                if (!Physics.Raycast(heaven, Vector3.down, out heavenHit, Mathf.Infinity, terrainLayerMask))
                    throw new Exception("Gridcell is not on solid ground.");

                float height = heavenHit.point.y;

                Gridcell cell = new Gridcell(height);

                // Check what sides are exitable
                cellCenter.y = height;
                Vector3 sideRaycastStart = cellCenter + Vector3.up * neighbourCheckHeight;

                if (!Physics.Raycast(sideRaycastStart, Vector3.left, CellSize, terrainLayerMask))
                    cell.LeftExit = true;
                else Debug.Log("hit");

                if (!Physics.Raycast(sideRaycastStart, Vector3.right, CellSize, terrainLayerMask))
                    cell.RightExit = true;
                else Debug.Log("hit");

                if (!Physics.Raycast(sideRaycastStart, Vector3.forward, CellSize, terrainLayerMask))
                    cell.UpExit = true;
                else Debug.Log("hit");

                if (!Physics.Raycast(sideRaycastStart, Vector3.back, CellSize, terrainLayerMask))
                    cell.DownExit = true;
                else Debug.Log("hit");

                grid[x, z] = cell;
            }

        markers = new List<GameObject>();
        Debug.Log("grid initialized");
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (markers.Count == 0)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                    for (int z = 0; z < grid.GetLength(1); z++)
                    {
                        if (grid[x, z].Passability > 0)
                        {
                            markers.Add((GameObject)Instantiate(Marker, GetWorldPosition(new GridPosition(x,z)), Quaternion.identity));
                        }
                    }
            }
            else
            {
                for (int i = 0; i < markers.Count; i++)
                    Destroy(markers[i]);

                markers.Clear();
            }
        }
    }

    public static BuildingGrid GetGrid()
    {
        return GameObject.Find("Grid").GetComponent<BuildingGrid>();
    }

    public float GetRaycastHeight()
    {
        return neighbourCheckHeight;
    }

    public bool IsInGridBounds(Vector3 position)
    {
        return position.x >= 0 && position.x < XCount * CellSize && position.z >= 0 && position.z < ZCount * CellSize;
    }

    /// <summary>
    /// Returns the position of the gridcell that contains a given world coordinates.
    /// </summary>
    /// <param name="position">The world coordinates to find grid coordinates for.</param>
    /// <returns>The coordinates of the gridcell that contains position.</returns>
    public GridPosition GetGridPosition(Vector3 position)
    {
        int x = (int)((position.x - (position.x % CellSize)) / CellSize);
        int z = (int)((position.z - (position.z % CellSize)) / CellSize);

        return new GridPosition(x, z);
    }

    /// <summary>
    /// Get the center of a grid cell given the GridPosition of that cell.
    /// </summary>
    /// <param name="position">The GridPosition of the cell.</param>
    /// <returns>The center of the given cell.</returns>
    public Vector2 GetGridCellCenter(GridPosition position)
    {
        float x = (position.X + 0.5f) * CellSize;
        float z = (position.Z + 0.5f) * CellSize;
        return new Vector2(x, z);
    }

    /// <summary>
    /// Find the world coordinates for the given gridcell.
    /// </summary>
    /// <param name="position">The gridcell to find world coordinates for.</param>
    /// <returns>The world coordinates for the given gridcell.</returns>
    public Vector3 GetWorldPosition(GridPosition position)
    {
        Vector3 output = transform.position;
        output.x = position.X * CellSize;
        output.y = transform.position.y;
        output.z = position.Z * CellSize;
        return output;
    }

    /// <summary>
    /// Attempts to occupy all the gridcells in the blueprint, given a hook in the global grid.
    /// Does not occupy any cells if not all to-occupy cells were free.
    /// </summary>
    /// <param name="blueprint">The blueprint that contains the to-occupy cells.</param>
    /// <param name="globalHookPosition">The global hook position in the grid that will be alligned with the local hook of the blueprint.</param>
    /// <returns>whether all cells could be occupied.</returns>
    public bool OccupyCells(Blueprint blueprint, GridPosition globalHookPosition)
    {
        return OccupyCells(blueprint.Cells, blueprint.LocalHook, globalHookPosition, blueprint.Passability);
    }

    /// <summary>
    /// Attempts to occupy all the gridcells in the blueprint, given a hook in the global grid.
    /// Does not occupy any cells if not all to-occupy cells were free.
    /// </summary>
    /// <param name="cells">The blueprint that contains the to-occupy cells.</param>
    /// <param name="localHookPosition">The blueprint that contains the to-occupy cells.</param>
    /// <param name="globalHookPosition">The global hook position in the grid that will be alligned with the local hook of the blueprint.</param>
    /// <param name="passability">The unit types that can pass over this cell.</param>
    /// <returns>whether all cells could be occupied.</returns>
    public bool OccupyCells(bool[,] cells, GridPosition localHookPosition, GridPosition globalHookPosition, GridcellPassability passability)
    {
        if (!ValidPlacement(cells, localHookPosition, globalHookPosition))
            return false;

        for (int x = 0; x < cells.GetLength(0); x++)
            for (int z = 0; z < cells.GetLength(1); z++)
                if (cells[x, z])
                {
                    int globalX = GetGlobalGridPosition(globalHookPosition.X, localHookPosition.X, x);
                    int globalZ = GetGlobalGridPosition(globalHookPosition.Z, localHookPosition.Z, z);

                    grid[globalX, globalZ].Passability = passability;
                }

        return true;
    }

    /// <summary>
    /// Occupy a single cell. Returns if the cell was already occupied.
    /// </summary>
    /// <param name="position">The cell to occupy.</param>
    /// <param name="passability">The unit types that can pass over this cell.</param>
    /// <returns>If the cell was already occupied.</returns>
    public bool OccupyCell(GridPosition position, GridcellPassability passability)
    {
        if (grid[position.X, position.Z].Passability > GridcellPassability.Ground)
            return true;
        else
        {
            grid[position.X, position.Z].Passability = passability;
            return false;
        }
    }

    /// <summary>
    /// Checks if a blueprint is allowed to be placed on a certain global hook position. (so all the cells that need to be occupied are free)
    /// </summary>
    /// <param name="blueprint">The blueprint to check placement for.</param>
    /// <param name="globalHookPosition">The global position in the grid to allign the local hook position with.</param>
    /// <returns>whether the placement is allowed or not.</returns>
    public bool ValidPlacement(Blueprint blueprint, GridPosition globalHookPosition)
    {
        return ValidPlacement(blueprint.Cells, blueprint.LocalHook, globalHookPosition);
    }

    /// <summary>
    /// Checks if a blueprint is allowed to be placed on a certain global hook position. (so all the cells that need to be occupied are free)
    /// </summary>
    /// <param name="cells">The blueprint to check placement for.</param>
    /// <param name="localHookPosition">The blueprint to check placement for.</param>
    /// <param name="globalHookPosition">The global position in the grid to allign the local hook position with.</param>
    /// <returns>whether the placement is allowed or not.</returns>
    public bool ValidPlacement(bool[,] cells, GridPosition localHookPosition, GridPosition globalHookPosition)
    {
        for (int x = 0; x < cells.GetLength(0); x++)
            for (int z = 0; z < cells.GetLength(1); z++)
            {
                if (cells[x, z])
                {
                    int globalX = GetGlobalGridPosition(globalHookPosition.X, localHookPosition.X, x);
                    int globalZ = GetGlobalGridPosition(globalHookPosition.Z, localHookPosition.Z, z);

                    if (globalX < 0 || globalZ < 0 || globalX >= grid.GetLength(0) || globalZ >= grid.GetLength(1))
                        return false;
                    if (grid[globalX, globalZ].Passability > 0)
                        return false;
                }
            }

        return true;
    }

    /// <summary>
    /// Calculate the local coordinate of an index in a block of cells given its globalHook and localHook. Works on 1 dimension, so 1 coordinate (x or z).
    /// </summary>
    /// <param name="globalHookPosition">The global position of the block of cells in the grid.</param>
    /// <param name="localHookPosition">The local position in the block of cells to allign to the globalHook.</param>
    /// <param name="index">The index for the coordinate (x or z, depends on which global/local hook was given).</param>
    /// <returns>The gloal position in the grid.</returns>
    public int GetGlobalGridPosition(int globalHookPosition, int localHookPosition, int index)
    {
        return globalHookPosition + index - localHookPosition;
    }

    /// <summary>
    /// Mark all cells in the block of given cells to be non-occupied.
    /// </summary>
    /// <param name="blueprint">The blueprint to release.</param>
    /// <param name="globalHookPosition">The global position in the grid to align with the local hook.</param>
    public void ClearCells(Blueprint blueprint, GridPosition globalHookPosition)
    {
        ClearCells(blueprint.Cells, blueprint.LocalHook, globalHookPosition);
    }

    /// <summary>
    /// Mark all cells in the block of given cells to be non-occupied.
    /// </summary>
    /// <param name="cells">The blueprint to release.</param>
    /// <param name="localHookPosition">The blueprint to release.</param>
    /// <param name="globalHookPosition">The global position in the grid to align with the local hook.</param>
    public void ClearCells(bool[,] cells, GridPosition localHookPosition, GridPosition globalHookPosition)
    {
        for (int x = 0; x < cells.GetLength(0); x++)
            for (int z = 0; z < cells.GetLength(1); z++)
                if (cells[x, z])
                {
                    int globalX = GetGlobalGridPosition(globalHookPosition.X, localHookPosition.X, x);
                    int globalZ = GetGlobalGridPosition(globalHookPosition.Z, localHookPosition.Z, z);

                    grid[globalX, globalZ].Passability = GridcellPassability.Ground;
                }
    }

    /// <summary>
    /// Release a single cell. Returns if the cell was already occupied.
    /// </summary>
    /// <param name="position">The cell to release.</param>
    public void ClearCell(GridPosition position)
    {
        grid[position.X, position.Z].Passability = GridcellPassability.Ground;
    }

    /// <summary>
    /// Gets the list of non-occupied neighbours of a given GridPosition.
    /// </summary>
    /// <param name="position">The position to obtain neighbours from.</param>
    /// <param name="passability">The passability type of the unit.</param>
    /// <returns>The list of non-occupied neighbours of position.</returns>
    public List<GridPosition> GetEmptyNeighbours(GridPosition position, GridcellPassability passability)
    {
        List<GridPosition> neighbours = new List<GridPosition>(4);

        Gridcell cell = grid[position.X, position.Z];

        // TODO check for symmetry
        if (cell.RightExit)
        {
            if (ValidPosition(position.X + 1, position.Z) && grid[position.X + 1, position.Z].Passability <= passability)
                neighbours.Add(new GridPosition(position.X + 1, position.Z));
        }

        if (cell.LeftExit)
        {
            if (ValidPosition(position.X - 1, position.Z) && grid[position.X - 1, position.Z].Passability <= passability)
                neighbours.Add(new GridPosition(position.X - 1, position.Z));
        }

        if (cell.UpExit)
        {
            if (ValidPosition(position.X, position.Z + 1) && grid[position.X, position.Z + 1].Passability <= passability)
                neighbours.Add(new GridPosition(position.X, position.Z + 1));
        }

        if (cell.DownExit)
        {
            if (ValidPosition(position.X, position.Z - 1) && grid[position.X, position.Z - 1].Passability <= passability)
                neighbours.Add(new GridPosition(position.X, position.Z - 1));
        }

        return neighbours;
    }

    /// <summary>
    /// Check if a grid position is not out of bounds
    /// </summary>
    /// <param name="x">The x grid coordinate</param>
    /// <param name="z">The z grid coordinate</param>
    /// <returns>Whether the position is out of bounds or not</returns>
    public bool ValidPosition(int x, int z)
    {
        return x >= 0 && x < XCount
            && z >= 0 && z < ZCount;
    }
}
