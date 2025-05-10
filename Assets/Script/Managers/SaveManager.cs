using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "save.json");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
            StartCoroutine(DelayedLoad());
    }

    private IEnumerator DelayedLoad()
    {
        yield return null;
        LoadGame();
    }

    public void SaveGame()
    {
        Debug.Log($"[SaveManager] Saving to {savePath}");
        if (BuildingSelectionPanel.Instance == null || ResourceManager.Instance == null || GameTime.Instance == null)
        {
            Debug.LogError("[SaveManager] Cannot save: missing manager instances.");
            return;
        }

        var save = new GameSave
        {
            buildings = FindObjectsOfType<Building>()
                .Select(b => new BuildingSave
                {
                    buildingDataId = b.data.name,
                    gridX = b.origin.x,
                    gridY = b.origin.y,
                    variantIndex = b.variantIndex
                }).ToList(),

            unlockedBuildings = BuildingSelectionPanel.Instance
                .GetUnlockedBuildings()
                .Select(d => new UnlockSave { buildingDataId = d.name })
                .ToList(),

            currentResources = ResourceManager.Instance.GetCurrentAmounts(),

            roads = FindObjectsOfType<Road>()
                .Select(r => new RoadSave
                {
                    roadDataId = r.data.name,
                    cells = r.GetCells().ToList(),
                    variantIndex = r.variantIndex
                }).ToList(),

            year = GameTime.Instance.year,
            month = GameTime.Instance.month,
            day = GameTime.Instance.day,
            hour = GameTime.Instance.hour,
            minute = GameTime.Instance.minute
        };

        string json = JsonUtility.ToJson(save, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"[SaveManager] Save complete: {save.buildings.Count} buildings, {save.roads.Count} roads, time {save.hour}:{save.minute:D2}");
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log($"[SaveManager] No save found at {savePath}");
            return;
        }

        string json = File.ReadAllText(savePath);
        GameSave save = JsonUtility.FromJson<GameSave>(json);
        Debug.Log($"[SaveManager] Loading: {save.buildings.Count} buildings, {save.roads.Count} roads, time {save.hour}:{save.minute:D2}");

        // Destroy existing objects
        foreach (var b in FindObjectsOfType<Building>()) Destroy(b.gameObject);
        foreach (var r in FindObjectsOfType<Road>()) Destroy(r.gameObject);

        // Recreate roads first
        int roadsCreated = 0;
        foreach (var rs in save.roads)
        {
            var data = Resources.Load<RoadData>($"Data/Roads/{rs.roadDataId}");
            if (data == null)
            {
                Debug.LogWarning($"[SaveManager] RoadData not found: {rs.roadDataId}");
                continue;
            }
            GameObject go = Instantiate(data.prefab);
            var road = go.GetComponent<Road>();
            road.Initialize(data, rs.cells, rs.variantIndex);
            roadsCreated++;
        }
        Debug.Log($"[SaveManager] {roadsCreated}/{save.roads.Count} roads recreated");

        // Recreate buildings
        int buildingsCreated = 0;
        foreach (var bs in save.buildings)
        {
            var data = Resources.Load<BuildingData>($"Data/Buildings/{bs.buildingDataId}");
            if (data == null)
            {
                Debug.LogWarning($"[SaveManager] BuildingData not found: {bs.buildingDataId}");
                continue;
            }
            GameObject go = Instantiate(data.prefab);
            go.GetComponent<Building>().Initialize(data, new Vector2Int(bs.gridX, bs.gridY), bs.variantIndex);
            buildingsCreated++;
        }
        Debug.Log($"[SaveManager] {buildingsCreated}/{save.buildings.Count} buildings recreated");

        // Restore unlocked buildings in selection panel
        var panel = BuildingSelectionPanel.Instance;
        var unlockedList = save.unlockedBuildings
            .Select(u => Resources.Load<BuildingData>($"Data/Buildings/{u.buildingDataId}"))
            .Where(d => d != null)
            .ToList();
        panel.Initialize(unlockedList, panel.initialRoads);
        Debug.Log($"[SaveManager] Selection panel reset: {unlockedList.Count} unlocked buildings");

        // Restore resources & time
        ResourceManager.Instance.SetAmounts(save.currentResources);
        GameTime.Instance.year = save.year;
        GameTime.Instance.month = save.month;
        GameTime.Instance.day = save.day;
        GameTime.Instance.hour = save.hour;
        GameTime.Instance.minute = save.minute;

        Debug.Log("[SaveManager] Load complete");
    }
}
