// Building.cs
using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    [HideInInspector] public Vector2Int origin;   // coin bas-gauche en cellules
    [HideInInspector] public Vector2Int size;     // taille en cellules
    [HideInInspector] public List<Building> connected = new List<Building>();

    [HideInInspector] public BuildingData data;
    public int variantIndex;

    /// <summary>
    /// Initialise ce bâtiment après un Load ou une construction.
    /// Pose l'objet, choisit la variante et marque les cellules occupées.
    /// </summary>
    public void Initialize(BuildingData data, Vector2Int origin, int variantIndex = 0)
    {
        this.data = data;
        this.origin = origin;
        this.size = data.size;
        this.variantIndex = variantIndex;

        // Position dans le monde
        Vector3 worldPos;
        if (GridManager.Instance != null)
            worldPos = GridManager.Instance.CellToWorld(origin);
        else
        {
            worldPos = new Vector3(origin.x, 0, origin.y);
            Debug.LogWarning("[Building] GridManager.Instance est null, position par défaut utilisée");
        }
        transform.position = worldPos;

        // Marquer les cellules sur la grille
        if (GridManager.Instance != null)
        {
            foreach (var cellCoord in OccupiedCells())
            {
                var cell = GridManager.Instance.GetCell(cellCoord);
                if (cell != null)
                    cell.isOccupied = true;  // nécessite un bool isOccupied dans Cell
            }
        }

        foreach (var cellCoord in OccupiedCells())
        {
            var cell = GridManager.Instance.GetCell(cellCoord);
            if (cell != null)
                cell.isOccupied = true;
        }

    }

    /// <summary>
    /// Renvoie la liste des cellules occupées par ce bâtiment.
    /// </summary>
    public IEnumerable<Vector2Int> OccupiedCells()
    {
        for (int dx = 0; dx < size.x; dx++)
            for (int dy = 0; dy < size.y; dy++)
                yield return origin + new Vector2Int(dx, dy);
    }
}
