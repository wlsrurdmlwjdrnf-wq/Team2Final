

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StagePickPanel : MonoBehaviour
{
    [Header("지역")]
    [SerializeField] private StageViewSO stageViewSO;          // 지역 이미지용 데이터
    [SerializeField] private TextMeshProUGUI regionText;       // 지역 이름 텍스트
    [SerializeField] private Image stageImage;                 // 지역 이미지

    [Header("버튼")]
    [SerializeField] private Button leftButton;               // 이전 지역 버튼
    [SerializeField] private Button rightButton;              // 다음 지역 버튼
    [SerializeField] private Button closeButton;              // 패널 닫기 버튼

    [Header("스크롤")]
    [SerializeField] private StageScroll stageScroll;         // 재사용 스크롤 뷰

    private int currentMainNumber = 1;                        // 현재 메인 넘버
    private int maxMainNumber = 10;                           // 최대 메인 넘버


    // 이벤트 등록
    private void OnEnable()
    {
        leftButton.onClick.AddListener(OnClickLeft);
        rightButton.onClick.AddListener(OnClickRight);
        closeButton.onClick.AddListener(ClosePanel);

        // 스크롤에서 메인 넘버가 바뀌면 호출됨
        stageScroll.OnMainNumberChanged += HandleMainNumberChanged;
    }

    // 이벤트 해제
    private void OnDisable()
    {
        leftButton.onClick.RemoveListener(OnClickLeft);
        rightButton.onClick.RemoveListener(OnClickRight);
        closeButton.onClick.RemoveListener(ClosePanel);

        stageScroll.OnMainNumberChanged -= HandleMainNumberChanged;
    }

    // 패널을 열어라.
    public void OpenPanel(List<StageSO> stageList)
    {
        gameObject.SetActive(true);

        // 전체 지역 수 갱신
        maxMainNumber = stageViewSO.stages.Length;

        // 스크롤 초기화
        stageScroll.Init(stageList);

        // 지역 UI 갱신
        RefreshRegionUI(currentMainNumber);
    }

    // 패널을 닫아라.
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    // 왼쪽 버튼 클릭
    private void OnClickLeft()
    {
        if (currentMainNumber <= 1)
            return;

        currentMainNumber--;
        stageScroll.ScrollToMainNumber(currentMainNumber);
        RefreshRegionUI(currentMainNumber);
    }

    // 오른쪽 버튼 클릭
    private void OnClickRight()
    {
        if (currentMainNumber >= maxMainNumber)
            return;

        currentMainNumber++;
        stageScroll.ScrollToMainNumber(currentMainNumber);
        RefreshRegionUI(currentMainNumber);
    }

    // 메인 넘버 변경 이벤트
    private void HandleMainNumberChanged(int mainNumber)
    {
        currentMainNumber = mainNumber;
        RefreshRegionUI(mainNumber);
    }

    // 지역 UI의 이름 및 이미지를 갱신해라.
    private void RefreshRegionUI(int mainNumber)
    {
        int index = mainNumber - 1;

        if (index >= 0 && index < stageViewSO.stages.Length)
        {
            StageView viewData = stageViewSO.stages[index];

            regionText.text = viewData.stageName;
            stageImage.sprite = viewData.stageImage;
        }

        // 버튼 활성화 여부 갱신
        UpdateButtonState();
    }

    // 좌우 버튼 상태 갱신
    private void UpdateButtonState()
    {
        // 왼쪽 버튼은 1보다 크면 활성화하고
        leftButton.gameObject.SetActive(currentMainNumber > 1);

        // 오른쪽 버튼은 10보다 작으면 활성화해라.
        rightButton.gameObject.SetActive(currentMainNumber < maxMainNumber);
    }
}