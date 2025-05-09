using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(House))]
public class HouseNeeds : MonoBehaviour
{
    [Header("Liste des besoins")]
    [Tooltip("Déclare ici tous les besoins de la maison")]
    public NeedType[] needs = new[] { NeedType.MarketAccess, NeedType.HarvestSupply };

    [Header("Paramètres de consommation")]
    [Tooltip("Quantité de récolte consommée à chaque intervalle")]
    public int harvestPerTick = 1;
    [Tooltip("Intervalle (en secondes) entre deux consommations de récolte")]
    public float harvestInterval = 10f; // ← 10 ticks = 10 s

    [Header("Debug")]
    [Tooltip("Recalcule les besoins satisfaits en continu")]
    public List<NeedType> satisfiedNeeds = new List<NeedType>();

    private House _house;
    private float _harvestTimer;

    void Awake()
    {
        _house = GetComponent<House>();
        _harvestTimer = harvestInterval;
    }

    void Update()
    {
        // 1) On décrémente le timer
        _harvestTimer -= Time.deltaTime;

        satisfiedNeeds.Clear();

        foreach (var need in needs)
        {
            switch (need)
            {
                case NeedType.MarketAccess:
                    if (_house.isConnectedToMarket)
                        satisfiedNeeds.Add(need);
                    break;

                case NeedType.HarvestSupply:
                    // On ne tente la consommation QUE quand le timer est écoulé
                    if (_harvestTimer <= 0f)
                    {
                        if (ResourceManager.Instance.Has(ResourceType.Harvest, harvestPerTick))
                        {
                            ResourceManager.Instance.Consume(ResourceType.Harvest, harvestPerTick);
                            satisfiedNeeds.Add(need);
                        }
                        // on réarme le timer quoi qu'il arrive, pour repartir sur un nouveau cycle
                        _harvestTimer = harvestInterval;
                    }
                    break;

                    // … autres besoins éventuels
            }
        }
    }

    /// <summary>Nombre de besoins satisfaits</summary>
    public int SatisfiedCount => satisfiedNeeds.Count;

    /// <summary>Nombre total de besoins</summary>
    public int TotalCount => needs.Length;
}
