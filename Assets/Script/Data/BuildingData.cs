// Assets/Scripts/BuildingData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "CityBuilder/BuildingData", fileName = "NewBuildingData")]
public class BuildingData : ScriptableObject
{
    [Tooltip("Dimension en cellules")]
    public Vector2Int size = Vector2Int.one;

    [Header("Prefab (fallback)")]
    [Tooltip("Le prefab 3D du b�timent (centr� sur son origine)")]
    public GameObject prefab;

    [Header("Optional Variants (random selection)")]
    [Tooltip("Si non vide, une de ces variantes sera instanci�e au hasard")]
    public GameObject[] prefabVariants;

    [Header("Co�t de construction")]
    public ResourceAmount[] constructionCost;
}
