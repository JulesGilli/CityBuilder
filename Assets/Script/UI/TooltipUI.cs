// TooltipUI.cs
using UnityEngine;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }
    public RectTransform background;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text costText;

    [Tooltip("Margin offset from the mouse for tooltip placement")]
    public Vector2 margin = new Vector2(10f, 10f);

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        background.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!background.gameObject.activeSelf) return;

        Vector2 mousePos = Input.mousePosition;
        Vector2 pivot = new Vector2(
            mousePos.x > Screen.width * 0.5f ? 1f : 0f,
            mousePos.y > Screen.height * 0.5f ? 1f : 0f
        );
        background.pivot = pivot;

        Vector2 offset = new Vector2(
            pivot.x == 1f ? -margin.x : margin.x,
            pivot.y == 1f ? -margin.y : margin.y
        );
        Vector2 anchoredPosition = mousePos + offset;
        anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, 0, Screen.width);
        anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, 0, Screen.height);

        background.position = anchoredPosition;
    }

    public void Show(string title, string desc, ResourceAmount[] costs)
    {
        nameText.text = title;
        descriptionText.text = desc;
        costText.text = string.Empty;
        foreach (var c in costs)
            costText.text += $"{c.resourceType}: {c.amount}\n";

        background.gameObject.SetActive(true);
    }

    public void Hide()
    {
        background.gameObject.SetActive(false);
    }
}
