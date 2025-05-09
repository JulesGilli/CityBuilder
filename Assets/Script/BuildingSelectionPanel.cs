// BuildingSelectionPanel.cs
using UnityEngine;
using System.Collections.Generic;

public class BuildingSelectionPanel : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform contentParent;

    [Header("Building Icons")]
    public GameObject buildingIconButtonPrefab;
    public List<BuildingData> buildingDatas;

    [Header("Road Icons")]
    public GameObject roadIconButtonPrefab;
    public List<RoadData> roadDatas;

    void Start()
    {
        // Instantiate building buttons
        foreach (var data in buildingDatas)
        {
            var go = Instantiate(buildingIconButtonPrefab, contentParent);
            var btn = go.GetComponent<BuildingIconButton>();
            btn.Initialize(data);
        }

        // Instantiate road buttons
        foreach (var data in roadDatas)
        {
            var go = Instantiate(roadIconButtonPrefab, contentParent);
            var btn = go.GetComponent<RoadIconButton>();
            btn.Initialize(data);
        }


    }
}