
using UnityEngine;

public class StandardBar : MonoBehaviour
{
    [Header("보상 상단 바")]
    [SerializeField] private GameObject standardBar; // 자동 스킬 사용 버튼과 시간제 보상 버튼이 있는 상단 바


    void Start()
    {
        // 시작할 때 기본적으로 켜기
        UpdateTopBar(0);

    }

    // ScrollParent에게 index를 받아서 켜고 끄기
    public void UpdateTopBar(int index)
    {
        // 캐릭터 창 == 0 || 스킬 창 == 1
        if (index == 0 || index == 1)   
            standardBar.SetActive(true);
        else
            standardBar.SetActive(false);
    }

}
