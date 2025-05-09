using UnityEngine;

[RequireComponent(typeof(BuildingNeeds))]
public class QuarryProducer : MonoBehaviour
{
    [Header("Production de pierre")]
    public int stonePerTick = 5;
    public float tickInterval = 8f;

    private BuildingNeeds _needs;
    private float _timer;
    private GridManager _grid;

    void Start()
    {
        _needs = GetComponent<BuildingNeeds>();
        _timer = tickInterval;

        // Récupère le gridManager via le PlacementManager global :contentReference[oaicite:4]{index=4}:contentReference[oaicite:5]{index=5}
        var placement = Object.FindFirstObjectByType<PlacementManager>();
        _grid = placement.gridManager;
    }

    void Update()
    {
        // 1) Doit être reliée à un entrepôt
        if (_needs.SatisfiedCount == 0) return;
        // 2) Doit être à côté d'un gisement
        if (!HasNearbyDeposit()) return;

        _timer -= Time.deltaTime;
        if (_timer > 0f) return;
        _timer += tickInterval;

        ResourceManager.Instance.Add(ResourceType.Stone, stonePerTick);
        Debug.Log($"{name} extrait {stonePerTick} pierre");
    }

    private bool HasNearbyDeposit()
    {
        var b = GetComponent<Building>();
        var origin = b.origin;  // champ existant dans Building :contentReference[oaicite:6]{index=6}:contentReference[oaicite:7]{index=7}
        var size = b.size;

        // Balaye bordure d'1 cellule autour de l'emprise
        for (int dx = -1; dx <= size.x; dx++)
            for (int dy = -1; dy <= size.y; dy++)
            {
                var cellPos = origin + new Vector2Int(dx, dy);
                if (!_grid.IsValidCell(cellPos)) continue;
                var occ = _grid.GetCell(cellPos).occupant;
                if (occ != null && occ.GetComponent<MineDeposit>() != null)
                    return true;
            }
        return false;
    }
}
