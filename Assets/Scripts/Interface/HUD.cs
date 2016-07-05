using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum MainTab
{
    Normal,
    Utility,
    Infantry,
    Vehicle
}

public class HUD : MonoBehaviour 
{
    public Player ControlledPlayer;
    public Minimap Minimap;
    public PowerBar PowerBar;
    public BuildingAndUnitCollection Collection;

    public float SideBarWidth;

    #region Tab Icons

    public Texture NormalBuildingsIcon;
    public Texture UtilityBuildingsIcon;
    public Texture InfantryIcon;
    public Texture VehicleIcon;

    public Texture SecondaryIcon;

    #endregion

    #region Positional Stuff

    public int IconsPerRow;
    public int SecondaryPerRow;

    private float creditBottom;
    private float minimapBottom;
    private float mainTabsBottom;
    private float secondaryTabsBottom;

    #endregion

    #region Selection Stuff

    public List<Building> ConstructionYards;
    public List<Building> Barracks;
    public List<Building> WarFactories;

    private bool tabsActive;
    private MainTab currentMainTab;
    private int currentSecondaryTab;

    #endregion

    void Start () 
    {
        creditBottom = 0;
        minimapBottom = 0;
        mainTabsBottom = 0;
        secondaryTabsBottom = 0;
        tabsActive = true;
        currentMainTab = MainTab.Normal;
        currentSecondaryTab = 0;

        ConstructionYards = new List<Building>();
        Barracks = new List<Building>();
        WarFactories = new List<Building>();
	}

    void OnGUI()
    {
        DrawCredits();
        DrawMinimap();
        DrawPowerBar();
        DrawMainTabIcons();
        if (tabsActive)
        {
            DrawSecondaryTabIcons();
            DrawTabContents();
        }
        else
            DrawSelection();
    }

    #region Add/Remove buildings

    public void AddConstructionYard(Building constructionYard)
    {
        ConstructionYards.Add(constructionYard);
    }

    public void RemoveConstructionYard(Building constructionYard)
    {
        ConstructionYards.Remove(constructionYard);
    }

    public void AddBarracks(Building barracks)
    {
        Barracks.Add(barracks);
    }

    public void RemoveBarracks(Building barracks)
    {
        Barracks.Remove(barracks);
    }

    public void AddWarFactory(Building warFactory)
    {
        WarFactories.Add(warFactory);
    }

    public void RemoveWarFactory(Building warFactory)
    {
        WarFactories.Remove(warFactory);
    }

    #endregion

    /// <summary>
    /// Draws the current credit balance.
    /// </summary>
    private void DrawCredits()
    {
        Rect rect = new Rect(Screen.width - SideBarWidth + SideBarWidth / 2 - 10, 10, 200, 20);
        GUI.Label(rect, ControlledPlayer.GetCredits().ToString());
        
        creditBottom = 20; // ------------------------------------------------------------^
    }

    /// <summary>
    /// Draws the minimap.
    /// </summary>
    private void DrawMinimap()
    {
        Minimap.Draw(new Rect(Screen.width - SideBarWidth * 0.85f, creditBottom, SideBarWidth * 0.7f, SideBarWidth * 0.7f));
        minimapBottom = creditBottom + SideBarWidth * 0.7f;//---------------------------------------^
    }

    /// <summary>
    /// Draws the power bar.
    /// </summary>
    private void DrawPowerBar()
    {
        float left = Screen.width - SideBarWidth + 10;
        float top = 10;
        Rect rect = new Rect(left, top, 40, 200);
        PowerBar.DrawPowerBar(rect, ControlledPlayer);
    }

    #region Main Tab Drawing/Selection

    /// <summary>
    /// Draws the icons for the four main tabs.
    /// </summary>
    private void DrawMainTabIcons()
    {
        float iconWidth = SideBarWidth * 0.9f / 4f;

        Rect rectangle = new Rect(Screen.width - SideBarWidth * 0.9f, minimapBottom, iconWidth, iconWidth);
        if (GUI.Button(rectangle, NormalBuildingsIcon))
            SelectMainTab(MainTab.Normal);

        rectangle.x += iconWidth;
        if (GUI.Button(rectangle, UtilityBuildingsIcon))
            SelectMainTab(MainTab.Utility);

        rectangle.x += iconWidth;
        if (GUI.Button(rectangle, InfantryIcon))
            SelectMainTab(MainTab.Infantry);

        rectangle.x += iconWidth;
        if (GUI.Button(rectangle, VehicleIcon))
            SelectMainTab(MainTab.Vehicle);

        mainTabsBottom = minimapBottom + iconWidth;
    }

