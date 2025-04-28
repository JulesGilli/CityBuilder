// Assets/Scripts/HouseNeeds.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(House))]
public class HouseNeeds : MonoBehaviour
{
    [Header("Liste des besoins")]
    [Tooltip("Déclare ici tous les besoins de la maison")]
    public NeedType[] needs = new[] { NeedType.MarketAccess };

    [Header("Debug")]
    [Tooltip("Recalcule les besoins satisfaits en continu")]
    public List<NeedType> satisfiedNeeds = new List<NeedType>();

    House _house;

    void Awake()
    {
        _house = GetComponent<House>();
    }

    void Update()
    {
        satisfiedNeeds.Clear();

        foreach (var need in needs)
        {
            switch (need)
            {
                case NeedType.MarketAccess:
                    if (_house.isConnectedToMarket)
                        satisfiedNeeds.Add(need);
                    break;
                    // case NeedType.WaterSupply: ...
            }
        }
    }

    /// <summary>Nombre de besoins satisfaits</summary>
    public int SatisfiedCount => satisfiedNeeds.Count;

    /// <summary>Nombre total de besoins</summary>
    public int TotalCount => needs.Length;
}
