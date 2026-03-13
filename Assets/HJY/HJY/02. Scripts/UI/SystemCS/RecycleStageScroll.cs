
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class RecycleStageScroll<T> : MonoBehaviour
{
    [Header("연결할 요소")]
    [SerializeField] protected ScrollRect scrollRect;             // 스크롤할 영역
    [SerializeField] protected RectTransform contentRect;         // 아이템이 배치될 content
    [SerializeField] protected RecycleScrollSlot<T> slotPrefab;   // 재사용할 슬롯 프리팹

    [Space]
    [Header("슬롯 설정")]
    [SerializeField] protected int bufferCount = 5; // 추가적으로 미리 만들어둘 위와 아래의 슬롯 개수
    [SerializeField] protected float spacing;       // 아이템 간의 간격

    [Space]
    [Header("세로 스크롤 설정")]
    [SerializeField] protected int itemsPerRow = 1;    // 한 줄에 배치될 아이템 수
    [SerializeField] protected float topOffset;        // 스크롤 뷰의 위쪽 여백
    [SerializeField] protected float bottomOffset;     // 스크롤 뷰의 아래쪽 여백
    [SerializeField] protected float horizontalOffset; // 좌측 여백 -> 좌측 정렬임!!!

    // 현재 생성되어 있는 슬롯 풀 -> 재사용을 위해 LinkedList 사용함!!!
    protected LinkedList<RecycleScrollSlot<T>> slotList = new LinkedList<RecycleScrollSlot<T>>();

    // 데이터 목록
    protected List<T> dataList = new List<T>();
    public IReadOnlyList<T> DataList => dataList;

    // 슬롯 크기
    protected float itemHeight;
    protected float itemWidth;

    // 생성할 슬롯 개수
    protected int poolSize;

    // 현재 화면에 첫 번째로 보이는 인덱스
    protected int tmpfirstVisibleIndex;

    // 화면에 실제로 보여질 수 있는 슬롯 개수
    protected int contentVisibleSlotCount;


    // 초기 설정 (초기화)
    public virtual void Init(List<T> dataLsit)
    {
        dataList = dataLsit;

        RectTransform scrollRectTransform = scrollRect.GetComponent<RectTransform>();

        // 슬롯 프리팹의 크기를 가져오기!!!
        itemHeight = slotPrefab.Height;
        itemWidth = slotPrefab.Width;

        // 전체 높이 계산
        int totalRows = Mathf.CeilToInt((float)dataList.Count / itemsPerRow);
        float contentHeight = itemHeight * totalRows + ((totalRows - 1) * spacing) + topOffset + bottomOffset;

        // Anchor 값 위쪽으로 고정
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.anchorMin = new Vector2(0f, 1f);

        // contentRect의 높이 계산
        contentVisibleSlotCount = (int)(scrollRectTransform.rect.height / itemHeight) * itemsPerRow;
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, contentHeight);

        // 슬롯 생성 및 리스트에 추가
        poolSize = contentVisibleSlotCount + (bufferCount * 2 * itemsPerRow);
        int index = -bufferCount * itemsPerRow;
        for (int i = 0; i < poolSize; i++)
        {
            RecycleScrollSlot<T> item = Instantiate(slotPrefab, contentRect);
            slotList.AddLast(item);
            item.Init();

            // 슬롯에 해당하는 데이터 연결
            UpdateSlot(item, index++);
        }

        // 스크롤 이벤트
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    // 화면 갱신
    public void Refresh()
    {
        UpdateData(dataList);
    }

    // 데이터 갱신
    public void UpdateData(List<T> dataList)
    {
        int index = tmpfirstVisibleIndex - bufferCount * itemsPerRow;
        foreach (RecycleScrollSlot<T> item in slotList)
        {
            UpdateSlot(item, index);
            index++;
        }
    }

    // 스크롤이 움직이면 호출하기
    protected virtual void OnScroll(Vector2 scrollPosition)
    {
        float contentY = contentRect.anchoredPosition.y;

        int firstVisibleRowIndex = Mathf.Max(0, Mathf.FloorToInt(contentY / (itemHeight + spacing))); 
        int firstVisibleIndex = firstVisibleRowIndex * itemsPerRow;                                   

        // 인덱스가 바뀐 상태라면 재배치
        if (tmpfirstVisibleIndex != firstVisibleIndex)
        {
            int diffIndex = (tmpfirstVisibleIndex - firstVisibleIndex) / itemsPerRow;

            // 위로 스크롤
            if (diffIndex < 0) 
            {
                int lastVisibleIndex = tmpfirstVisibleIndex + contentVisibleSlotCount;
                for (int i = 0, cnt = Mathf.Abs(diffIndex) * itemsPerRow; i < cnt; i++)
                {
                    RecycleScrollSlot<T> item = slotList.First.Value;
                    slotList.RemoveFirst();
                    slotList.AddLast(item);

                    int newIndex = lastVisibleIndex + (bufferCount * itemsPerRow) + i;
                    UpdateSlot(item, newIndex);
                }
            }

            // 아래로 스크롤
            else if (diffIndex > 0) 
            {
                for (int i = 0, cnt = Mathf.Abs(diffIndex) * itemsPerRow; i < cnt; i++)
                {
                    RecycleScrollSlot<T> item = slotList.Last.Value;
                    slotList.RemoveLast();
                    slotList.AddFirst(item);

                    int newIndex = tmpfirstVisibleIndex - (bufferCount * itemsPerRow) - i;
                    UpdateSlot(item, newIndex);
                }
            }

            tmpfirstVisibleIndex = firstVisibleIndex;
        }
    }

    // 슬롯 정보 갱신
    protected virtual void UpdateSlot(RecycleScrollSlot<T> item, int index)
    {
        // 슬롯 위치 계산
        int row = 0 <= index ? index / itemsPerRow : (index - 1) / itemsPerRow;
        int column = Mathf.Abs(index) % itemsPerRow;

        Vector2 pivot = item.RectTransform.pivot;
        float totalWidth = (itemsPerRow * (itemWidth + spacing)) - spacing;
        float contentWidth = contentRect.rect.width;
        float adjustedY = -(row * (itemHeight + spacing)) - itemHeight * (1 - pivot.y);  // 위에서 아래로 배치
        float adjustedX = column * (itemWidth + spacing) + itemWidth * pivot.x;          // 왼쪽을 기준으로 계산
        adjustedX += horizontalOffset;
        adjustedY -= topOffset;
        item.RectTransform.localPosition = new Vector3(adjustedX, adjustedY, 0);

        // 데이터 범위를 벗어났다면 비활성화해라.
        if (index < 0 || index >= dataList.Count)
        {
            item.gameObject.SetActive(false);
            return;
        }
        // 안 벗어났다면 해당 인덱스의 데이터를 연결해라.
        else
        {
            item.UpdateSlot(dataList[index]);
            item.gameObject.SetActive(true);
        }
    }
}