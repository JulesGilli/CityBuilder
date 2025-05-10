using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Collider))]
public class MineDeposit : MonoBehaviour
{
    [Tooltip("Type de ressource du gisement")]
    public ResourceType depositType = ResourceType.Stone;

    [Tooltip("Taille du gisement en cellules")]
    public Vector2Int size = Vector2Int.one;

    private GridManager _grid;

    void Awake()
    {
        var placement = Object.FindFirstObjectByType<PlacementManager>();
        if (placement != null)
            _grid = placement.gridManager;
    }

    void OnValidate()
    {
        SnapToGrid();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
            SnapToGrid();
    }
#endif

    void Start()
    {
        if (!Application.isPlaying || _grid == null)
            return;


        // 1) Recentre proprement
        SnapToGrid();
        // 2) Enregistre ensuite l’occupation
        RegisterOccupancy();
    }

    void OnDestroy()
    {
        if (Application.isPlaying)
            UnregisterOccupancy();
    }

    /// <summary>
    /// Calcule la cellule “coin bas-gauche” à partir du centre (transform.position).
    /// </summary>
    private Vector2Int GetOriginCell()
    {
        float cs = _grid.cellSize;
        Vector3 pos = transform.position;

        // Passage en coordonnées de coin bas-gauche
        float originXF = pos.x - (size.x * cs) / 2f;
        float originYF = pos.z - (size.y * cs) / 2f;

        int originX = Mathf.RoundToInt(originXF / cs);
        int originY = Mathf.RoundToInt(originYF / cs);

        return new Vector2Int(originX, originY);
    }

    /// <summary>
    /// Positionne le dépôt de manière à ce que son centre couvre exactement la zone de size.x×size.y cellules.
    /// </summary>
    private void SnapToGrid()
    {
        if (_grid == null) return;

        float cs = _grid.cellSize;
        Vector2Int origin = GetOriginCell();

        // On remet la position pour centrer la zone
        Vector3 pos = transform.position;
        pos.x = origin.x * cs + (size.x * cs) / 2f;
        pos.z = origin.y * cs + (size.y * cs) / 2f;
        transform.position = pos;
    }

    private void RegisterOccupancy()
    {
        var origin = GetOriginCell();

        for (int dx = 0; dx < size.x; dx++)
            for (int dy = 0; dy < size.y; dy++)
            {
                var cellPos = origin + new Vector2Int(dx, dy);
                if (!_grid.IsValidCell(cellPos)) continue;
                var cell = _grid.GetCell(cellPos);
                cell.occupant = gameObject;
                cell.type = CellType.Building;
            }
    }

    private void UnregisterOccupancy()
    {
        var origin = GetOriginCell();

        for (int dx = 0; dx < size.x; dx++)
            for (int dy = 0; dy < size.y; dy++)
            {
                var cellPos = origin + new Vector2Int(dx, dy);
                if (!_grid.IsValidCell(cellPos)) continue;
                var cell = _grid.GetCell(cellPos);
                if (cell.occupant == gameObject)
                {
                    cell.occupant = null;
                    cell.type = CellType.Empty;
                }
            }
    }
}
