
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [Header("퀘스트 패널")]
    [SerializeField] private GameObject questPanel;            // 퀘스트 패널
    [SerializeField] private Image regionImage;                // 지역 이미지
    [SerializeField] private TextMeshProUGUI regionText;       // 지역 이름
    [SerializeField] private Button closeButton;               // 패널 닫기 버튼

    [Header("퀘스트 성공 여부")]
    [SerializeField] private GameObject clearPanel;            // 퀘스트 성공 패널
    [SerializeField] private GameObject failPanel;             // 퀘스트 실패 패널

    [Header("스테이지 패널")]
    [SerializeField] private StagePickPanel stagePickPanel;   // 스테이지 선택 패널

    [Header("데이터")]
    [SerializeField] private StageViewSO stageViewSO;  // 지역 이름 및 이미지 데이터
    [SerializeField] private List<StageSO> stageList;  // 스테이지 데이터 리스트
    


    private void OnEnable()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseQuest);
    }

    private void OnDisable()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseQuest);
    }

    // 퀘스트 패널을 열어라.
    public void OpenQuest(int mainNumber)
    {
        if (questPanel == null || stageViewSO == null) return;

        // 패널 활성화
        questPanel.SetActive(true);

        // 지역 UI 갱신
        int index = mainNumber - 1;
        if (index >= 0 && index < stageViewSO.stages.Length)
        {
            StageView viewData = stageViewSO.stages[index];

            if (regionText != null)
                regionText.text = viewData.stageName;

            if (regionImage != null)
                regionImage.sprite = viewData.stageImage;
        }
    }

    public void CloseQuest()
    {
        if (questPanel != null)
            questPanel.SetActive(false);
    }


   // 클리어 패널을 열어라.
    public void OpenClear() => clearPanel?.SetActive(true);
    
    // 클리어 패널을 닫아라.
    public void CloseClear() => clearPanel?.SetActive(false);

    // 실패 패널을 열어라,
    public void OpenFail() => failPanel?.SetActive(true);

    // 실패 패널을 닫아라.
    public void CloseFail() => failPanel?.SetActive(false);


   // 스테이지 선택 패널을 열어라.
    public void OpenStage()
    {
        if (stagePickPanel != null)
            stagePickPanel.OpenPanel(stageList);
    }
}