// TooltipUI.cs
using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    [Header("UI References")]
    public RectTransform background;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text costText;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        background.gameObject.SetActive(false);
    }

    /// <summary>
    /// Affiche le tooltip � la position d�finie en Inspector
    /// </summary>
    public void Show(string title, string desc, ResourceAmount[] costs)
    {
        // Met � jour le texte
        nameText.text = title;
        descriptionText.text = desc;
        costText.text = string.Empty;
        foreach (var c in costs)
            costText.text += $"{c.resourceType}: {c.amount}\n";

        // Active et repositionne � l'emplacement initial
        background.gameObject.SetActive(true);
    }

    /// <summary>
    /// Cache le tooltip
    /// </summary>
    public void Hide()
    {
        background.gameObject.SetActive(false);
    }
}