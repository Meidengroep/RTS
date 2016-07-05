using UnityEngine;
using System.Collections;

/// <summary>
/// Contains a simulated mouse cursor, even when the real cursor is being locked to the center of the screen.
/// </summary>
public class SoftwareMouse : MonoBehaviour 
{
    public float Sensitivity;

    /// <summary>
    /// Returns the position of the cursor on the screen.
    /// </summary>
    public Vector2 ScreenPosition
    {
        get { return screenPosition; }
    }

    /// <summary>
    /// Gets the screen position for raycasting (inverted y-coordinate).
    /// </summary>
    public Vector2 ScreenPositionForRaycast { get; private set; }

    public Texture2D Cursor;
    public bool ShowCursor;

    public float SideBarWidth;

    public bool UseCustomBoundary = false;
    public float CustomBoundaryRadius = 0;

    private Vector2 screenPosition;
    private Vector2 screenCenter;

    void Start () 
    {        
        //Screen.lockCursor = true;

        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        screenPosition = screenCenter;
        UpdateRaycastPos();

        CalculateNewPosition();
	}

    /// <summary>
    /// Check if the cursor is on the game screen.
    /// </summary>
    /// <returns>If the cursor is on the game screen.</returns>
    public bool IsOnGameScreen()
    {
        return screenPosition.x >= 0 && screenPosition.x < Screen.width - SideBarWidth && screenPosition.y >= 0 && screenPosition.y <= Screen.height;
    }

    void Update()
    {
        CalculateNewPosition();
    }

    void OnGUI()
    {
        if (ShowCursor)
            GUI.Label(
                new Rect(screenPosition.x - Cursor.width / 2,
                         screenPosition.y - Cursor.height / 2,
                         Cursor.width, 
                         Cursor.height), 
                new GUIContent(Cursor));
    }

    /// <summary>
    /// Calculate the new mouse position.
    /// </summary>
    private void CalculateNewPosition()
    {
        //screenPosition.x += Input.GetAxis("Mouse X") * Time.deltaTime * Sensitivity;
        //screenPosition.y -= Input.GetAxis("Mouse Y") * Time.deltaTime * Sensitivity;

        screenPosition = Input.mousePosition;
        screenPosition.y = Screen.height - screenPosition.y;

        screenPosition.x = Mathf.Clamp(screenPosition.x, 0, Screen.width);
        screenPosition.y = Mathf.Clamp(screenPosition.y, 0, Screen.height);

        // Do we constrain the mouse to a custom circle centered at the center of the screen?
        if (UseCustomBoundary)
        {
            Vector2 differenceWithCenter = screenPosition - screenCenter;
            float maxDifference = CustomBoundaryRadius * Screen.height/  2;

            if (differenceWithCenter.magnitude > maxDifference)
                screenPosition = screenCenter + differenceWithCenter.normalized * maxDifference;
        }

        UpdateRaycastPos();
    }

    /// <summary>
    /// Update the screen position for raycasting (inverts the y-coordinate).
    /// </summary>
    private void UpdateRaycastPos()
    {
        ScreenPositionForRaycast = new Vector2(screenPosition.x, Screen.height - screenPosition.y);
    }
}
