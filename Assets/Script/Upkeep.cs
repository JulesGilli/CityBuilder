// Assets/Scripts/Upkeep.cs
using UnityEngine;

/// <summary>
/// Composant générique pour payer un coût régulier en ressource.
/// </summary>
[RequireComponent(typeof(Building))]
public class Upkeep : MonoBehaviour
{
    [Header("Paramètres d’entretien")]
    [Tooltip("Type de ressource à dépenser pour l’entretien")]
    public ResourceType resourceType = ResourceType.Gold;

    [Tooltip("Quantité de ressource dépensée à chaque intervalle")]
    public int amountPerInterval = 2;

    [Tooltip("Intervalle (en secondes) entre chaque dépense")]
    public float interval = 15f;

    [Tooltip("Si vrai, désactive ce composant en cas de fonds insuffisants")]
    public bool disableOnFailure = false;

    private float _timer;

    void Start()
    {
        _timer = interval;
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer > 0f) return;
        _timer += interval;  // reset du chrono

        bool ok = ResourceManager.Instance.Spend(resourceType, amountPerInterval);
        if (!ok)
        {
            Debug.LogWarning($"[Upkeep] {name} n’a pas pu payer {amountPerInterval} {resourceType} !");
            if (disableOnFailure)
                enabled = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Affiche un petit cube ou sphere pour identifier les batis avec upkeep
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
