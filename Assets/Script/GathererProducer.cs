using UnityEngine;

[RequireComponent(typeof(BuildingNeeds))]
public class GathererProducer : MonoBehaviour
{
    [Header("Production de r�colte")]
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
        // 1) La cabane doit �tre reli�e � un entrep�t pour stocker la r�colte
        if (_needs.SatisfiedCount == 0)
            return;

        // 2) (Optionnel) Si tu veux qu�elle soit plac�e pr�s d�un d�p�t de baies,
        //    tu peux reprendre HasNearbyDeposit() comme pour la carri�re.

        _timer -= Time.deltaTime;
        if (_timer > 0f)
            return;
        _timer += tickInterval;

        ResourceManager.Instance.Add(ResourceType.Harvest, harvestPerTick);
        Debug.Log($"{name} produit {harvestPerTick} r�colte");
    }
}
