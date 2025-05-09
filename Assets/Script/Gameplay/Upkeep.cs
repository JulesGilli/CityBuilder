// Assets/Scripts/Upkeep.cs
using UnityEngine;

/// <summary>
/// Composant g�n�rique pour payer un co�t r�gulier en ressource.
/// </summary>
[RequireComponent(typeof(Building))]
public class Upkeep : MonoBehaviour
{
    [Header("Param�tres d�entretien")]
    [Tooltip("Type de ressource � d�penser pour l�entretien")]
    public ResourceType resourceType = ResourceType.Gold;

    [Tooltip("Quantit� de ressource d�pens�e � chaque intervalle")]
    public int amountPerInterval = 2;

    [Tooltip("Intervalle (en secondes) entre chaque d�pense")]
    public float interval = 15f;

    [Tooltip("Si vrai, d�sactive ce composant en cas de fonds insuffisants")]
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
            Debug.LogWarning($"[Upkeep] {name} n�a pas pu payer {amountPerInterval} {resourceType} !");
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
