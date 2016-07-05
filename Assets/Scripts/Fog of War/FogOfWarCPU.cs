using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogOfWarCPU : MonoBehaviour 
{
    public int Width;
    public int Depth;
    public float FogHeight;
    public float CellSize;
    public byte Increment;
    public MeshRenderer Renderer;
    public MeshFilter Filter;
    public bool DrawMap;

    private Dictionary<GridPosition, GameObject> buildingsInFog;

    private GUIStyle fogMapStyle;

    private Texture2D fogMapImage;
    private byte[] fogMap;
    private Dictionary<float, FogOfWarRange> ranges;

	void Awake () 
	{
        fogMapStyle = new GUIStyle();
       
        ranges = new Dictionary<float, FogOfWarRange>(4);
        buildingsInFog = new Dictionary<GridPosition,GameObject>();
        Reset();
	}

    private void Reset()
    {
        fogMapImage = new Texture2D(Width, Depth, TextureFormat.RGB24, false);
        fogMapStyle.normal.background = fogMapImage;
        fogMap = new byte[Width * Depth * 3];
        
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Depth; y++)
            {
                int bytePos = GetByteArrayPosition(x, y);
                fogMap[bytePos] = 0;
                fogMap[bytePos + 1] = 0;
                fogMap[bytePos + 2] = 0;
                //fogMap[bytePos + 3] = 1;
                fogMapImage.SetPixel(x, y, new Color(0, 0, 0));
            }

        fogMapImage.Apply();

        Filter.mesh = new Mesh();
        Filter.mesh.vertices = new Vector3[] { new Vector3(0, FogHeight, 0), 
                                               new Vector3(Width * CellSize, FogHeight, 0), 
                                               new Vector3(Width * CellSize, FogHeight, Depth * CellSize), 
                                               new Vector3(0, FogHeight, Depth * CellSize) 
                                             };
        Filter.mesh.uv = new Vector2[]{ new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
        Filter.mesh.normals = new Vector3[] { new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0) };
        Filter.mesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };

        fogMapImage.wrapMode = TextureWrapMode.Clamp;
        Renderer.material.SetTexture("_FogMapImage", fogMapImage);
    }

    void OnGUI()
    {
        if (!DrawMap)
            return;
        //GUIContent map = new GUIContent(fogMapImage);
        GUI.Label(new Rect(20, 20, Width * 4, Depth * 4), "", fogMapStyle);
    }

    public void AddBuildingToFog(Vector3 buildingPosition, GameObject model)
    {
        GridPosition position = GetGridPosition(buildingPosition);
        buildingsInFog.Add(position, model);
    }

    public bool IsInFog(Vector3 position)
    {
        GridPosition positionGrid = GetGridPosition(position);
        int bytePos = GetByteArrayPosition(positionGrid.X, positionGrid.Z);
        return fogMap[bytePos] == 0;
    }

    private GridPosition GetGridPosition(Vector3 position)
    {
        return new GridPosition(Mathf.FloorToInt(position.x / CellSize), Mathf.FloorToInt(position.z / CellSize));
    }

    void Update()
    {
        //Debug.Log(test.bytes.Length);
        fogMapImage.LoadRawTextureData(fogMap);
        fogMapImage.Apply();
    }


    public void Uncover(Vector3 position, float range)
    {
        // Find the center to hook on
        GridPosition positionGrid = new GridPosition(Mathf.FloorToInt(position.x / CellSize), Mathf.FloorToInt(position.z / CellSize));
        
        // Find the range grid for this range
        FogOfWarRange rangeStruct = GenerateRange(range);
        bool[,] grid = rangeStruct.Grid;

        // TODO possible optimisation
        for (int x = 0; x < rangeStruct.Width; x++)
            for (int y = 0; y < rangeStruct.Height; y++)
            {
                if (grid[x, y])
                {
                    GridPosition finalPos = positionGrid + new GridPosition(x,y) - rangeStruct.Center;

                    // TODO optimisation
                    if (ValidPosition(finalPos.X, finalPos.Z))
                        UncoverPixel(finalPos.X, finalPos.Z);
                }
            }
    }

    public void CoverUp(Vector3 position, float range)
    {
        // Find the center to hook on
        GridPosition positionGrid = new GridPosition(Mathf.FloorToInt(position.x / CellSize), Mathf.FloorToInt(position.z / CellSize));

        // Find the range grid for this range
        FogOfWarRange rangeStruct = GenerateRange(range);
        bool[,] grid = rangeStruct.Grid;

        // TODO possible optimisation
        for (int x = 0; x < rangeStruct.Width; x++)
            for (int y = 0; y < rangeStruct.Height; y++)
            {
                if (grid[x, y])
                {
                    GridPosition finalPos = positionGrid + new GridPosition(x, y) - rangeStruct.Center;

                    // TODO optimisation
                    if (ValidPosition(finalPos.X, finalPos.Z))
                        CoverUpPixel(finalPos.X, finalPos.Z);
                }
            }
    }

    private void UncoverPixel(int x, int y)
    {
        // Work back from B to G to R
        int bytePos = GetByteArrayPosition(x, y);

        if (fogMap[bytePos] <= 255 - Increment)
            fogMap[bytePos] += Increment;
        else if (fogMap[bytePos + 1] <= 255 - Increment)
            fogMap[bytePos + 1] += Increment;
        else if (fogMap[bytePos + 2] <= 255 - Increment)
            fogMap[bytePos + 2] += Increment;
        else Debug.Log("Fog of war would go into overflow, something is wrong (More than 765 units are seeing the same spot!?)");

        GridPosition pos = new GridPosition(x, y);
        if (buildingsInFog.ContainsKey(pos))
        {
            Destroy(buildingsInFog[pos]);
            buildingsInFog.Remove(pos);
        }
        //fogMap.SetPixel(x, y, currentColor);
    }

    private void CoverUpPixel(int x, int y)
    {
        // Work back from B to G to R
        int bytePos = GetByteArrayPosition(x, y);

        if (fogMap[bytePos + 2] >= Increment)
            fogMap[bytePos + 2] -= Increment;
        else if (fogMap[bytePos + 1] >= Increment)
            fogMap[bytePos + 1] -= Increment;
        else if (fogMap[bytePos] >= Increment)
            fogMap[bytePos] -= Increment;
        //else Debug.Log("Fog of war would go into negative value, something is wrong!");

        //fogMap.SetPixel(x, y, currentColor);
    }

    private FogOfWarRange GenerateRange(float range)
    {
        if (ranges.ContainsKey(range))
            return ranges[range];
        else
        {
            int size = Mathf.CeilToInt(range / CellSize) * 2 + 1;
            int center = Mathf.CeilToInt(range / CellSize) + 1;
            float scaledRange = range / CellSize;

            bool[,] grid = new bool[size, size];

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    // Obtain pixel center
                    float xFloat = x + 0.5f;
                    float yFloat = y + 0.5f;

                    // Calculate distance from center
                    float distance = Mathf.Sqrt((xFloat - center) * (xFloat - center) + (yFloat - center) * (yFloat - center));
                    if (distance <= scaledRange)
                    {
                        grid[x, y] = true;
                    }
                    else
                        grid[x, y] = false;
                }

            FogOfWarRange rangeStruct = new FogOfWarRange(grid, new GridPosition(center, center), size, size);
            ranges.Add(range, rangeStruct);
            return rangeStruct;
        }
    }

    private int GetByteArrayPosition(int x, int y)
    {
        return y * 3 * Width + x * 3;
    }

    /// <summary>
    /// Check if a grid position is not out of bounds
    /// </summary>
    /// <param name="x">The x grid coordinate</param>
    /// <param name="z">The z grid coordinate</param>
    /// <returns>Whether the position is out of bounds or not</returns>
    private bool ValidPosition(int x, int z)
    {
        return x >= 0 && x < Width
            && z >= 0 && z < Depth;
    }
}
