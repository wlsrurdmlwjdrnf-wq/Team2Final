using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureView : MonoBehaviour
{
    [SerializeField] private Button[] stageButtons;

    // + 클리어 보상 팝업, 텍스트

    public event Action<int> OnAdventureStageButtonClicked;

    private void Awake()
    {
        ButtonsAddListener();

        LockAdventureStages(); // 모든 모험 스테이지 버튼 잠금으로 초기화

        StageManager.Instance.OnAllMonstersCleared += UpdateEnableStage;
    }
    private void Start()
    {
        UpdateEnableStage();
    }

    private void ButtonsAddListener()
    {
        if (stageButtons.Length == 0) return;

        int index = 1;
        foreach (var button in stageButtons)
        {
            button.onClick.AddListener(() => OnAdventureStageButtonClicked?.Invoke(index));
            index++;
        }
    }
    private void LockAdventureStages()
    {
        foreach (var button in stageButtons)
        {
            button.interactable = false;
        }
    }
    public void UpdateEnableStage()
    {
        for(int i = 0; i <= StageManager.Instance.GetRecord().bestAdventureNumber; i++)
        {
            stageButtons[i].interactable = true;
        }   
    }

    private void OnDestroy()
    {
        if (StageManager.Instance == null) return;
        StageManager.Instance.OnAllMonstersCleared -= UpdateEnableStage;
    }
}
