// Assets/Scripts/Warehouse.cs
using UnityEngine;

[RequireComponent(typeof(Building))]
public class Warehouse : MonoBehaviour
{
    [Tooltip("Portée (en unités monde) pour desservir un producteur")]
    public float actionRadius = 8f;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, actionRadius);
    }
}
