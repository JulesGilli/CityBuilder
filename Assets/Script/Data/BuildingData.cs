// Assets/Scripts/BuildingData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "CityBuilder/BuildingData", fileName = "NewBuildingData")]
public class BuildingData : ScriptableObject
{
    [Tooltip("Dimension en cellules")]
    public Vector2Int size = Vector2Int.one;

    [Header("Prefab (fallback)")]
    [Tooltip("Le prefab 3D du bâtiment (centré sur son origine)")]
    public GameObject prefab;

    [Header("Optional Variants (random selection)")]
    [Tooltip("Si non vide, une de ces variantes sera instanciée au hasard")]
    public GameObject[] prefabVariants;

    [Header("Coût de construction")]
    public ResourceAmount[] constructionCost;
}
