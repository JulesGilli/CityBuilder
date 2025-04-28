// Assets/Scripts/House.cs
using UnityEngine;
using System.Linq;

[ExecuteAlways]
[RequireComponent(typeof(Building))]
public class House : MonoBehaviour
{
    [Header("Connexion au marché")]
    [Tooltip("True si cette maison est reliée à au moins un marché via les routes")]
    public bool isConnectedToMarket = false;

    Building _b;

    void OnEnable()
    {
        _b = GetComponent<Building>();
        UpdateConnection();
    }

    void Update()
    {
        // En Edit comme en Play, on met à jour la valeur du booléen
        UpdateConnection();
    }

    void UpdateConnection()
    {
        if (_b == null) return;
        // On regarde la liste des bâtiments connectés
        // et on vérifie si l'un d'eux possède un component Market
        isConnectedToMarket = _b.connected
            .Any(other => other.GetComponent<Market>() != null);
    }
}
