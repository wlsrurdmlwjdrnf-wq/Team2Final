
using UnityEngine;

public class TabBar : MonoBehaviour
{
    public RectTransform contentTr;   // 패널들 부모
    public float panelWidth = 720f;   // 패널 너비

    private void Start()
    {
        // 앱 실행 직후 Content의 위치를 (0,0)으로 설정
        contentTr.anchoredPosition = Vector2.zero; 
    }

    public void ShowUpgrade()
    {
        contentTr.anchoredPosition = new Vector2(0, 0);
    }

    public void ShowGrowth()
    {
        contentTr.anchoredPosition = new Vector2(-panelWidth, 0);
    }

    public void ShowTier()
    {
        contentTr.anchoredPosition = new Vector2(-panelWidth * 2, 0);
    }
}