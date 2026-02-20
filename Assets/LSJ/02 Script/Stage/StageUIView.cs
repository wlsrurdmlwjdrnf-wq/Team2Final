using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageUIView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageNumberText;
    [SerializeField] private TextMeshProUGUI stageNameText;

    [SerializeField] private Button bossStageButton;

    [SerializeField] private GameObject stageBar;

    public event Action OnBossStageButtonClicked;

    private void Awake()
    {
        bossStageButton.onClick.AddListener(() =>  OnBossStageButtonClicked?.Invoke());
        StageManager.Instance.OnStageChanged += UpdateStageNumber;
        StageManager.Instance.OnStageChanged += ShowStageBar;
        StageManager.Instance.OnBossStageChanged += HideStageBar;
        StageManager.Instance.OnTierStageChanged += HideStageBar;
        StageManager.Instance.OnAdventureStageChanged += HideStageBar;
    }

    public void UpdateStageNumber()
    {
        stageNumberText.text = $"{StageManager.Instance.CurrentMainNumber} - {StageManager.Instance.CurrentSubNumber}";
        stageNameText.text = "일반 스테이지";
    }
    public void ShowStageBar()
    {
        stageBar.SetActive(true);
    }
    public void HideStageBar()
    {
        stageBar.SetActive(false);
    }
    private void OnDestroy()
    {
        if (StageManager.Instance == null) return;
        StageManager.Instance.OnStageChanged -= UpdateStageNumber;
        StageManager.Instance.OnStageChanged -= ShowStageBar;
        StageManager.Instance.OnBossStageChanged -= HideStageBar;
        StageManager.Instance.OnTierStageChanged -= HideStageBar;
        StageManager.Instance.OnAdventureStageChanged -= HideStageBar;
    }
}
