using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Road : MonoBehaviour
{
    [HideInInspector] public RoadData data;
    [HideInInspector] public Vector2Int coord;  // La cellule o� se trouve ce tron�on
}
