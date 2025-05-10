// BuildingInfoUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BuildingInfoUI : MonoBehaviour
{
    public static BuildingInfoUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text extraText;
    public Slider researchProgressBar;

    [Header("Research UI")]
    public RectTransform researchContainer;
    public Button researchButtonPrefab;

    private Coroutine currentResearch;
    private BuildingData researchTarget;
    private float researchElapsed;
    private Building currentBuilding;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panel.SetActive(false);
        if (researchProgressBar != null)
            researchProgressBar.gameObject.SetActive(false);
    }

    /// <summary>
    /// Affiche les informations et les options de recherche pour le bâtiment sélectionné.
    /// </summary>
    public void Show(Building b)
    {
        currentBuilding = b;
        var data = b.data;

        // Met à jour les textes
        titleText.text = data.name;
        descriptionText.text = data.description;
        extraText.text = string.Empty;

        // Barre de progression si recherche en cours
        if (researchProgressBar != null)
        {
            if (currentResearch != null && researchTarget != null)
            {
                researchProgressBar.maxValue = researchTarget.researchDuration;
                researchProgressBar.value = researchElapsed;
                researchProgressBar.gameObject.SetActive(true);
            }
            else
            {
                researchProgressBar.gameObject.SetActive(false);
            }
        }

        // Vider les anciens boutons
        foreach (Transform child in researchContainer)
            Destroy(child.gameObject);

        // Créer les boutons pour chaque bâtiment déblocable
        foreach (var toUnlock in data.unlocks ?? new List<BuildingData>())
        {
            bool unlocked = BuildingSelectionPanel.Instance.IsBuildingUnlocked(toUnlock);
            var btn = Instantiate(researchButtonPrefab, researchContainer);
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = $"Rechercher : {toUnlock.name}";

            // Gérer l'interactivité du bouton
            btn.interactable = !unlocked && currentResearch == null;
            ColorBlock cb = btn.colors;
            cb.disabledColor = new Color(0.5f, 0.5f, 0.5f);
            btn.colors = cb;

            // Tooltip au survol
            var trigger = btn.gameObject.AddComponent<EventTrigger>();
            var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener(_ => TooltipUI.Instance.Show(toUnlock.name, toUnlock.description, toUnlock.researchCost));
            trigger.triggers.Add(entryEnter);
            var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener(_ => TooltipUI.Instance.Hide());
            trigger.triggers.Add(entryExit);

            // Lancer la recherche
            if (!unlocked)
            {
                btn.onClick.AddListener(() => StartResearch(toUnlock));
            }
        }

        panel.SetActive(true);
    }

    /// <summary>
    /// Démarre la recherche d'un bâtiment.
    /// </summary>
    private void StartResearch(BuildingData toUnlock)
    {
        if (currentResearch != null) return;

        researchTarget = toUnlock;
        researchElapsed = 0f;

        if (researchProgressBar != null)
        {
            researchProgressBar.maxValue = toUnlock.researchDuration;
            researchProgressBar.value = 0f;
            researchProgressBar.gameObject.SetActive(true);
        }

        // Désactiver tous les boutons de recherche pendant la recherche
        foreach (Transform child in researchContainer)
        {
            var btnChild = child.GetComponent<Button>();
            if (btnChild != null)
                btnChild.interactable = false;
        }

        currentResearch = StartCoroutine(RunResearch());
    }

    /// <summary>
    /// Coroutine qui gère la progression de la recherche.
    /// </summary>
    private IEnumerator RunResearch()
    {
        while (researchElapsed < researchTarget.researchDuration)
        {
            researchElapsed += Time.deltaTime;
            if (researchProgressBar != null)
                researchProgressBar.value = researchElapsed;
            yield return null;
        }

        // Fin de la recherche
        BuildingSelectionPanel.Instance.UnlockBuilding(researchTarget);
        currentResearch = null;

        if (researchProgressBar != null)
            researchProgressBar.gameObject.SetActive(false);

        // Met à jour l'affichage si le panel est ouvert
        if (panel.activeSelf)
            Show(currentBuilding);
    }

    /// <summary>
    /// Ferme le panel d'information.
    /// </summary>
    public void Hide()
    {
        panel.SetActive(false);
    }
}