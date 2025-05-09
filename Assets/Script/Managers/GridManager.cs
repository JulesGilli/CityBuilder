// GridManager.cs
using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public int width, height;
    public float cellSize = 1f;

    // Représentation interne
    private Cell[,] grid;

    void Awake()
    {
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Cell(new Vector2Int(x, y));
            }
        }
    }

    // Convertit coordonnée de grille → position monde
    public Vector3 CellToWorld(Vector2Int coord)
        => new Vector3(coord.x * cellSize, 0, coord.y * cellSize);

    public bool IsValidCell(Vector2Int coord)
        => coord.x >= 0 && coord.x < width && coord.y >= 0 && coord.y < height;

    public Cell GetCell(Vector2Int coord)
        => IsValidCell(coord) ? grid[coord.x, coord.y] : null;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for (int x = 0; x <= width; x++)
        {
            Vector3 from = new Vector3(x * cellSize, 0, 0);
            Vector3 to = new Vector3(x * cellSize, 0, height * cellSize);
            Gizmos.DrawLine(from, to);
        }
        for (int y = 0; y <= height; y++)
        {
            Vector3 from = new Vector3(0, 0, y * cellSize);
            Vector3 to = new Vector3(width * cellSize, 0, y * cellSize);
            Gizmos.DrawLine(from, to);
        }
    }

    public Vector3 GetWorldCenter(Vector2Int origin, Vector2Int size)
    {
        Vector3 bottomLeft = CellToWorld(origin);
        Vector3 halfExtents = new Vector3(size.x * cellSize, 0, size.y * cellSize) * 0.5f;
        return bottomLeft + halfExtents;
    }
}
