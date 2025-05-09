// RoadIconButton.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class RoadIconButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;
    private RoadData _data;
    private InputManager _input;

    void Awake()
    {
        _input = Object.FindFirstObjectByType<InputManager>();
        if (_input == null)
            Debug.LogError("InputManager not found in scene.");
    }

    public void Initialize(RoadData data)
    {
        _data = data;
        // Ensure iconImage is assigned
        if (iconImage == null)
        {
            iconImage = GetComponentInChildren<Image>();
            if (iconImage == null)
                Debug.LogError("RoadIconButton: Icon Image reference missing!");
        }

        if (_data.icon != null)
            iconImage.sprite = _data.icon;
        else
            Debug.LogWarning($"RoadData '{_data.name}' has no icon assigned.");

        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        if (_input != null)
            button.onClick.AddListener(() => _input.SetRoadMode(_data));
        else
            Debug.LogError("RoadIconButton: InputManager reference is null!");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Instance.Show(
        _data.displayName,
        _data.description,
        _data.constructionCost
            );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }
}