using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdventureView : MonoBehaviour
{
    [SerializeField] private Button[] stageButtons;
    [SerializeField] private Button stageStartButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelFailPanelButton;

    [SerializeField] private GameObject selectPanel;
    [SerializeField] private GameObject stageClearPanel;
    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private TextMeshProUGUI diaRewardText;
    [SerializeField] private TextMeshProUGUI emeraldRewardText;
    [SerializeField] private TextMeshProUGUI artifactRewardText;

    public event Action<int, Button> OnAdventureStageButtonClicked;
    public event Action OnAdventureStageStartButtonClicked;
    public event Action OnConfirmButtonClicked;
    public event Action OnCancelButtonClicked;

    private bool _isAdventure = false;

    private void Awake()
    {
        ButtonsAddListener();
        stageStartButton.onClick.AddListener(() => OnAdventureStageStartButtonClicked?.Invoke());
        //confirmButton.onClick.AddListener(() => OnConfirmButtonClicked?.Invoke());
        //cancelFailPanelButton.onClick.AddListener(() => OnCancelButtonClicked?.Invoke());

        LockAdventureStages(); // 모든 모험 스테이지 버튼 잠금으로 초기화

        StageManager.Instance.OnAllMonstersCleared += UpdateEnableStage;
        StageManager.Instance.OnAdventureStageChanged += ToggleIsAdventure;
        StageManager.Instance.OnAllMonstersCleared += ShowClearPanel;
        StageManager.Instance.OnGameOver += ShowFailPanel;
    }
    private void Start()
    {
        UpdateEnableStage();  // 데이터를 받아와 가능한 모험 스테이지 오픈
    }

    private void ButtonsAddListener()
    {
        if (stageButtons.Length == 0) return;

        int index = 1;
        foreach (var button in stageButtons)
        {
            int captureIndex = index;
            Button captureButton = button;
            button.onClick.AddListener(() => OnAdventureStageButtonClicked?.Invoke(captureIndex, captureButton));
            index++;
        }
    }
    private void LockAdventureStages()
    {
        Debug.Log("모험스테이지 잠금!");
        foreach (var button in stageButtons)
        {
            button.interactable = false;
        }
    }
    public void UpdateEnableStage()
    {
        for(int i = 0; i <= StageManager.Instance.GetRecord().bestAdventureNumber; i++)
        {
            if (stageButtons.Length == i) return;
            stageButtons[i].interactable = true;
        }   
    }
    public void UpdateStageInfo(int number)
    {
        diaRewardText.text = (StageManager.ADVENTURE_BASE_DIAMOND_AMOUNT * number).ToString();
        emeraldRewardText.text = (StageManager.ADVENTURE_BASE_EMERALD_AMOUNT * number).ToString();
    }
    public void HideSelectAdventurePanel()
    {
        selectPanel.SetActive(false);
    }
    public void ShowClearPanel()
    {
        if (!_isAdventure) return;
        stageClearPanel.SetActive(true);
        ToggleIsAdventure();
    }
    public void HideClearPanel()
    {
        stageClearPanel.SetActive(false);
    }
    private void ToggleIsAdventure()
    {
        _isAdventure = !_isAdventure;
    }
    private void ShowFailPanel()
    {
        if (!_isAdventure) return;
        gameOverPanel.SetActive(true);
        ToggleIsAdventure();
    }
    public void HideFailPanel()
    {
        gameOverPanel.SetActive(false);
    }
    private void OnDestroy()
    {
        if (StageManager.Instance == null) return;
        StageManager.Instance.OnAllMonstersCleared -= UpdateEnableStage;
        StageManager.Instance.OnAdventureStageChanged -= ToggleIsAdventure;
        StageManager.Instance.OnAllMonstersCleared -= ShowClearPanel;
        StageManager.Instance.OnGameOver -= ShowFailPanel;
    }
}
