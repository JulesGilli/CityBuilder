// Assets/Scripts/RoadData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "CityBuilder/City/RoadData", fileName = "RouteUnique")]
public class RoadData : ScriptableObject
{
    [Tooltip("Prefab 3D 1×1 de la route (cube aplati, plane, etc.)")]
    public GameObject prefab;

    [Header("UI Settings")]
    [Tooltip("Icône affichée dans le panel de sélection")]
    public Sprite icon;

    [Tooltip("Nom affiché dans le tooltip")]
    public string displayName;

    [Tooltip("Description (facultative) dans le tooltip")]
    [TextArea]
    public string description;

    [Tooltip("Coûts éventuels de construction (laisser vide si gratuit)")]
    public ResourceAmount[] constructionCost;
}
