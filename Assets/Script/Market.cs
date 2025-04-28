// Assets/Scripts/Market.cs
using UnityEngine;

[RequireComponent(typeof(Building))]
public class Market : MonoBehaviour
{
    [Tooltip("Port�e maximale (en unit�s world) pour desservir une maison")]
    public float actionRadius = 10f;

    void OnDrawGizmosSelected()
    {
        // Affiche la port�e dans l��diteur
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, actionRadius);
    }
}
