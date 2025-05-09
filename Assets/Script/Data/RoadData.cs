// Assets/Scripts/RoadData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "CityBuilder/City/RoadData", fileName = "RouteUnique")]
public class RoadData : ScriptableObject
{
    [Tooltip("Prefab 3D 1�1 de la route (cube aplati, plane, etc.)")]
    public GameObject prefab;

    [Header("UI Settings")]
    [Tooltip("Ic�ne affich�e dans le panel de s�lection")]
    public Sprite icon;

    [Tooltip("Nom affich� dans le tooltip")]
    public string displayName;

    [Tooltip("Description (facultative) dans le tooltip")]
    [TextArea]
    public string description;

    [Tooltip("Co�ts �ventuels de construction (laisser vide si gratuit)")]
    public ResourceAmount[] constructionCost;
}
