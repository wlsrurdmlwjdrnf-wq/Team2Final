using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageUIView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageNumberText;
    [SerializeField] private TextMeshProUGUI stageNameText;

    [SerializeField] private Button bossStageButton;
    [SerializeField] private Button exitStageButton;

    [SerializeField] private GameObject[] needHideObjects;

    public event Action OnBossStageButtonClicked;
    public event Action OnExitStageButtonClicked;

    // TODO : 일반 스테이지 이동 기능

    private void Awake()
    {
        bossStageButton.onClick.AddListener(() =>  OnBossStageButtonClicked?.Invoke());
        exitStageButton.onClick.AddListener(() => OnExitStageButtonClicked?.Invoke());

        StageManager.Instance.OnStageChanged += UpdateStageNumber;
        StageManager.Instance.OnStageChanged += SetTrueObjects;
        StageManager.Instance.OnBossStageChanged += SetFalseObjects;
        StageManager.Instance.OnTierStageChanged += SetFalseObjects;
        StageManager.Instance.OnAdventureStageChanged += SetFalseObjects;

        StageManager.Instance.OnStageChanged += HideExitButton;
        StageManager.Instance.OnBossStageChanged += ShowExitButton;
        StageManager.Instance.OnTierStageChanged += ShowExitButton;
        StageManager.Instance.OnAdventureStageChanged += ShowExitButton;

    }

    public void UpdateStageNumber()
    {
        if (StageManager.Instance.CurrentStageData.isBossStage)
        {
            stageNumberText.text = $"{StageManager.Instance.CurrentMainNumber} - {StageManager.Instance.CurrentSubNumber}";
            stageNameText.text = "보스 스테이지";
        }
        else if (StageManager.Instance.CurrentStageData.isTierStage)
        {
            stageNumberText.text = (PlayerStatManager.Instance.PlayerTier + 1).ToString();
            stageNameText.text = "승급 스테이지";
        }
        else if (StageManager.Instance.CurrentStageData.isAdventureStage)
        {
            stageNumberText.text = "";
            stageNameText.text = "모험 스테이지";
        }
        else
        {
            stageNumberText.text = $"{StageManager.Instance.CurrentMainNumber} - {StageManager.Instance.CurrentSubNumber}";
            stageNameText.text = "일반 스테이지";
        }
    }
    public void SetTrueObjects()
    {
        foreach (GameObject go in needHideObjects)
        {
            go.SetActive(true);
        }
    }
    public void SetFalseObjects()
    {
        foreach (GameObject go in needHideObjects)
        {
            go.SetActive(false);
        }
    }
    public void ShowExitButton()
    {
        exitStageButton.gameObject.SetActive(true);
    }
    public void HideExitButton()
    {
        exitStageButton.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        if (StageManager.Instance == null) return;
        StageManager.Instance.OnStageChanged -= UpdateStageNumber;
        StageManager.Instance.OnStageChanged -= SetTrueObjects;
        StageManager.Instance.OnBossStageChanged -= SetFalseObjects;
        StageManager.Instance.OnTierStageChanged -= SetFalseObjects;
        StageManager.Instance.OnAdventureStageChanged -= SetFalseObjects;

        StageManager.Instance.OnStageChanged -= HideExitButton;
        StageManager.Instance.OnBossStageChanged -= ShowExitButton;
        StageManager.Instance.OnTierStageChanged -= ShowExitButton;
        StageManager.Instance.OnAdventureStageChanged -= ShowExitButton;
    }
}
