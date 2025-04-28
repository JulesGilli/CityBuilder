// Assets/Scripts/RoadData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "CityBuilder/City/RoadData", fileName = "RouteUnique")]
public class RoadData : ScriptableObject
{
    [Tooltip("Prefab 3D 1×1 de la route (cube aplati, plane, etc.)")]
    public GameObject prefab;
}
