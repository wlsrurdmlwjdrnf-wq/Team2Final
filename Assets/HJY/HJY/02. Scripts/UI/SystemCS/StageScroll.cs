
using System;
using UnityEngine;

public class StageScroll : RecycleStageScroll<StageSO>
{
    [SerializeField] private StageRangeSlot stageSlotPrefab; // 슬롯 프리팹

    public Action<int> OnMainNumberChanged;                 // 현재 보이는 스테이지의 메인 넘버가 바뀌었는지를 알려주는 이벤트

    private int currentMainNumber = -1;                     // 현재 화면 기준으로 메인 넘버를 저장 -> 중복 호출 방지!!!

    
    // 스크롤이 움직였다면 호출
    protected override void OnScroll(Vector2 scrollPosition)
    {
        // RecycleStageScroll의 재사용 슬롯 이동 로직이 실행되면
        base.OnScroll(scrollPosition);

        // 현재 보이는 메인 넘버를 체크해라.
        UpdateCurrentMainNumber();
    }

    // 현재 보이는 메인 넘버를 감지하기
    private void UpdateCurrentMainNumber()
    {
        if (dataList == null || dataList.Count == 0)
            return;

        // 위치 계산
        float contentY = contentRect.anchoredPosition.y;
        int firstVisibleRowIndex = Mathf.Max(0, Mathf.FloorToInt(contentY / (itemHeight + spacing)));

        // 데이터를 인덱스로 변환
        int firstVisibleIndex = firstVisibleRowIndex * itemsPerRow;

        // 인덱스가 유효 범위 안에 있다면
        if (firstVisibleIndex >= 0 && firstVisibleIndex < dataList.Count)
        {
            // 해당 슬롯의 메인 넘버를 가져와라.
            int newMainNumber = dataList[firstVisibleIndex].mainNumber;

            // 이전과 값이 다르다면 이벤트가 발생함
            if (newMainNumber != currentMainNumber)
            {
                currentMainNumber = newMainNumber;

                // 변경된 걸 StagePickPanel에다 알리기
                OnMainNumberChanged?.Invoke(currentMainNumber);
            }
        }
    }

    // 버튼을 누르면 특정 메인 넘버 위치로 스크롤이 이동함
    public void ScrollToMainNumber(int mainNumber)
    {
        // 해당 메인 넘버를 가진 첫 번째 데이터 인덱스를 찾아라.
        int index = dataList.FindIndex(s => s.mainNumber == mainNumber);
        if (index < 0)
            return;

        // 위치 계산
        int row = index / itemsPerRow;

        float targetY = row * (itemHeight + spacing);
        float contentHeight = contentRect.sizeDelta.y;
        float viewportHeight = scrollRect.viewport.rect.height;

        float normalized = 1f - Mathf.Clamp01(targetY / (contentHeight - viewportHeight));

        // 스크롤 위치를 이동해라.
        scrollRect.verticalNormalizedPosition = normalized;
    }
}