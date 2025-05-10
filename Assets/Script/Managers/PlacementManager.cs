// Assets/Scripts/PlacementManager.cs
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public GridManager gridManager;
    public RoadData roadData;  // Lie ici ton unique RoadData

    [Header("Parent Containers")]
    [Tooltip("Conteneur pour tous les bâtiments")]
    public Transform buildingsParent;
    // Pose un bâtiment multi-cells
    // PlacementManager.cs
    public bool TryPlaceBuilding(BuildingData data, Vector2Int origin)
    {
        // ─── 0) Vérification des ressources ───────────────────────
        // On s’assure qu’on peut payer le coût avant même de tester la grille
        foreach (var cost in data.constructionCost)
        {
            if (ResourceManager.Instance.Get(cost.resourceType) < cost.amount)
            {
                Debug.LogWarning(
                  $"Pas assez de {cost.resourceType} pour construire {data.name} " +
                  $"(nécessaire : {cost.amount}, dispo : {ResourceManager.Instance.Get(cost.resourceType)})"
                );
                return false;
            }
        }

        // ─── 1) Vérification des cellules libres ──────────────────
        for (int dx = 0; dx < data.size.x; dx++)
            for (int dy = 0; dy < data.size.y; dy++)
            {
                var cell = origin + new Vector2Int(dx, dy);
                Cell c = gridManager.GetCell(cell);
                if (c == null || c.type != CellType.Empty) return false;
            }

        // ─── 2) Instanciation du prefab ───────────────────────────
        Vector3 worldPos = gridManager.GetWorldCenter(origin, data.size);
        // Choisit un prefab au hasard parmi les variantes, ou fallback
        GameObject chosen = (data.prefabVariants != null && data.prefabVariants.Length > 0)
            ? data.prefabVariants[Random.Range(0, data.prefabVariants.Length)]
            : data.prefab;

        GameObject go = Instantiate(chosen, worldPos, Quaternion.identity, buildingsParent);


        // (ajout de votre composant Building, etc.)
        Building b = go.GetComponent<Building>() ?? go.AddComponent<Building>();
        b.origin = origin;
        b.size = data.size;
        b.data = data;
        ConnectionManager.Instance.RegisterBuilding(b);

        // ─── 3) On dépense réellement les ressources ─────────────
        foreach (var cost in data.constructionCost)
        {
            bool paid = ResourceManager.Instance.Spend(cost.resourceType, cost.amount);
            // paid est toujours true ici, car on a préalablement testé Get(...)
        }

        // ─── 4) Mise à jour des cellules ──────────────────────────
        for (int dx = 0; dx < data.size.x; dx++)
            for (int dy = 0; dy < data.size.y; dy++)
            {
                var c = gridManager.GetCell(origin + new Vector2Int(dx, dy));
                c.type = CellType.Building;
                c.occupant = go;
            }

        return true;
    }

    public bool CanPlaceBuilding(BuildingData data, Vector2Int origin)
    {
        // 1) Vérification que toutes les cellules du bâtiment sont libres
        for (int dx = 0; dx < data.size.x; dx++)
        {
            for (int dy = 0; dy < data.size.y; dy++)
            {
                var cell = gridManager.GetCell(origin + new Vector2Int(dx, dy));
                if (cell == null || cell.type != CellType.Empty)
                    return false;
            }
        }

        // 2) Si c'est la carrière, s'assurer qu'elle est placée à côté d'un gisement
        if (data.name == "Quarry" && !HasDepositNearby(origin, data.size))
            return false;

        // (éventuels autres checks spécifiques à d'autres bâtiments)

        return true;
    }

    private bool HasDepositNearby(Vector2Int origin, Vector2Int size)
    {
        // On balaye une bordure d'une cellule tout autour de l'emprise du bâtiment
        for (int dx = -1; dx <= size.x; dx++)
        {
            for (int dy = -1; dy <= size.y; dy++)
            {
                var cellPos = origin + new Vector2Int(dx, dy);

                // Ignorer hors grille
                if (!gridManager.IsValidCell(cellPos))
                    continue;

                // Si l'occupant est un gisement, ok
                var occ = gridManager.GetCell(cellPos).occupant;
                if (occ != null && occ.GetComponent<MineDeposit>() != null)
                    return true;
            }
        }

        // Aucun gisement trouvé à proximité
        return false;
    }


    public bool CanPlaceRoad(Vector2Int o)
    {
        var c = gridManager.GetCell(o);
        return c != null && c.type == CellType.Empty;
    }

    public bool TryPlaceRoad(Vector2Int coord)
    {
        // 1) Vérification de la cellule
        var c = gridManager.GetCell(coord);
        if (c == null || c.type != CellType.Empty)
            return false;

        // 2) Instanciation du prefab
        Vector3 worldPos = gridManager.GetWorldCenter(coord, Vector2Int.one);
        GameObject go = Instantiate(roadData.prefab, worldPos, Quaternion.identity, transform);

        // 3) Configuration du composant Road
        var roadComp = go.GetComponent<Road>() ?? go.AddComponent<Road>();
        roadComp.data = roadData;
        // on stocke la liste des cellules (ici, une seule case)
        roadComp.cells = new List<Vector2Int> { coord };

        // 4) Enregistrer la route (sans argument, comme dans ton ConnectionManager actuel)
        ConnectionManager.Instance.RegisterRoad();

        // 5) Met à jour la grille
        c.type = CellType.Road;
        c.occupant = go;

        return true;
    }


    /// <summary>
    /// Try to demolish whatever occupies this cell (building or road).
    /// </summary>
    public bool TryDemolishAt(Vector2Int coord)
    {
        var cell = gridManager.GetCell(coord);
        if (cell == null || cell.type == CellType.Empty) return false;

        // 1) Si c'est un bâtiment
        var b = cell.occupant.GetComponent<Building>();
        if (b != null)
        {
            DemolishBuilding(b);
            return true;
        }

        // 2) Sinon on cherche un Road
        var r = cell.occupant.GetComponent<Road>();
        if (r != null)
        {
            DemolishRoad(r);
            return true;
        }

        return false;
    }

    private void DemolishBuilding(Building b)
    {
        // 1) refund half the cost
        foreach (var cost in b.data.constructionCost)
        {
            int refund = Mathf.FloorToInt(cost.amount * 0.5f);
            ResourceManager.Instance.Add(cost.resourceType, refund);
        }

        // 2) clear all its cells
        for (int dx = 0; dx < b.size.x; dx++)
            for (int dy = 0; dy < b.size.y; dy++)
            {
                var pos = b.origin + new Vector2Int(dx, dy);
                if (!gridManager.IsValidCell(pos)) continue;
                var c = gridManager.GetCell(pos);
                if (c.occupant == b.gameObject)
                {
                    c.occupant = null;
                    c.type = CellType.Empty;
                }
            }

        // 3) destroy GameObject and update connections
        Destroy(b.gameObject);
        ConnectionManager.Instance.RegisterRoad();
    }

    private void DemolishRoad(Road r)
    {
        // 1) Remboursement à 50%
        foreach (var cost in r.data.constructionCost)
        {
            int refund = Mathf.FloorToInt(cost.amount * 0.5f);
            ResourceManager.Instance.Add(cost.resourceType, refund);
        }

        // 2) Libération de toutes les cellules de la route
        foreach (var cellCoord in r.cells)
        {
            var cell = gridManager.GetCell(cellCoord);
            if (cell != null && cell.occupant == r.gameObject)
            {
                cell.occupant = null;
                cell.type = CellType.Empty;
            }
        }

        // 3) Destruction du GameObject
        Destroy(r.gameObject);

        // 4) Recalcul des connexions (toujours sans argument)
        ConnectionManager.Instance.RegisterRoad();
    }
}
