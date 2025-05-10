// BuildingSelectionPanel.cs
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class BuildingSelectionPanel : MonoBehaviour
{
    public static BuildingSelectionPanel Instance { get; private set; }
    public RectTransform contentParent;
    public GameObject buildingIconButtonPrefab;
    public GameObject roadIconButtonPrefab;
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
        unlockedBuildings.AddRange(initialBuildings);
        unlockedRoads.AddRange(initialRoads);
        foreach (var data in unlockedBuildings) AddBuildingIcon(data);
        foreach (var data in unlockedRoads) AddRoadIcon(data);
    }

    public bool IsBuildingUnlocked(BuildingData data) => unlockedBuildings.Contains(data);

    public void UnlockBuilding(BuildingData data)
    {
        if (data == null || unlockedBuildings.Contains(data)) return;
        unlockedBuildings.Add(data);
        AddBuildingIcon(data);
    }

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
