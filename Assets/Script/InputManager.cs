// InputManager.cs
// Simplifié : suppression des toggles UI, prend en charge les placements via script (icônes)
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    [Header("Refs Manager")]
    public PlacementManager placement;
    public GameObject tileHighlightPrefab;
    public Material matValid, matInvalid;

    // Internals
    private List<GameObject> highlights = new List<GameObject>();
    private BuildingData selectedBuilding = null;
    private RoadData selectedRoad = null;
    private bool inPlaceMode => selectedBuilding != null || selectedRoad != null;
    private Vector2Int? roadStart = null;

    private bool demolitionMode = false;

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
    }

    void Update()
    {
        // Ne pas interférer si on clique sur l'UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (demolitionMode)
        {
            if (Input.GetMouseButtonDown(1))
            {
                ClearMode();
                return;
            }

            HandleDemolition();
            return;
        }

        // Si on n'est pas déjà en placement ou démolition et qu'on clique gauche :
        if (!inPlaceMode && !demolitionMode && Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject() && CastRay(out Vector3 wp))
            {
                var coord = WorldPosToCell(wp);
                var cell = placement.gridManager.GetCell(coord);
                if (cell != null && cell.occupant != null)
                {
                    var b = cell.occupant.GetComponent<Building>();
                    if (b != null)
                    {
                        BuildingInfoUI.Instance.Show(b);
                        return;
                    }
                }
            }
            // Si on clique ailleurs, on cache le panel
            BuildingInfoUI.Instance.Hide();
        }

        if (!inPlaceMode) return;

        // Clic droit annule tout
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

    /// <summary>
    /// Déclenché par UI script pour débuter le placement d'un building
    /// </summary>
    public void StartBuildingPlacement(BuildingData data)
    {
        ClearMode();
        SetBuildingMode(data);
    }

    /// <summary>
    /// Déclenché par UI script pour débuter le placement d'une route
    /// </summary>
    public void StartRoadPlacement(RoadData data)
    {
        ClearMode();
        SetRoadMode(data);
    }

    // Set modes internes
    private void SetBuildingMode(BuildingData data)
    {
        selectedBuilding = data;
        selectedRoad = null;
        roadStart = null;
        ShowGrid(true);
    }

    public void SetRoadMode(RoadData data)
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
        demolitionMode = false;

        ShowGrid(false);
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
        // Phase 1: choix du départ
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

        // Phase 2: affichage du chemin
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
            foreach (var cell in path)
                placement.TryPlaceRoad(cell);
            ConnectionManager.Instance.RegisterRoad();
            roadStart = null;
        }
    }

    List<Vector2Int> GetManhattanPath(Vector2Int start, Vector2Int end)
    {
        var path = new List<Vector2Int>();
        int dirX = end.x >= start.x ? 1 : -1;
        for (int x = start.x; x != end.x; x += dirX)
            path.Add(new Vector2Int(x, start.y));
        path.Add(new Vector2Int(end.x, start.y));
        int dirY = end.y >= start.y ? 1 : -1;
        for (int y = start.y; y != end.y; y += dirY)
            path.Add(new Vector2Int(end.x, y));
        path.Add(new Vector2Int(end.x, end.y));
        return path;
    }

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
                var cell = origin + new Vector2Int(dx, dy);
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

    /// <summary> Bascule le mode démolition ON/OFF </summary>
    public void ToggleDemolitionMode()
    {
        if (demolitionMode)
            ClearMode();
        else
            StartDemolitionMode();
    }


    private void HandleDemolition()
    {
        // highlight the hovered cell if occupied
        if (!CastRay(out Vector3 wp)) { /* clear highlight */ return; }
        var coord = WorldPosToCell(wp);
        var cell = placement.gridManager.GetCell(coord);

        // position a highlight...
        // (reuse your tileHighlightPrefab pool, coloring any occupied cell)
        // 

        if (Input.GetMouseButtonDown(0) && cell != null && cell.type != CellType.Empty)
        {
            placement.TryDemolishAt(coord);
        }
    }

    /// <summary>Switch into Demolition mode.</summary>
    public void StartDemolitionMode()
    {
        ClearMode();  // disable any building/road selection
        demolitionMode = true;
        ShowGrid(true);
    }
}
