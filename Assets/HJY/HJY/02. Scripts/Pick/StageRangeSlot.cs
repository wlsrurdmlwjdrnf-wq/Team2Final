
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageRangeSlot : MonoBehaviour
{
    [Header("스테이지")]
    [SerializeField] private TextMeshProUGUI stageNumberText;
    [SerializeField] private TextMeshProUGUI stageNameText;
    [SerializeField] private Button warpButton;

    //[SerializeField] private TextMeshProUGUI levelText;
    //[SerializeField] private Image elementIcon;
    //[SerializeField] private Image gradeIcon;

    [Header("잠금")]
    [SerializeField] private GameObject lockBlur;

    StageSO currentSO;

    public void Setup(StageSO stageSO)
    {
        currentSO = stageSO;

        stageNumberText.text = $"{stageSO.mainNumber}-{stageSO.subNumber}";
        // stageNameText.text = stageSO.stageName;
        // levelText.text = $"Lv.{stageSO.level}";

        // elementIcon.sprite = IconTable.GetElementIcon(stageSO.element);
        // gradeIcon.sprite = IconTable.GetGradeIcon(stageSO.grade);

        // 잠금 여부를 StageManager에서 확인
        // 잠금 여부
        // bool unlocked = StageManager.Instance.IsStageUnlocked(stageSO);
        // lockBlur.SetActive(!unlocked);

        // 버튼 이벤트 초기화 후 다시 바인딩
        warpButton.onClick.RemoveAllListeners();
        warpButton.onClick.AddListener(OnClickEnter);


    }

    public void OnClickEnter()
    {
        //if (!StageManager.Instance.IsStageUnlocked(currentSO)) return;

        StageManager.Instance.ApplyStage(currentSO);
    }
}
