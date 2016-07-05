using UnityEngine;
using System.Collections;

/// <summary>
/// Blinks the alpha channel of the color of the material of a given Meshrender.
/// </summary>
public class BlinkPreview : MonoBehaviour 
{
    public MeshRenderer[] Renderers;
    public float BlinkSpeed;
    public float MinOpacity;
    public float MaxOpacity;

    // The current time in a single half-blink.
    private float currentTime;

    // Whether we are increasing or decreasing in opacity.
    private int modifier;

	public void Start () 
    {
        currentTime = 0;
        modifier = 1;
	}

    void OnDisable()
    {
        // Reset the color if the blinker is being disabled (so the building is placed).
        for (int i = 0; i < Renderers.Length; i++)
        {
            Color currentColor = Renderers[i].material.color;
            currentColor.a = 1;
            Renderers[i].material.color = currentColor;
        }
    }
	
	void Update () 
    {
        // Increase or decrease opacity.
        currentTime += modifier * Time.deltaTime;

        // Check if we should go back.
        if (currentTime < 0 || currentTime > BlinkSpeed)
        {
            currentTime = Mathf.Clamp(currentTime, 0, BlinkSpeed);
            modifier *= -1;
        }

        // Recalculate opacity every time to avoid rounding errors.
        float alpha =  MinOpacity + (MaxOpacity - MinOpacity) * (currentTime / BlinkSpeed);

        for (int i = 0; i < Renderers.Length; i++)
        {
            // Set the color.
            Color currentColor = Renderers[i].material.color;
            currentColor.a = alpha;
            Renderers[i].material.color = currentColor;
        }
	}
}
