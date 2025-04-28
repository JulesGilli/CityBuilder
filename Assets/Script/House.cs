// Assets/Scripts/House.cs
using UnityEngine;
using System.Linq;

[ExecuteAlways]
[RequireComponent(typeof(Building))]
public class House : MonoBehaviour
{
    [Header("Connexion au march�")]
    [Tooltip("True si cette maison est reli�e � au moins un march� via les routes")]
    public bool isConnectedToMarket = false;

    Building _b;

    void OnEnable()
    {
        _b = GetComponent<Building>();
        UpdateConnection();
    }

    void Update()
    {
        // En Edit comme en Play, on met � jour la valeur du bool�en
        UpdateConnection();
    }

    void UpdateConnection()
    {
        if (_b == null) return;
        // On regarde la liste des b�timents connect�s
        // et on v�rifie si l'un d'eux poss�de un component Market
        isConnectedToMarket = _b.connected
            .Any(other => other.GetComponent<Market>() != null);
    }
}
