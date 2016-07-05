using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the player's camera.
/// </summary>
public class CameraControl : MonoBehaviour 
{
    public SoftwareMouse Mouse;
    public float ScrollSpeed;
    public float ScrollMargin;

    public float ZoomSpeed;
    public float MinZoom;
    public float MaxZoom;

    public Transform CameraTranslation;
    public Transform CameraRotation;

    private float accumulatedZoom;

    void Start()
    {
        accumulatedZoom = 0;
    }
	
	void Update () 
    {
        // Scroll the camera around if the mouse is near the edge of the screen.
        if (Mouse.ScreenPosition.x <= ScrollMargin)
        {
            CameraTranslation.transform.Translate(-ScrollSpeed * Time.deltaTime, 0, 0);
        }

        if (Mouse.ScreenPosition.x >= Screen.width - ScrollMargin - 1)
        {
            CameraTranslation.transform.Translate(ScrollSpeed * Time.deltaTime, 0, 0);
        }

        if (Mouse.ScreenPosition.y <= ScrollMargin)
        {
            CameraTranslation.transform.Translate(0, 0, ScrollSpeed * Time.deltaTime);
        }
        if (Mouse.ScreenPosition.y >= Screen.height - ScrollMargin - 1)
        {
            CameraTranslation.transform.Translate(0, 0, -ScrollSpeed * Time.deltaTime);
        }
        // End scroll

        // Zoom in/out using scrollwheel.
        float prevAccum = accumulatedZoom;

        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        accumulatedZoom += scrollWheel;

        accumulatedZoom = Mathf.Clamp(accumulatedZoom, MinZoom, MaxZoom);

        float zoom;

        if (scrollWheel > 0)
            zoom = accumulatedZoom - prevAccum;
        else zoom = -(accumulatedZoom - prevAccum);

        CameraRotation.transform.Translate(0, 0, zoom * Mathf.Sign(scrollWheel) * ZoomSpeed);
	}
}
