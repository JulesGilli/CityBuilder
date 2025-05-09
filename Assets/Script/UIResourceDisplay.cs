using UnityEngine;
using TMPro;

/// <summary>
/// Affiche en temps réel les ressources (or, bois, pierre, récolte) dans l'UI TextMeshPro.
/// Place ce script sur un GameObject UI (ex: "ResourcePanel").
/// Dans l'inspector, lie les TMP_Text correspondants.
/// </summary>
public class UIResourceDisplay : MonoBehaviour
{
    [Header("UI TextMeshPro References")]
    [Tooltip("TMP Text pour afficher l'or")]
    public TMP_Text goldText;

    [Tooltip("TMP Text pour afficher le bois")]
    public TMP_Text woodText;

    [Tooltip("TMP Text pour afficher la pierre")]
    public TMP_Text stoneText;

    [Tooltip("TMP Text pour afficher la récolte")]
    public TMP_Text harvestText;

    void Update()
    {
        if (ResourceManager.Instance == null)
            return;

        int gold = ResourceManager.Instance.Get(ResourceType.Gold);
        int wood = ResourceManager.Instance.Get(ResourceType.Wood);
        int stone = ResourceManager.Instance.Get(ResourceType.Stone);
        int harvest = ResourceManager.Instance.Get(ResourceType.Harvest);

        if (goldText != null)
            goldText.text = $"Gold: {gold}";

        if (woodText != null)
            woodText.text = $"Wood: {wood}";

        if (stoneText != null)
            stoneText.text = $"Stone: {stone}";

        if (harvestText != null)
            harvestText.text = $"Harvest: {harvest}";
    }
}
