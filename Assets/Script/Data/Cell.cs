using UnityEngine;

public enum CellType { Empty, Building, Road }

public class Cell
{
    public Vector2Int coord;
    public CellType type = CellType.Empty;
    public GameObject occupant;

    // ← Ajout de ce booléen pour marquer l'occupation
    public bool isOccupied = false;

    public Cell(Vector2Int c)
    {
        coord = c;
        isOccupied = false;
    }
}
