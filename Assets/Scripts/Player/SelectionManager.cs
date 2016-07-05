using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Manages unit and building selection for a player.
/// </summary>
public class SelectionManager : MonoBehaviour 
{
    public SoftwareMouse Mouse;
    public Camera ActiveCamera;
    public Player ControllingPlayer;

    public GUIStyle selectionBoxStyle;

    private List<SelectableWithHealth> selectedObjects;
    private Rect selectionBox;
    private bool isSelecting;

    private RectOffset selectionBoxStyleBorder;

	void Start ()
    {
        selectionBox = new Rect(0, 0, 0, 0);
        isSelecting = false;
        
        selectedObjects = new List<SelectableWithHealth>();

        selectionBoxStyleBorder = new RectOffset(selectionBoxStyle.border.left, selectionBoxStyle.border.right, selectionBoxStyle.border.top, selectionBoxStyle.border.bottom);
	}

    /// <summary>
    /// Gets the currently selected objects.
    /// </summary>
    /// <returns>The currently selected objects.</returns>
    public List<SelectableWithHealth> GetSelectedObjects()
    {
        return selectedObjects;
    }
	
	void Update () 
    {
        // If the left mouse button is being held, check if we need to start or update the selection.
        if (Input.GetMouseButton(0))
        {
            if (isSelecting)
            {
                UpdateSelection();
            }
            else
            {
                if (Mouse.IsOnGameScreen())
                {
                    selectedObjects.Clear();
                    StartSelection();
                }
            }
            ApplySelection();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isSelecting)
            {
                ApplySelection();
                isSelecting = false;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (selectedObjects.Count > 0)
                RightClick();
        }
    }

    void OnGUI()
    {
        // Draw selection box.
        if (isSelecting)
        {
            Rect newRect = new Rect();
            newRect.x = Mathf.Min(selectionBox.x, selectionBox.x + selectionBox.width);
            newRect.y = Mathf.Min(selectionBox.y, selectionBox.y + selectionBox.height);
            newRect.width = Mathf.Abs(selectionBox.width);
            newRect.height = Mathf.Abs(selectionBox.height);

            if (newRect.height / 2 < selectionBoxStyle.border.top)
            {
                // Correct the border to not spill over
                selectionBoxStyle.border.top = (int)(newRect.height / 2);
                selectionBoxStyle.border.bottom = (int)(newRect.height / 2);
            }
            if (newRect.width / 2 < selectionBoxStyle.border.left)
            {
                // Correct the border to not spill over
                selectionBoxStyle.border.left = (int)(newRect.width / 2);
                selectionBoxStyle.border.right = (int)(newRect.width / 2);
            }

            GUI.Label(newRect, "", selectionBoxStyle);

            selectionBoxStyle.border = new RectOffset(selectionBoxStyleBorder.left, selectionBoxStyleBorder.right, selectionBoxStyleBorder.top, selectionBoxStyleBorder.bottom);
        }

        for (int s = 0; s < selectedObjects.Count; s++)
        {
            SelectionMarker mark = selectedObjects[s].GetSelectionMarker();
            Vector3 screenPos = GetProperScreenPoint(mark.WorldPosition);

            if (screenPos.z >= 0)
            {
                GUI.Label(new Rect(screenPos.x - 30, screenPos.y - 10, 60, 20), new GUIContent(mark.MarkerText));
            }
        }

        // GUI label reference comment from dog fighter
        /*GUI.Label(
                new Rect(StatusTexture_Left, StatusTexture_Top, StatusTexture_Width, StatusTexture_Height),
                new GUIContent(HullTexture));

        GUI.color = shieldColor;
        GUI.Label(
                 new Rect(StatusTexture_Left, StatusTexture_Top, StatusTexture_Width, StatusTexture_Height),
                 new GUIContent(ShieldTexture));*/
    }

    /// <summary>
    /// Start a selection (initializes the selection box).
    /// </summary>
    private void StartSelection()
    {
        selectionBox = new Rect(Mouse.ScreenPosition.x, Mouse.ScreenPosition.y, 0, 0);
        isSelecting = true;
    }

    /// <summary>
    /// Update the selectionbox.
    /// </summary>
    private void UpdateSelection()
    {
        selectionBox.width = Mouse.ScreenPosition.x - selectionBox.x;
        selectionBox.height = Mouse.ScreenPosition.y - selectionBox.y;
    }

    /// <summary>
    /// Select the units that are currently inside the selectionbox.
    /// </summary>
    private void ApplySelection()
    {
        selectedObjects.Clear();

        List<Unit> units = ControllingPlayer.GetUnits();
        List<Building> buildings = ControllingPlayer.GetBuildings();

        if (selectionBox.width != 0 || selectionBox.height != 0)
        {
            // Prefer to select units when a box is dragged.
            for (int u = 0; u < units.Count; u++)
            {
                if (IsObjectInSelectionBox(units[u].gameObject))
                    selectedObjects.Add(units[u]);
            }

            if (selectedObjects.Count == 0)
                // If no units, try to select a single building.
                for (int b = 0; b < buildings.Count; b++)
                {
                    if (IsObjectInSelectionBox(buildings[b].gameObject))
                    {
                        selectedObjects.Add(buildings[b]);
                        break;
                    }
                }
        }
        else
            FindNewObjectClick(new Vector3(selectionBox.x, Screen.height - selectionBox.y));
    }

    /// <summary>
    /// Checks if a given object is in the selectionbox.
    /// </summary>
    /// <param name="go">The GameObject to check for.</param>
    /// <returns>Whether the GameObject was in the selection box or not.</returns>
    private bool IsObjectInSelectionBox(GameObject go)
    {
        Vector3 screenPoint = GetProperScreenPoint(go.transform.position);

        if (screenPoint.z >= 0 && selectionBox.Contains(screenPoint, true))
            return true;
        else return false;
    }

    /// <summary>
    /// Cancel the selection.
    /// </summary>
    private void CancelSelection()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Find the object that is on the screenpoint.
    /// </summary>
    /// <param name="screenPoint">The point to shoot the ray through.</param>
    private void FindNewObjectClick(Vector2 screenPoint)
    {
        Ray ray = ActiveCamera.ScreenPointToRay(screenPoint);

        RaycastHit info;
        if (Physics.Raycast(ray, out info))
        {
            SelectableWithHealth clickedObject = info.collider.GetComponent<SelectableWithHealth>();
            if (clickedObject != null)
            {
                selectedObjects.Add(clickedObject);
            }
        }
    }

    /// <summary>
    /// Handles a right-click event.
    /// </summary>
    private void RightClick()
    {
        if (!PlayerControlHelper.SamePlayerControl(selectedObjects[0].gameObject.layer, this.gameObject.layer))
            return;

        Ray ray = ActiveCamera.ScreenPointToRay(Mouse.ScreenPositionForRaycast);

        RaycastHit info;
        if (Physics.Raycast(ray, out info))
        {
            for (int i = 0; i < selectedObjects.Count; i++)
            {
                selectedObjects[i].RightClick(Mouse, info);
            }
        }
    }

    /// <summary>
    /// Gets the screenpoint given a worldpoint, that takes into account that y has to be inverted.
    /// </summary>
    /// <param name="worldPoint">The worldpoint to project.</param>
    /// <returns>The screenpoint given the worldpoint with inverted y-coordinate.</returns>
    private Vector3 GetProperScreenPoint(Vector3 worldPoint)
    {
        Vector3 screenPos = ActiveCamera.WorldToScreenPoint(worldPoint);
        screenPos.y = Screen.height - screenPos.y;
        return screenPos;
    }
}
