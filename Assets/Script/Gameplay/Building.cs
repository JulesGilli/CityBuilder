// Building.cs
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [HideInInspector] public Vector2Int origin;   // coin bas-gauche en cellules
    [HideInInspector] public Vector2Int size;     // taille en cellules
    [HideInInspector] public List<Building> connected = new List<Building>();

    [HideInInspector] public BuildingData data;
    public int variantIndex;


    public void Initialize(BuildingData data, Vector2Int origin)
        => Initialize(data, origin, 0);
    /// <summary>
    /// Initialise ce b�timent apr�s un Load ou une construction.
    /// Pose l'objet, choisit la variante et marque les cellules occup�es.
    /// </summary>
    public void Initialize(BuildingData data, Vector2Int origin, int variantIndex = 0)
    {
        this.data = data;
        this.origin = origin;
        this.size = data.size;
        this.variantIndex = variantIndex;

        // Centre le b�timent sur la grille
        transform.position = GridManager.Instance.GetWorldCenter(origin, size);

        // Marque la grille
        foreach (var coord in OccupiedCells())
        {
            var cell = GridManager.Instance.GetCell(coord);
            if (cell == null) continue;
            cell.isOccupied = true;
            cell.type = CellType.Building;
            cell.occupant = gameObject;
        }

        // R�cup�re ou cr�e un collider, puis d�sactive le trigger
        Collider col = GetComponent<Collider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = false;

        // Enregistre dans le ConnectionManager
        ConnectionManager.Instance.RegisterBuilding(this);
    }


    /// <summary>
    /// Renvoie la liste des cellules occup�es par ce b�timent.
    /// </summary>
    public IEnumerable<Vector2Int> OccupiedCells()
    {
        for (int dx = 0; dx < size.x; dx++)
            for (int dy = 0; dy < size.y; dy++)
                yield return origin + new Vector2Int(dx, dy);
    }
}
