// Assets/Scripts/House.cs
using UnityEngine;
using System.Linq;

[ExecuteAlways]
[RequireComponent(typeof(Building))]
public class House : MonoBehaviour
{
    [Header("Connexion au marché")]
    [Tooltip("True si cette maison est dans le rayon d'un marché ET reliée par une route")]
    public bool isConnectedToMarket = false;

    Building _b;

    void OnEnable()
    {
        _b = GetComponent<Building>();
        UpdateConnection();
    }

    void Update()
    {
        UpdateConnection();
    }

    void UpdateConnection()
    {
        if (_b == null) return;

        // 1) trouve tous les marchés connectés par routes
        var connectedMarkets = _b.connected
                                .Select(other => other.GetComponent<Market>())
                                .Where(m => m != null);

        // 2) vérifie qu’au moins un marché est dans son actionRadius
        bool prev = isConnectedToMarket;
        isConnectedToMarket = connectedMarkets
            .Any(m => Vector3.Distance(m.transform.position, transform.position)
                      <= m.actionRadius);

        // 3) debug
        if (isConnectedToMarket != prev)
            Debug.Log($"{name}: connecté à un marché ? {isConnectedToMarket}");
    }
}
