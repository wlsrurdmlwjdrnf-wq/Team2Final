
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RegionButton : MonoBehaviour
{
    [System.Serializable]
    public class RegionUI
    {
        public Button button;       // 지역 버튼
        public Image iconImage;     // 잠금 표시 아이콘
        public bool isUnlocked;     // 초기 잠금 여부
    }

    [Header("지역 순서")]
    [SerializeField] private List<RegionUI> regions; // 지역 목록

    [Header("잠금 설정")]
    [SerializeField] private Sprite lockedIcon;                 // 잠금 표시 아이콘
    [SerializeField] private Color lockedColor = Color.black;   // 잠금 아이콘 색상
    

    [Header("잠금 해제")]
    [SerializeField] private Sprite unlockedIcon;                 // 잠금 해제 아이콘
    [SerializeField] private Color unlockedColor = Color.white;   // 잠금 해제 아이콘 색상

    [Header("정보 연결")]
    [SerializeField] private MapUI mapUI;                         // 맵 스크립트

    // 버튼 이벤트를 저장해둘 리스트
    private List<UnityAction> clickActions = new List<UnityAction>();

    private void Awake()
    {
        // 초기화
        clickActions.Clear();

        for (int i = 0; i < regions.Count; i++)
        {
            int index = i; // 클로저 안전 변수

           // 액션을 변수에 저장!!!
            UnityAction action = () => OnClick(index);

            clickActions.Add(action);

            regions[i].button.onClick.AddListener(action);

            UpdateVisual(index);
        }
    }

    // 액션 제거
    private void OnDestroy()
    {
        for (int i = 0; i < regions.Count; i++)
        {
            if (i < clickActions.Count)
            {
                regions[i].button.onClick.RemoveListener(clickActions[i]);
            }
        }
    }

    // 지역 이름 버튼 클릭
    private void OnClick(int index)
    {
        if (!regions[index].isUnlocked)
            return;

        // 잠금 해제면 퀘스트 패널 열기
        mapUI.OpenQuest(index);
    }

    // 잠금 해제
    public void SetUnlock(int index, bool unlock)
    {
        if (index < 0 || index >= regions.Count)
            return;

        regions[index].isUnlocked = unlock;
        UpdateVisual(index);
    }

    // 잠금 표시 아이콘 갱신
    private void UpdateVisual(int index)
    {
        if (regions[index].isUnlocked)
        {
            regions[index].iconImage.sprite = unlockedIcon;
            regions[index].iconImage.color = unlockedColor;

            // 버튼 활성화
            regions[index].button.interactable = true;
        }
        else
        {
            regions[index].iconImage.sprite = lockedIcon;
            regions[index].iconImage.color = lockedColor;

            // 버튼 비활성화
            regions[index].button.interactable = false;
        }
    }
}