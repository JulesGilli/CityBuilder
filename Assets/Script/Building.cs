using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    [HideInInspector] public Vector2Int origin;   // coin bas-gauche en cellules
    [HideInInspector] public Vector2Int size;     // taille en cellules
    [HideInInspector] public List<Building> connected = new List<Building>();

    [HideInInspector] public BuildingData data;
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
