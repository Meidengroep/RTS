using UnityEngine;
using System.Collections;

public class PowerBar : MonoBehaviour
{
    public Texture Background;
    public Texture Bar;

    public Color SufficientPowerColor;
    public Color InsufficientPowerColor;

    /// <summary>
    /// Draws the power bar, with the position being the top left of the bar.
    /// </summary>
    /// <param name="position">The top left of the bar.</param>
    /// <param name="player">The player whose power to draw.</param>
    public void DrawPowerBar(Rect rect, Player player)
    {
        GUIContent backgroundContent = new GUIContent(Background);
        GUI.Label(rect, backgroundContent);

        Color oldColor = GUI.color;
        if (player.LowPower())
            GUI.color = InsufficientPowerColor;
        else GUI.color = SufficientPowerColor;

        GUIContent barContent = new GUIContent(Bar);
        GUI.Label(rect, barContent);

        GUI.color = oldColor;
    }
}
