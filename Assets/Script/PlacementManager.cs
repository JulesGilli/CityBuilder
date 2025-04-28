// Assets/Scripts/PlacementManager.cs
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public GridManager gridManager;
    public RoadData roadData;  // Lie ici ton unique RoadData

    // Pose un bâtiment multi-cells
    // PlacementManager.cs
    public bool TryPlaceBuilding(BuildingData data, Vector2Int origin)
    {
        // Vérification que toutes les cellules sont libres…
        if (!CanPlaceBuilding(data, origin))
            return false;

        // Calcule le centre exact de la zone data.size × data.size
        Vector3 worldPos = gridManager.GetWorldCenter(origin, data.size);

        // Instancie au bon endroit
        GameObject go = Instantiate(data.prefab, worldPos, Quaternion.identity, transform);

        // Dans TryPlaceBuilding, après Instantiate :
        Building b = go.AddComponent<Building>();
        b.origin = origin;
        b.size = data.size;

        // Informe le ConnectionManager
        ConnectionManager.Instance.RegisterBuilding(b);

        // Marque les cellules comme occupées
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
        for (int dx = 0; dx < data.size.x; dx++)
            for (int dy = 0; dy < data.size.y; dy++)
            {
                var c = gridManager.GetCell(origin + new Vector2Int(dx, dy));
                if (c == null || c.type != CellType.Empty) return false;
            }
        return true;
    }

    public bool CanPlaceRoad(Vector2Int o)
    {
        var c = gridManager.GetCell(o);
        return c != null && c.type == CellType.Empty;
    }

    public bool TryPlaceRoad(Vector2Int coord)
    {
        var c = gridManager.GetCell(coord);
        if (c == null || c.type != CellType.Empty) return false;

        Vector3 worldPos = gridManager.GetWorldCenter(coord, Vector2Int.one);
        GameObject go = Instantiate(roadData.prefab, worldPos, Quaternion.identity, transform);

        ConnectionManager.Instance.RegisterRoad();

        c.type = CellType.Road;
        c.occupant = go;
        return true;
    }
}
