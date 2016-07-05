using UnityEngine;
using System.Collections;

/// <summary>
/// Defines a mouse mode.
/// </summary>
public enum Mode
{
    Selection = 0,
    BuildingPlacement = 1,
    None = 2
}

/// <summary>
/// Allows for switching between different mouse modes (e.g. selection, building placement, etc.)
/// </summary>
public class ModeController : MonoBehaviour 
{
    public SelectionManager SelectionManager;
    public BuildingPlacer BuildingPlacer;
    public Mode InitialMode;
    public Player ControllingPlayer;

    private Mode currentMode;

	void Start () 
    {
        currentMode = InitialMode;

        SetModeInternal(currentMode, true);
	}

    /// <summary>
    /// Returns the current mode.
    /// </summary>
    /// <returns>The current mode.</returns>
    public Mode GetCurrentMode()
    {
        return currentMode;
    }

    /// <summary>
    /// Set the current mode.
    /// </summary>
    /// <param name="mode">The new mode.</param>
    public void SetMode(Mode mode)
    {
        SetModeInternal(mode, false);
    }

    /// <summary>
    /// Set the current mode.
    /// </summary>
    /// <param name="mode">The new mode.</param>
    /// <param name="first">Whether this was the first time to set any mode.</param>
    private void SetModeInternal(Mode mode, bool first)
    {
        if (mode == currentMode && !first)
            return;

        DisableBuildingPlacer();
        DisableSelectionManager();

        switch (mode)
        {
            case Mode.Selection:
                EnableSelectionManager();
                break;
            case Mode.BuildingPlacement:
                EnableBuildingPlacer();
                break;
            case Mode.None:
                break;
        }

        currentMode = mode;
    }

    void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetMode(Mode.BuildingPlacement);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetMode(Mode.Selection);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SetMode(Mode.None);
        }

	}

    /// <summary>
    /// Handling the enabling of disabling of methods.
    /// </summary>
    #region Enable/Disable methods
    private void EnableBuildingPlacer()
    {
        //BuildingPlacer.StartPlacement(BuildingPlacer.CurrentBuildingPrefab);
        BuildingPlacer.enabled = true;
    }

    private void DisableBuildingPlacer()
    {
        BuildingPlacer.CancelPlacement();
        BuildingPlacer.enabled = false;
    }

    private void EnableSelectionManager()
    {
        SelectionManager.enabled = true;
    }

    private void DisableSelectionManager()
    {
        SelectionManager.enabled = false;
    }
    #endregion
}
