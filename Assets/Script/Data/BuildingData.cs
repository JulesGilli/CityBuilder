using UnityEngine;

[CreateAssetMenu(menuName = "CityBuilder/BuildingData", fileName = "NewBuildingData")]
public class BuildingData : ScriptableObject
{
    [Tooltip("Dimension en cellules")]
    public Vector2Int size = Vector2Int.one;

    [Tooltip("Le prefab 3D du bâtiment (centré sur son origine)")]
    public GameObject prefab;
}
