using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    public GridManager gridManager;

    private List<Building> buildings = new List<Building>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        foreach (var b in FindObjectsOfType<Building>())
            RegisterBuilding(b);
    }

    /// <summary>Appelé après placement d'un bâtiment</summary>
    public void RegisterBuilding(Building b)
    {
        if (!buildings.Contains(b))
        {
            buildings.Add(b);
            RecalculateAllConnections();
        }
    }

    /// <summary>Appelé après placement d'une route</summary>
    public void RegisterRoad()
    {
        RecalculateAllConnections();
    }

    private void RecalculateAllConnections()
    {
        foreach (var b in buildings)
        {
            b.connected.Clear();
            var reachable = FindReachableBuildings(b)
                            .Where(x => x != b)
                            .ToList();
            b.connected.AddRange(reachable);

            // Debug : affiche la liste des connexions
            if (reachable.Count > 0)
            {
                var names = string.Join(", ", reachable.Select(x => x.gameObject.name));
            }
            else
            {
            }
        }
    }

    private List<Building> FindReachableBuildings(Building start)
    {
        var visited = new HashSet<Vector2Int>();
        var queue = new Queue<Vector2Int>();
        var found = new HashSet<Building>();

        // Enqueue des routes adjacentes
        foreach (var cell in start.OccupiedCells())
            foreach (var dir in Neighbors4())
            {
                var adj = cell + dir;
                if (gridManager.IsValidCell(adj)
                    && gridManager.GetCell(adj).type == CellType.Road
                    && visited.Add(adj))
                {
                    queue.Enqueue(adj);
                }
            }

        // BFS sur le réseau routier
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();

            // Check bâtiments voisins
            foreach (var dir in Neighbors4())
            {
                var adj = c + dir;
                if (!gridManager.IsValidCell(adj)) continue;

                var cell = gridManager.GetCell(adj);
                if (cell.type == CellType.Building)
                {
                    var other = cell.occupant.GetComponent<Building>();
                    if (other != null)
                        found.Add(other);
                }
            }

            // Explore routes voisines
            foreach (var dir in Neighbors4())
            {
                var adj = c + dir;
                if (gridManager.IsValidCell(adj)
                    && gridManager.GetCell(adj).type == CellType.Road
                    && visited.Add(adj))
                {
                    queue.Enqueue(adj);
                }
            }
        }

        return found.ToList();
    }

    private static IEnumerable<Vector2Int> Neighbors4()
    {
        yield return Vector2Int.up;
        yield return Vector2Int.right;
        yield return Vector2Int.down;
        yield return Vector2Int.left;
    }
}
