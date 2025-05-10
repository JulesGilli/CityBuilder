// Road.cs
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Road : MonoBehaviour
{
    [HideInInspector] public RoadData data;
    [HideInInspector] public List<Vector2Int> cells;
    [HideInInspector] public int variantIndex;

    /// <summary>
    /// Initialize this road after load or placement.
    /// </summary>
    public void Initialize(RoadData data, List<Vector2Int> cells, int variantIndex = 0)
    {
        this.data = data;
        this.cells = cells;
        this.variantIndex = variantIndex;

        // Position at center of road footprint
        Vector2Int min = cells[0];
        Vector2Int max = cells[0];
        foreach (var c in cells)
        {
            min = new Vector2Int(Mathf.Min(min.x, c.x), Mathf.Min(min.y, c.y));
            max = new Vector2Int(Mathf.Max(max.x, c.x), Mathf.Max(max.y, c.y));
        }
        Vector2Int size = max - min + Vector2Int.one;
        transform.position = GridManager.Instance.GetWorldCenter(min, size);

        // Mark grid cells
        foreach (var coord in cells)
        {
            var cell = GridManager.Instance.GetCell(coord);
            if (cell == null) continue;
            cell.isOccupied = true;
            cell.type = CellType.Road;
            cell.occupant = gameObject;
        }

        // Ensure collider and raycast pickable
        Collider col = GetComponent<Collider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = false;

        // Register in connection manager (no argument)
        ConnectionManager.Instance.RegisterRoad();
    }

    /// <summary>
    /// Returns the occupied grid cells.
    /// </summary>
    public IEnumerable<Vector2Int> GetCells() => cells;
}