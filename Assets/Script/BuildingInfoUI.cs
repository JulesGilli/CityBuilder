// BuildingInfoUI.cs
// Affiche les informations d'un b�timent s�lectionn�
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingInfoUI : MonoBehaviour
{
    public static BuildingInfoUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;            // Panel racine, d�sactiv� par d�faut
    public TMP_Text titleText;          // Nom du b�timent
    public TMP_Text descriptionText;    // Description depuis BuildingData
    public TMP_Text extraText;          // Informations dynamiques

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        panel.SetActive(false);
    }

    /// <summary>
    /// Affiche le panel avec les infos du b�timent donn�.
    /// </summary>
    public void Show(Building b)
    {
        var data = b.data;
        titleText.text = data.name;            // utilise data.name
        descriptionText.text = data.description;

        // R�initialisation
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
                $"Taille : {b.size.x}�{b.size.y}\n" +
                $"Coordonn�e : ({b.origin.x},{b.origin.y})";
        }

        panel.SetActive(true);
    }

    /// <summary>Cache le panel.</summary>
    public void Hide()
    {
        panel.SetActive(false);
    }
}
