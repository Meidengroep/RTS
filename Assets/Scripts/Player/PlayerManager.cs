using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages all the players in-game.
/// </summary>
public class PlayerManager : MonoBehaviour 
{
    public Player[] Players;
    public int CurrentPlayer;
    public KeyCode SwapKey;

    public Camera ActiveCamera;

    public Texture RedPixel;
    public Texture GreenPixel;

    public float HealthbarHeight;
    public float HealthbarWidth;

	void Start () 
    {
        CurrentPlayer = 0;
        for (int i = 1; i < Players.Length; i++)
            Players[i].gameObject.SetActive(false);
    }

    void OnGUI()
    {
        for (int p = 0; p < Players.Length; p++)
        {
            List<Unit> units = Players[p].GetUnits();
            for (int u = 0; u < units.Count; u++)
            {
                DrawUnitHealthbar(units[u]);
            }

            List<Building> buildings = Players[p].GetBuildings();
            for (int b = 0; b < buildings.Count; b++)
            {
                DrawBuildingHealthbar(buildings[b]);
            }
        }
    }

    /// <summary>
    /// Draw the healthbar for a given unit.
    /// </summary>
    /// <param name="unit">The unit to draw the healthbar for.</param>
    private void DrawUnitHealthbar(Unit unit)
    {
        if (!unit.IsVisible())
            return;
        Vector3 healthbarWorldPosition = unit.HealthbarPosition.position;

        Vector2 healthbarScreenPosition = ActiveCamera.WorldToScreenPoint(healthbarWorldPosition);
        healthbarScreenPosition.y = Screen.height - healthbarScreenPosition.y;

        DrawHealthbar(unit.GetHealth(), unit.MaxHealth, healthbarScreenPosition);
    }

    /// <summary>
    /// Draw the healthbar for a given building.
    /// </summary>
    /// <param name="unit">The building to draw the healthbar for.</param>
    private void DrawBuildingHealthbar(Building building)
    {
        if (!building.IsVisible())
            return;
        Vector3 healthbarWorldPosition = building.HealthbarPosition.position;

        Vector2 healthbarScreenPosition = ActiveCamera.WorldToScreenPoint(healthbarWorldPosition);
        healthbarScreenPosition.y = Screen.height - healthbarScreenPosition.y;

        DrawHealthbar(building.GetHealth(), building.MaxHealth, healthbarScreenPosition);
    }

    /// <summary>
    /// Draw any healthbar.
    /// </summary>
    /// <param name="currentHealth">The current health of the bar.</param>
    /// <param name="maxHealth">The maximum health of the bar.</param>
    /// <param name="barCenter">The center position of the bar.</param>
    private void DrawHealthbar(float currentHealth, float maxHealth, Vector2 barCenter)
    {
        Rect bar = new Rect(barCenter.x - HealthbarWidth / 2, barCenter.y - HealthbarHeight / 2, HealthbarWidth, HealthbarHeight);

        GUI.DrawTexture(bar, RedPixel);

        bar.width = HealthbarWidth * currentHealth / maxHealth;

        GUI.DrawTexture(bar, GreenPixel);
    }

    void Update() 
    {
        for (int p = 0; p < Players.Length; p++)
        {
            Players[p].BringOutYourDead();
        }

        if (Input.GetKeyDown(SwapKey))
        {
            CurrentPlayer++;
            CurrentPlayer = CurrentPlayer % Players.Length;

            ChangePlayer(CurrentPlayer);
        }
	}
    
    /// <summary>
    /// Change the currently controlled player.
    /// </summary>
    /// <param name="index">The index of the player to control now.</param>
    private void ChangePlayer(int index)
    {
        for (int p = 0; p < Players.Length; p++)
            if (CurrentPlayer == p)
            {
                Players[p].gameObject.SetActive(true);
            }
            else
                Players[p].gameObject.SetActive(false);
    }
}