    private void SelectMainTab(MainTab selectedTab)
    {
        if (selectedTab != currentMainTab)
        {
            tabsActive = true;
            currentMainTab = selectedTab;
            ResetSecondaryTab();
        }
        else
            tabsActive = !tabsActive;
    }

    private void ResetSecondaryTab()
    {
        currentSecondaryTab = 0;
    }

    #endregion

    #region Secondary Tabs

    /// <summary>
    /// Draws the secondary tabs (multiple buildings that produce the same main tab) icons.
    /// </summary>
    private void DrawSecondaryTabIcons()
    {
        switch (currentMainTab)
        {
            case MainTab.Normal:
                if (ConstructionYards.Count > 0)
                    currentSecondaryTab = DrawSecondaryTabIcons_Aux(ConstructionYards.Count);
                else
                    secondaryTabsBottom = mainTabsBottom;
                break;
            case MainTab.Utility:
                if (ConstructionYards.Count > 0)
                    currentSecondaryTab = DrawSecondaryTabIcons_Aux(ConstructionYards.Count);
                else
                    secondaryTabsBottom = mainTabsBottom;
                break;
            case MainTab.Infantry:
                if (Barracks.Count > 0)
                    currentSecondaryTab = DrawSecondaryTabIcons_Aux(Barracks.Count);
                else
                    secondaryTabsBottom = mainTabsBottom;
                break;
            case MainTab.Vehicle:
                if (WarFactories.Count > 0)
                    currentSecondaryTab = DrawSecondaryTabIcons_Aux(WarFactories.Count);
                else
                    secondaryTabsBottom = mainTabsBottom;
                break;
        }
    }

    private int DrawSecondaryTabIcons_Aux(int amount)
    {
        float iconWidth = (SideBarWidth * 0.9f) / SecondaryPerRow;
        float left = Screen.width - SideBarWidth * 0.9f;
        float top = mainTabsBottom + 10;

        int selection = currentSecondaryTab;

        for (int i = 0; i < amount; i++)
        {
            int column = i % SecondaryPerRow;
            int row = (i - column) / SecondaryPerRow;
            float leftOffset = column * iconWidth;
            float topOffset = row * iconWidth;

            Rect rectangle = new Rect(left + leftOffset, top + topOffset, iconWidth, iconWidth);

            if (GUI.Button(rectangle, SecondaryIcon))
            {
                selection = i;
            }
        }

        secondaryTabsBottom = top + Mathf.Ceil((float)amount / (float)SecondaryPerRow) * iconWidth;

        return selection;
    }

    #endregion

    /// <summary>
    /// Draws the contents of the current active tab is any.
    /// </summary>
    private void DrawTabContents()
    {
        switch (currentMainTab)
        {
            case MainTab.Normal:
                DrawTabContents_Aux(BuildingsAndUnitsTabs.NormalBuildingsTab, ConstructionYards, ExtractBuildingIcon);
                break;
            case MainTab.Utility:
                DrawTabContents_Aux(BuildingsAndUnitsTabs.UtilityBuildingsTab, ConstructionYards, ExtractBuildingIcon);
                break;
            case MainTab.Infantry:
                DrawTabContents_Aux(BuildingsAndUnitsTabs.InfantryTab, Barracks, ExtractUnitIcon);
                break;
            case MainTab.Vehicle:
                DrawTabContents_Aux(BuildingsAndUnitsTabs.VehiclesTab, WarFactories, ExtractUnitIcon);
                break;
        }
    }

    private delegate Texture IconExtractor(BuildingsAndUnitsEnum enumEntry);

