// BuildingData.cs
using System.Collections.Generic;
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

    [Header("Icon de la construction")]
    public Sprite icon;

    [Header("Description de la construction")]
    public string description;

    [Header("Research")]
    [Tooltip("B�timents � d�bloquer via cette �nigme")]
    public List<BuildingData> unlocks;

    [Header("Research Cost")]
    [Tooltip("Co�t en ressources pour lancer la recherche")]
    public ResourceAmount[] researchCost;

    [Header("Research Duration")]
    [Tooltip("Dur�e de recherche en secondes")]
    public float researchDuration = 5f;
}