// BuildingInfoUI.cs
// Affiche les informations d'un bâtiment sélectionné
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingInfoUI : MonoBehaviour
{
    public static BuildingInfoUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;            // Panel racine, désactivé par défaut
    public TMP_Text titleText;          // Nom du bâtiment
    public TMP_Text descriptionText;    // Description depuis BuildingData
    public TMP_Text extraText;          // Informations dynamiques

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        panel.SetActive(false);
    }

    /// <summary>
    /// Affiche le panel avec les infos du bâtiment donné.
    /// </summary>
    public void Show(Building b)
    {
        var data = b.data;
        titleText.text = data.name;            // utilise data.name
        descriptionText.text = data.description;

        // Réinitialisation
        extraText.text = string.Empty;

        // Si c'est une maison, afficher les besoins
        var houseNeeds = b.GetComponent<HouseNeeds>();
        if (houseNeeds != null)
        {
            extraText.text =
                $"Besoins satisfaits : {houseNeeds.SatisfiedCount}/{houseNeeds.TotalCount}";
        }
        else
        {
            // Autres types : afficher la taille et la position
            extraText.text =
                $"Taille : {b.size.x}×{b.size.y}\n" +
                $"Coordonnée : ({b.origin.x},{b.origin.y})";
        }

        panel.SetActive(true);
    }

    /// <summary>Cache le panel.</summary>
    public void Hide()
    {
        panel.SetActive(false);
    }
}
