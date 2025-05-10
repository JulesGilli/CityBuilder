// Building.cs
using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    [HideInInspector] public Vector2Int origin;
    [HideInInspector] public Vector2Int size;
    [HideInInspector] public BuildingData data;
    [HideInInspector] public int variantIndex;

    /// <summary>
    /// Initialise ce b�timent apr�s Load ou construction.
    /// Positionne le GameObject et marque la grille.
    /// </summary>
    public void Initialize(BuildingData data, Vector2Int origin, int variantIndex = 0)
    {
        this.data = data;
        this.origin = origin;
        this.size = data.size;
        this.variantIndex = variantIndex;

        // D�finir position
        Vector3 worldPos = Vector3.zero;
        if (GridManager.Instance != null)
            worldPos = GridManager.Instance.CellToWorld(origin);
        else
            Debug.LogWarning("[Building] GridManager non disponible, position par d�faut utilis�e");
        transform.position = worldPos;

        // Marquer les cellules occup�es
        if (GridManager.Instance != null)
        {
            foreach (var coord in OccupiedCells())
            {
                var cell = GridManager.Instance.GetCell(coord);
                if (cell != null)
                {
                    cell.isOccupied = true;
                    cell.occupant = gameObject;
                    cell.type = CellType.Building;
                }
            }
        }
    }

    /// <summary>
    /// Retourne tous les offsets de cellules occup�es par ce b�timent.
    /// </summary>
    public IEnumerable<Vector2Int> OccupiedCells()
    {
        for (int dx = 0; dx < size.x; dx++)
            for (int dy = 0; dy < size.y; dy++)
                yield return origin + new Vector2Int(dx, dy);
    }
}