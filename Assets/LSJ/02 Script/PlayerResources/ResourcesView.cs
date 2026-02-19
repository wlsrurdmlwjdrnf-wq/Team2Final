using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI statPointText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI emeraldText;
    [SerializeField] private TextMeshProUGUI diamondText;
    [SerializeField] private TextMeshProUGUI enhancementCubeText;
    [SerializeField] private TextMeshProUGUI fireStoneText;
    [SerializeField] private TextMeshProUGUI waterStoneText;
    [SerializeField] private TextMeshProUGUI windStoneText;
    [SerializeField] private TextMeshProUGUI earthStoneText;
    [SerializeField] private TextMeshProUGUI featherText;

    private Dictionary<ResourceType, TextMeshProUGUI> _textMap = new();

    private void Awake()
    {
        if (expText) _textMap[ResourceType.EXP] = expText;
        if (statPointText) _textMap[ResourceType.StatPoint] = statPointText;
        if (goldText) _textMap[ResourceType.Gold] = goldText;
        if (diamondText) _textMap[ResourceType.Diamond] = diamondText;
        if (emeraldText) _textMap[ResourceType.Emerald] = emeraldText;
        if (enhancementCubeText) _textMap[ResourceType.EnhancementCube] = enhancementCubeText;
        if (fireStoneText) _textMap[ResourceType.FireStone] = fireStoneText;
        if (waterStoneText) _textMap[ResourceType.WaterStone] = waterStoneText;
        if (windStoneText) _textMap[ResourceType.WindStone] = windStoneText;
        if (earthStoneText) _textMap[ResourceType.EarthStone] = earthStoneText;
        if (featherText) _textMap[ResourceType.Feather] = featherText;
    }

    public void UpdateResourceDisplay(ResourceType type, string formattedValue)
    {
        if (_textMap.TryGetValue(type, out var text))
        {
            text.text = formattedValue;
        }
    }
}
