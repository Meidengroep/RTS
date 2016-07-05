using UnityEngine;
using System.Collections;

/// <summary>
/// Handles placement of buildings by showing a preview, checking for valid placement and eventual placing the building.
/// </summary>
public class BuildingPlacer : MonoBehaviour
{
    public SoftwareMouse Mouse;
    public Camera ActiveCamera;
    public BuildingGrid Grid;
    public GameObject Marker;
    public Player ControllingPlayer;

    private Building currentConstructor;
    private GridPosition currentGridPosition;
    private GameObject currentBuildingInstance;
    private Building currentBuildingInstanceClass;
    private Blueprint currentBlueprint;
    private int buildingLayerMask;

	void Start () 
    {
	    currentGridPosition = new GridPosition(0,0);
        buildingLayerMask = LayerMask.GetMask(Layers.TerrainLayerName);
	}

    /// <summary>
    /// Set up a temporary instance of "building" to allow for positioning and eventually placement of the building.
    /// </summary>
    /// <param name="building">The building prefab to place.</param>
    public void StartPlacement(Building building, Building constructor)
    {
        currentBuildingInstance = (GameObject)Instantiate(building.gameObject, Grid.GetWorldPosition(currentGridPosition), building.gameObject.transform.rotation);
        currentBuildingInstanceClass = currentBuildingInstance.GetComponent<Building>();

        currentBuildingInstanceClass.enabled = false;
        currentBuildingInstance.GetComponent<Collider>().enabled = false;
        currentBuildingInstanceClass.Blink.enabled = false;
        currentBlueprint = Blueprint.EnumToBlueprint(currentBuildingInstanceClass.BlueprintPreset);

        this.currentConstructor = constructor;

        Marker.SetActive(true);
    }

    /// <summary>
    /// Cancel the placement of the current building, if placement was started at all.
    /// </summary>
    public void CancelPlacement()
    {
        if (currentBuildingInstance != null)
        {
            Destroy(currentBuildingInstance);
            Marker.SetActive(false);
        }
    }

    /// <summary>
    /// Place the currently active temporary building.
    /// </summary>
    private void PlaceCurrentBuilding()
    {
        if (Grid.OccupyCells(currentBlueprint, currentGridPosition))
        {
            currentBuildingInstance.GetComponent<Collider>().enabled = true;
            ControllingPlayer.AddBuilding(currentBuildingInstanceClass);
            currentBuildingInstanceClass.enabled = true;
            currentBuildingInstanceClass.Blink.enabled = false;

            currentBuildingInstance = null;
            currentBuildingInstanceClass = null;

            currentConstructor.BuildingPlaced();
            ControllingPlayer.ModeController.SetMode(Mode.Selection);
        }
        
    }
	
    void Update () 
    {
        Ray ray = ActiveCamera.ScreenPointToRay(Mouse.ScreenPositionForRaycast);

        RaycastHit info;
        if (Physics.Raycast(ray, out info, Mathf.Infinity, buildingLayerMask))
        {
            currentGridPosition = Grid.GetGridPosition(info.point);

            Marker.transform.position = Grid.GetWorldPosition(currentGridPosition);
        }

        // Shift the grid given the current blueprint.
        GridPosition shiftedPosition = new GridPosition(Grid.GetGlobalGridPosition(currentGridPosition.X, currentBlueprint.LocalHook.X, 0),
                                                        Grid.GetGlobalGridPosition(currentGridPosition.Z, currentBlueprint.LocalHook.Z, 0));

        currentBuildingInstance.transform.position = Grid.GetWorldPosition(shiftedPosition);
        currentBuildingInstanceClass.Blink.enabled = !Grid.ValidPlacement(currentBlueprint, currentGridPosition);

        if (Input.GetMouseButtonDown(0))
        {
            PlaceCurrentBuilding();
        }
	}
}