    /// <summary>
    /// Draw the icons of a set of units/buildings from a main tab.
    /// </summary>
    /// <param name="tabContents">The units/buildings available in the tab.</param>
    /// <param name="iconExtractor">The function to extract the icon for each building/unit.</param>
    private void DrawTabContents_Aux(BuildingsAndUnitsEnum[] tabContents, List<Building> constructors, IconExtractor iconExtractor)
    {
        // Get some measurements for later.
        float iconWidth = (SideBarWidth * 0.9f) / IconsPerRow;
        float left = Screen.width - SideBarWidth * 0.9f;
        float top = secondaryTabsBottom + 10;

        // Iterate over every buildable thing in the tab.
        for (int i = 0; i < tabContents.Length; i++)
        {
            // Figure out some more measurements.
            int column = i % IconsPerRow;
            int row = (i - column) / IconsPerRow;
            float leftOffset = column * iconWidth;
            float topOffset = row * iconWidth;

            // Find out what building or unit we are drawing.
            BuildingsAndUnitsEnum enumEntry = tabContents[i];

            GUIContent button;
            Building currentConstructor = null;
            // If we can construct the building or unit, make a button that can callback to start construction, or shows the current construction situation.
            bool buildable = constructors.Count > 0 && ControlledPlayer.HasRights(enumEntry);
            bool isTheCurrentlyConstructingItem = false;
            if (buildable)
            {
                currentConstructor = constructors[currentSecondaryTab];
                if (currentConstructor.IsConstructing() || currentConstructor.BuildingReady())
                {
                    if (enumEntry == currentConstructor.GetCurrentConstructingType())
                    {
                        string progress = currentConstructor.GetConstructionProgress().ToString();
                        button = new GUIContent(progress, iconExtractor(enumEntry));
                        isTheCurrentlyConstructingItem = true;
                    }
                    else
                        button = new GUIContent("X", iconExtractor(enumEntry));
                }
                else
                    button = new GUIContent(iconExtractor(enumEntry));
            }
            else
            // Otherwise, just draw the greyed out icon
                button = new GUIContent("N/A", iconExtractor(enumEntry));

            Rect rectangle = new Rect(left + leftOffset, top + topOffset, iconWidth, iconWidth);
            if (GUI.Button(rectangle, button))
            {
                #region Left Click
                if (Event.current.button == 0)
                {
                    if (buildable)
                    {
                        if ((int)enumEntry < (int)BuildingsAndUnitsEnum.BuildingCount)
                        {
                            // Building
                            if (!currentConstructor.IsConstructing())
                            {
                                if (currentConstructor.BuildingReady())
                                {
                                    // Start building placement.
                                    ControlledPlayer.ModeController.SetMode(Mode.BuildingPlacement);
                                    ControlledPlayer.ModeController.BuildingPlacer.StartPlacement(currentConstructor.GetCurrentConstructingBuilding(), currentConstructor);
                                }
                                else
                                {
                                    // Start new construction
                                    currentConstructor.StartConstruction(Collection.GetBuilding(enumEntry));
                                }
                            }
                        }
                        else
                        {
                            // Unit
                            if (!currentConstructor.IsConstructing())
                            {
                                Unit u = Collection.GetUnit(enumEntry);
                                currentConstructor.StartConstruction(u);
                            }
                        }
                    }
                }
                #endregion

                #region Right Click
                if (Event.current.button == 1 && isTheCurrentlyConstructingItem)
                {
                    currentConstructor.CancelConstruction();
                }
                #endregion
            }
        }
    }

    /// <summary>
    /// Draw the currently selected units and buildings on the sidebar.
    /// </summary>
    private void DrawSelection()
    {
        float iconWidth = (SideBarWidth * 0.9f) / IconsPerRow;
        float left = Screen.width - SideBarWidth * 0.9f;
        float top = mainTabsBottom + 10;

        List<SelectableWithHealth> selection = ControlledPlayer.ModeController.SelectionManager.GetSelectedObjects();

        for (int i = 0; i < selection.Count; i++)
        {
            int column = i % IconsPerRow;
            int row = (i - column) / IconsPerRow;
            float leftOffset = column * iconWidth;
            float topOffset = row * iconWidth;

            BuildingsAndUnitsEnum enumEntry = selection[i].Type;

            Texture icon;
            if ((int)enumEntry < (int)BuildingsAndUnitsEnum.BuildingCount)
                icon = ExtractBuildingIcon(enumEntry);
            else
                icon = ExtractUnitIcon(enumEntry);

            Rect rectangle = new Rect(left + leftOffset, top + topOffset, iconWidth, iconWidth);
            GUI.Button(rectangle, new GUIContent(icon));
         
        }
    }

    private Texture ExtractBuildingIcon(BuildingsAndUnitsEnum enumEntry)
    {
        return Collection.GetBuilding(enumEntry).Icon;
    }

    private Texture ExtractUnitIcon(BuildingsAndUnitsEnum enumEntry)
    {
        return Collection.GetUnit(enumEntry).Icon;
    }
}
