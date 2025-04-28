// Assets/Scripts/LumberjackProducer.cs
using UnityEngine;

[RequireComponent(typeof(BuildingNeeds))]
public class LumberjackProducer : MonoBehaviour
{
    [Header("Production de bois")]
    public int woodPerTick = 5;
    public float tickInterval = 8f;

    private BuildingNeeds _needs;
    private float _timer;

    void Start()
    {
        _needs = GetComponent<BuildingNeeds>();
        _timer = tickInterval;
    }

    void Update()
    {
        // Ne produit que si le besoin WarehouseAccess est satisfait
        if (_needs.SatisfiedCount == 0)
            return;

        _timer -= Time.deltaTime;
        if (_timer > 0f) return;
        _timer += tickInterval;

        ResourceManager.Instance.Add(ResourceType.Wood, woodPerTick);
        Debug.Log($"{name} récolte {woodPerTick} bois");
    }
}
