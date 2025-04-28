// Assets/Scripts/Market.cs
using UnityEngine;

[RequireComponent(typeof(Building))]
public class Market : MonoBehaviour
{
    [Tooltip("Portée maximale (en unités world) pour desservir une maison")]
    public float actionRadius = 10f;

    void OnDrawGizmosSelected()
    {
        // Affiche la portée dans l’éditeur
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, actionRadius);
    }
}
