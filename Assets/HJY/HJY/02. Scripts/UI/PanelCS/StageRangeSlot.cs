
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageRangeSlot : RecycleScrollSlot<StageSO>
{
    [Header("스테이지")]
    [SerializeField] private TextMeshProUGUI stageNumberText;   // 스테이지 번호 텍스트
    [SerializeField] private TextMeshProUGUI stageNameText;     // 스테이지 이름 텍스트
    [SerializeField] private Button warpButton;                 // 스테이지 이동 버튼

    [Header("잠금")]
    [SerializeField] private GameObject lockBlur;              // 잠금 상태 표시로 블러 처리를 해주는 오브젝트

    private StageSO currentSO;                                 // 스테이지 데이터

    [SerializeField] private float width = 520f;               // 슬롯의 가로 크기
    [SerializeField] private float height = 100f;              // 슬롯의 세로 크기


    // 슬롯의 크기를 반환!!!
    public override float Width => width;
    public override float Height => height;
    

    // 슬롯 데이터 갱신
    public override void UpdateSlot(StageSO stageSO)
    {
        currentSO = stageSO;

        stageNumberText.text = $"{stageSO.mainNumber}-{stageSO.subNumber}";
        stageNameText.text = stageSO.name; 

        warpButton.onClick.RemoveAllListeners();
        warpButton.onClick.AddListener(OnClickEnter);

        // 안 열린 스테이지 프리팹 블러로 처리하기
        if(stageSO.mainNumber < StageManager.Instance.GetRecord().bestMainNumber)
            lockBlur.SetActive(false);
        else if(stageSO.mainNumber == StageManager.Instance.GetRecord().bestMainNumber
            && stageSO.subNumber <= StageManager.Instance.GetRecord().bestSubNumber)
            lockBlur.SetActive(false);
        else lockBlur.SetActive(true);

    }

    // 해당 스테이지로 이동
    private void OnClickEnter()
    {
        // StageManager를 통해 해당 스테이지 적용
        StageManager.Instance.ApplyStage(currentSO);
    }
}