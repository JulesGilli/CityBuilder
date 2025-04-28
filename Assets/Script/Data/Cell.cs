using UnityEngine;

public enum CellType { Empty, Building, Road }

public class Cell
{
    public Vector2Int coord;
    public CellType type = CellType.Empty;
    public GameObject occupant;

    public Cell(Vector2Int c) { coord = c; }
}
