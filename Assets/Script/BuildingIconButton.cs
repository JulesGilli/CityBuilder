using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class BuildingIconButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;
    private BuildingData _data;
    private InputManager _input;

    void Awake()
    {
        _input = Object.FindFirstObjectByType<InputManager>();
        if (_input == null)
            Debug.LogError("InputManager introuvable dans la scène !");
    }

    public void Initialize(BuildingData buildingData)
    {
        _data = buildingData;
        iconImage.sprite = _data.icon;

        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            // On délègue au InputManager
            _input.StartBuildingPlacement(_data);
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Instance.Show(
            _data.name,
            _data.description,
            _data.constructionCost
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }
}
