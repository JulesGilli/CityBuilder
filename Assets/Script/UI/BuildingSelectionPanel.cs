// BuildingSelectionPanel.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class BuildingSelectionPanel : MonoBehaviour
{
    public static BuildingSelectionPanel Instance { get; private set; }

    [Header("UI References")]
    public RectTransform contentParent;
    public GameObject buildingIconButtonPrefab;
    public GameObject roadIconButtonPrefab;

    [Header("Default Unlocks")]
    public List<BuildingData> initialBuildings;
    public List<RoadData> initialRoads;

    private List<BuildingData> unlockedBuildings = new List<BuildingData>();
    private List<RoadData> unlockedRoads = new List<RoadData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Initialize panel with default unlocked items
        Initialize(initialBuildings, initialRoads);
    }

    /// <summary>
    /// Clears and repopulates panel with given unlocked buildings and roads.
    /// </summary>
    public void Initialize(List<BuildingData> buildings, List<RoadData> roads)
    {
        Clear();
        if (buildings != null) unlockedBuildings.AddRange(buildings);
        if (roads != null) unlockedRoads.AddRange(roads);

        foreach (var data in unlockedBuildings)
            AddBuildingIcon(data);
        foreach (var data in unlockedRoads)
            AddRoadIcon(data);
    }

    /// <summary>
    /// Removes all icons and clears unlocked lists.
    /// </summary>
    public void Clear()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
        unlockedBuildings.Clear();
        unlockedRoads.Clear();
    }

    /// <summary>
    /// Returns a copy of the unlocked buildings.
    /// </summary>
    public List<BuildingData> GetUnlockedBuildings()
    {
        return unlockedBuildings.ToList();
    }

    /// <summary>
    /// Returns a copy of the unlocked roads.
    /// </summary>
    public List<RoadData> GetUnlockedRoads()
    {
        return unlockedRoads.ToList();
    }

    /// <summary>
    /// Checks if the given building data is unlocked.
    /// </summary>
    public bool IsBuildingUnlocked(BuildingData data)
    {
        return unlockedBuildings.Contains(data);
    }

    /// <summary>
    /// Checks if the given road data is unlocked.
    /// </summary>
    public bool IsRoadUnlocked(RoadData data)
    {
        return unlockedRoads.Contains(data);
    }

    /// <summary>
    /// Adds a new building to unlocked list and panel.
    /// </summary>
    public void UnlockBuilding(BuildingData data)
    {
        if (data == null || unlockedBuildings.Contains(data)) return;
        unlockedBuildings.Add(data);
        AddBuildingIcon(data);
    }

    /// <summary>
    /// Adds a new road to unlocked list and panel.
    /// </summary>
    public void UnlockRoad(RoadData data)
    {
        if (data == null || unlockedRoads.Contains(data)) return;
        unlockedRoads.Add(data);
        AddRoadIcon(data);
    }

    private void AddBuildingIcon(BuildingData data)
    {
        var go = Instantiate(buildingIconButtonPrefab, contentParent);
        go.GetComponent<BuildingIconButton>().Initialize(data);
    }

    private void AddRoadIcon(RoadData data)
    {
        var go = Instantiate(roadIconButtonPrefab, contentParent);
        go.GetComponent<RoadIconButton>().Initialize(data);
    }
}
