// Assets/Scripts/BuildingNeeds.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Building))]
public class BuildingNeeds : MonoBehaviour
{
    [Header("Besoins à vérifier")]
    public NeedType[] needs = new[] { NeedType.WarehouseAccess };

    [Header("Debug")]
    public List<NeedType> satisfiedNeeds = new List<NeedType>();

    Building _b;

    void Awake()
    {
        _b = GetComponent<Building>();
    }

    void Update()
    {
        satisfiedNeeds.Clear();

        foreach (var need in needs)
        {
            switch (need)
            {
                case NeedType.MarketAccess:
                    if (_b.connected.Any(x => x.GetComponent<Market>() is Market m &&
                         Vector3.Distance(m.transform.position, transform.position) <= m.actionRadius))
                        satisfiedNeeds.Add(need);
                    break;

                case NeedType.WarehouseAccess:
                    if (_b.connected.Any(x => x.GetComponent<Warehouse>() is Warehouse w &&
                         Vector3.Distance(w.transform.position, transform.position) <= w.actionRadius))
                        satisfiedNeeds.Add(need);
                    break;
            }
        }
    }

    public int SatisfiedCount => satisfiedNeeds.Count;
    public int TotalCount => needs.Length;
}
