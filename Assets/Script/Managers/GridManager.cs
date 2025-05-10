// GridManager.cs
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width, height;
    public float cellSize = 1f;

    private Cell[,] grid;
    public static GridManager Instance { get; private set; }

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize grid data
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
    {
        return new Vector3(coord.x * cellSize, 0f, coord.y * cellSize);
    }

    public bool IsValidCell(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x < width && coord.y >= 0 && coord.y < height;
    }

    public Cell GetCell(Vector2Int coord)
    {
        return IsValidCell(coord) ? grid[coord.x, coord.y] : null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for (int x = 0; x <= width; x++)
        {
            Gizmos.DrawLine(new Vector3(x * cellSize, 0f, 0f), new Vector3(x * cellSize, 0f, height * cellSize));
        }
        for (int y = 0; y <= height; y++)
        {
            Gizmos.DrawLine(new Vector3(0f, 0f, y * cellSize), new Vector3(width * cellSize, 0f, y * cellSize));
        }
    }
}