// Assets/Scripts/HouseProducer.cs
using UnityEngine;

[RequireComponent(typeof(HouseNeeds))]
public class HouseProducer : MonoBehaviour
{
    [Header("Production")]
    [Tooltip("Or de base produit à chaque intervalle")]
    public int baseGoldPerTick = 5;
    [Tooltip("Bonus d’or par besoin satisfait")]
    public int bonusGoldPerNeed = 3;
    [Tooltip("Intervalle en secondes entre chaque production")]
    public float tickInterval = 10f;

    private HouseNeeds _needs;
    private float _timer;

    void Start()
    {
        _needs = GetComponent<HouseNeeds>();
        _timer = tickInterval;
    }

    void Update()
    {
        // 1) Si aucun besoin n'est satisfait, on ne produit rien
        if (_needs.SatisfiedCount == 0)
            return;

        // 2) Attente du prochain tick
        _timer -= Time.deltaTime;
        if (_timer > 0f)
            return;
        _timer += tickInterval;

        // 3) Calcul et ajout de l’or
        int satisfied = _needs.SatisfiedCount;
        int amount = baseGoldPerTick + satisfied * bonusGoldPerNeed;
        ResourceManager.Instance.Add(ResourceType.Gold, amount);

        Debug.Log($"{name} produit {amount} gold " +
                  $"(base {baseGoldPerTick} + {satisfied}×{bonusGoldPerNeed})");
    }
}
