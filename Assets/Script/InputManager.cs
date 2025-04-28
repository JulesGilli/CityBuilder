using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    [Header("Refs Manager")]
    public PlacementManager placement;
    public GameObject tileHighlightPrefab;
    public Material matValid, matInvalid;

    [Header("UI Toggles")]
    public Toggle[] buildingToggles;
    public BuildingData[] buildingVariants;
    public Toggle roadToggle;
    public RoadData roadVariant;

    // Internals
    private List<GameObject> highlights = new List<GameObject>();
    private BuildingData selectedBuilding = null;
    private RoadData selectedRoad = null;
    private bool inPlaceMode => selectedBuilding != null || selectedRoad != null;

    // Nouvelle variable pour le mode routes "start-end"
    private Vector2Int? roadStart = null;

    void Start()
    {
        // Pool de quads pour le surlignage
        int maxTiles = placement.gridManager.width * placement.gridManager.height;
        for (int i = 0; i < maxTiles; i++)
        {
            var go = Instantiate(tileHighlightPrefab,
                                 Vector3.zero,
                                 Quaternion.Euler(90, 0, 0),
                                 transform);
            go.SetActive(false);
            highlights.Add(go);
        }

        ClearMode();

        // Abonnements aux toggles
        for (int i = 0; i < buildingToggles.Length; i++)
        {
            int idx = i;
            buildingToggles[i].onValueChanged.AddListener(on =>
            {
                if (on) SetBuildingMode(buildingVariants[idx]);
                else if (selectedBuilding == buildingVariants[idx]) ClearMode();
            });
        }

        roadToggle.onValueChanged.AddListener(on =>
        {
            if (on) SetRoadMode(roadVariant);
            else if (selectedRoad == roadVariant) ClearMode();
        });
    }

    void Update()
    {
        // Ne pas interférer si on clique sur l'UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (!inPlaceMode) return;

        // Clic droit annule tout (et reset roadStart)
        if (Input.GetMouseButtonDown(1))
        {
            roadStart = null;
            ClearMode();
            return;
        }

        if (selectedBuilding != null)
            HandlePreviewAndPlaceBuilding();
        else if (selectedRoad != null)
            HandlePreviewAndPlaceRoad();
    }

    void SetBuildingMode(BuildingData data)
    {
        selectedBuilding = data;
        selectedRoad = null;
        roadStart = null;
        ShowGrid(true);
    }

    void SetRoadMode(RoadData data)
    {
        selectedRoad = data;
        selectedBuilding = null;
        roadStart = null;
        ShowGrid(true);
    }

    public void ClearMode()
    {
        selectedBuilding = null;
        selectedRoad = null;
        roadStart = null;
        ShowGrid(false);
        foreach (var t in buildingToggles) t.isOn = false;
        roadToggle.isOn = false;
        DisableAllHighlights();
    }

    void ShowGrid(bool on)
    {
        var gr = GetComponent<GridRenderer>();
        if (gr) gr.enabled = on;
    }

    void HandlePreviewAndPlaceBuilding()
    {
        var data = selectedBuilding;
        if (!CastRay(out Vector3 wp)) { DisableAllHighlights(); return; }

        var origin = WorldPosToCell(wp);
        bool canPlace = placement.CanPlaceBuilding(data, origin);

        UpdateHighlights(origin, data.size, canPlace);

        if (Input.GetMouseButtonDown(0) && canPlace)
            placement.TryPlaceBuilding(data, origin);
    }

    void HandlePreviewAndPlaceRoad()
    {
        if (!CastRay(out Vector3 wp)) { DisableAllHighlights(); return; }

        var hover = WorldPosToCell(wp);
        // Phase 1 : on n'a pas encore de point de départ
        if (roadStart == null)
        {
            bool ok = placement.CanPlaceRoad(hover);
            DisableAllHighlights();
            if (placement.gridManager.IsValidCell(hover))
            {
                var h = highlights[0];
                h.SetActive(true);
                Vector3 c = placement.gridManager.GetWorldCenter(hover, Vector2Int.one);
                h.transform.position = c + Vector3.up * 0.01f;
                h.GetComponent<MeshRenderer>().sharedMaterial = ok ? matValid : matInvalid;
            }
            if (Input.GetMouseButtonDown(0) && ok)
                roadStart = hover;
            return;
        }

        // Phase 2 : on a un départ → on montre le chemin
        var start = roadStart.Value;
        var path = GetManhattanPath(start, hover);
        bool allEmpty = true;
        foreach (var cell in path)
            if (!placement.CanPlaceRoad(cell)) { allEmpty = false; break; }

        DisableAllHighlights();
        for (int i = 0; i < path.Count && i < highlights.Count; i++)
        {
            var h = highlights[i];
            h.SetActive(true);
            Vector3 c = placement.gridManager.GetWorldCenter(path[i], Vector2Int.one);
            h.transform.position = c + Vector3.up * 0.01f;
            h.GetComponent<MeshRenderer>().sharedMaterial = allEmpty ? matValid : matInvalid;
        }

        if (Input.GetMouseButtonDown(0) && allEmpty)
        {
            // 1) On place toutes les tuiles de route
            foreach (var cell in path)
                placement.TryPlaceRoad(cell);

            // 2) On notifie le ConnectionManager pour recalculer
            ConnectionManager.Instance.RegisterRoad();

            // 3) On réinitialise le sélection de départ
            roadStart = null;
        }

    }

    // Génère un chemin "Manhattan" start→end
    List<Vector2Int> GetManhattanPath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        // horizontal
        int dirX = end.x >= start.x ? 1 : -1;
        for (int x = start.x; x != end.x; x += dirX)
            path.Add(new Vector2Int(x, start.y));
        // ajoute la dernière colonne
        path.Add(new Vector2Int(end.x, start.y));
        // vertical
        int dirY = end.y >= start.y ? 1 : -1;
        for (int y = start.y; y != end.y; y += dirY)
            path.Add(new Vector2Int(end.x, y));
        // ajoute la dernière ligne
        path.Add(new Vector2Int(end.x, end.y));
        return path;
    }

    // Helpers
    bool CastRay(out Vector3 wp)
    {
        wp = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float d))
            return false;
        wp = ray.GetPoint(d);
        return true;
    }

    Vector2Int WorldPosToCell(Vector3 wp)
    {
        float cs = placement.gridManager.cellSize;
        return new Vector2Int(
            Mathf.FloorToInt(wp.x / cs),
            Mathf.FloorToInt(wp.z / cs)
        );
    }

    void UpdateHighlights(Vector2Int origin, Vector2Int size, bool valid)
    {
        DisableAllHighlights();
        int idx = 0;
        for (int dx = 0; dx < size.x; dx++)
            for (int dy = 0; dy < size.y; dy++)
            {
                Vector2Int cell = origin + new Vector2Int(dx, dy);
                if (!placement.gridManager.IsValidCell(cell)) continue;
                var h = highlights[idx++];
                h.SetActive(true);
                Vector3 c = placement.gridManager.GetWorldCenter(cell, Vector2Int.one);
                h.transform.position = c + Vector3.up * 0.01f;
                h.GetComponent<MeshRenderer>().sharedMaterial = valid ? matValid : matInvalid;
            }
    }

    void DisableAllHighlights()
    {
        foreach (var h in highlights) h.SetActive(false);
    }
}
