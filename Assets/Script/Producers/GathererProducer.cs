using UnityEngine;

[RequireComponent(typeof(BuildingNeeds))]
public class GathererProducer : MonoBehaviour
{
    [Header("Production de récolte")]
    public int harvestPerTick = 3;
    public float tickInterval = 6f;

    private BuildingNeeds _needs;
    private float _timer;

    void Start()
    {
        _needs = GetComponent<BuildingNeeds>();
        _timer = tickInterval;
    }

    void Update()
    {
        // 1) La cabane doit être reliée à un entrepôt pour stocker la récolte
        if (_needs.SatisfiedCount == 0)
            return;

        // 2) (Optionnel) Si tu veux qu’elle soit placée près d’un dépôt de baies,
        //    tu peux reprendre HasNearbyDeposit() comme pour la carrière.

        _timer -= Time.deltaTime;
        if (_timer > 0f)
            return;
        _timer += tickInterval;

        ResourceManager.Instance.Add(ResourceType.Harvest, harvestPerTick);
        Debug.Log($"{name} produit {harvestPerTick} récolte");
    }
}
