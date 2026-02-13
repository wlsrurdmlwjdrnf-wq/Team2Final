using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestView : MonoBehaviour
{
    [SerializeField] private Button testButton;
    [SerializeField] private Button testButton2;
    [SerializeField] private Button testButton3;
    [SerializeField] private Button testButton4;
    [SerializeField] private Button testButton5;
    [SerializeField] private Button testButton6;
    [SerializeField] private Button testButton7;
    [SerializeField] private TextMeshProUGUI expRequirementText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI statPointText;
    [SerializeField] private TextMeshProUGUI goldText;
    

    public event Action OnAddGoldClicked;
    public event Action OnSpendGoldClicked;
    public event Action OnAtkPowerUpClicked;
    public event Action OnCritRateUpClicked;
    public event Action OnLevelUpClicked;
    public event Action OnBossStageClicked;
    public event Action OnTierStageClicked;

    private void Awake()
    {
        UpdateView();
        testButton.onClick.AddListener(() =>  OnAddGoldClicked?.Invoke());
        testButton2.onClick.AddListener(() =>  OnSpendGoldClicked?.Invoke());
        testButton3.onClick.AddListener(() =>  OnAtkPowerUpClicked?.Invoke());
        testButton4.onClick.AddListener(() =>  OnCritRateUpClicked?.Invoke());
        testButton5.onClick.AddListener(() =>  OnLevelUpClicked?.Invoke());
        testButton6.onClick.AddListener(() =>  OnBossStageClicked?.Invoke());
        testButton7.onClick.AddListener(() =>  OnTierStageClicked?.Invoke());

    }

    public void UpdateView()
    {
        expRequirementText.text = PlayerLevelUpSystem.GetExpRequirement();
        expText.text = PlayerResourceManager.Instance.GetFormatted(ResourceType.EXP);
        statPointText.text = "StatPoints : " + PlayerResourceManager.Instance.GetFormatted(ResourceType.StatPoint);
        goldText.text = "Gold : " + PlayerResourceManager.Instance.GetFormatted(ResourceType.Gold);
    }
}
