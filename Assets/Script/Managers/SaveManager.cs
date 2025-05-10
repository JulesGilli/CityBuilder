using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-100)]
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    string savePath;

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
        // Attendre la fin de la frame courante
        yield return null;
        LoadGame();
    }

    public void SaveGame()
    {
        Debug.Log($"[SaveManager] Attempting to save to {savePath}");
        // Ensure managers are present
        if (BuildingSelectionPanel.Instance == null || ResourceManager.Instance == null || GameTime.Instance == null)
        {
            Debug.LogError("[SaveManager] Cannot save: missing instances. BSP=" + BuildingSelectionPanel.Instance + ", RM=" + ResourceManager.Instance + ", GT=" + GameTime.Instance);
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

            year = GameTime.Instance.year,
            month = GameTime.Instance.month,
            day = GameTime.Instance.day,
            hour = GameTime.Instance.hour,
            minute = GameTime.Instance.minute
        };

        var json = JsonUtility.ToJson(save, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"[SaveManager] Save complete: {save.buildings.Count} buildings, {save.unlockedBuildings.Count} unlocked, time {save.hour}:{save.minute:D2}");
    }

    public void LoadGame()
    {
        // 1) Vérifie que le fichier existe
        if (!File.Exists(savePath))
        {
            Debug.Log("[SaveManager] Aucune sauvegarde trouvée à " + savePath);
            return;
        }

        // 2) Lit et désérialise le JSON
        string json = File.ReadAllText(savePath);
        GameSave save = JsonUtility.FromJson<GameSave>(json);
        Debug.Log($"[SaveManager] Chargement : {save.buildings.Count} bâtiments, {save.unlockedBuildings.Count} débloqués, heure {save.hour}:{save.minute:D2}");

        // 3) Vider la scène des bâtiments existants
        foreach (var b in FindObjectsOfType<Building>())
            Destroy(b.gameObject);

        // 4) Recréer les bâtiments placés
        int created = 0;
        foreach (var bs in save.buildings)
        {
            var data = Resources.Load<BuildingData>($"Data/Buildings/{bs.buildingDataId}");
            if (data == null)
            {
                Debug.LogWarning($"[SaveManager] BuildingData non trouvé : {bs.buildingDataId}");
                continue;
            }
            GameObject go = Instantiate(data.prefab);
            go.GetComponent<Building>().Initialize(data, new Vector2Int(bs.gridX, bs.gridY), bs.variantIndex);
            created++;
        }
        Debug.Log($"[SaveManager] {created}/{save.buildings.Count} bâtiments recréés");

        // 5) Restaurer la liste des débloqués (bâtiments + routes)
        //    On utilise panel.initialRoads pour réinjecter les routes de base
        var panel = BuildingSelectionPanel.Instance;

        List<BuildingData> unlockedB = save.unlockedBuildings
            .Select(u => Resources.Load<BuildingData>($"Data/Buildings/{u.buildingDataId}"))
            .Where(d => d != null)
            .ToList();

        panel.Initialize(
            unlockedB,
            panel.initialRoads
        );
        Debug.Log($"[SaveManager] Panel de sélection réinitialisé : {unlockedB.Count} bâtiments débloqués, {panel.initialRoads.Count} routes disponibles");

        // 6) Restaurer ressources et temps
        ResourceManager.Instance.SetAmounts(save.currentResources);

        GameTime.Instance.year = save.year;
        GameTime.Instance.month = save.month;
        GameTime.Instance.day = save.day;
        GameTime.Instance.hour = save.hour;
        GameTime.Instance.minute = save.minute;

        Debug.Log("[SaveManager] Restauration terminée");
    }

}
