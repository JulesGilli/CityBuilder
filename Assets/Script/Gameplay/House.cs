// Assets/Scripts/House.cs
using UnityEngine;
using System.Linq;

[ExecuteAlways]
[RequireComponent(typeof(Building))]
public class House : MonoBehaviour
{
    [Header("Connexion au march�")]
    [Tooltip("True si cette maison est dans le rayon d'un march� ET reli�e par une route")]
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

        // 1) trouve tous les march�s connect�s par routes
        var connectedMarkets = _b.connected
                                .Select(other => other.GetComponent<Market>())
                                .Where(m => m != null);

        // 2) v�rifie qu�au moins un march� est dans son actionRadius
        bool prev = isConnectedToMarket;
        isConnectedToMarket = connectedMarkets
            .Any(m => Vector3.Distance(m.transform.position, transform.position)
                      <= m.actionRadius);

        // 3) debug
        if (isConnectedToMarket != prev)
            Debug.Log($"{name}: connect� � un march� ? {isConnectedToMarket}");
    }
}
