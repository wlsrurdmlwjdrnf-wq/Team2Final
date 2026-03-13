
using UnityEngine;
using UnityEngine.UI;

public class TabBar : MonoBehaviour
{
    [Header("탭 버튼")]
    [SerializeField] private Button enhanceBtn;    // 강화 버튼
    [SerializeField] private Button growthBtn;     // 성장 버튼
    [SerializeField] private Button upgradeBtn;    // 승급 버튼

    [Header("탭 페이지")]
    [SerializeField] private GameObject enhancePage;   // 강화 페이지
    [SerializeField] private GameObject growthPage;    // 성장 페이지
    [SerializeField] private GameObject upgradePage;   // 승급 페이지


    [Header("플레이어 정보")]
    [SerializeField] private GameObject playerExp;  // 플레이어 경험치
    [SerializeField] private GameObject playerTier; // 플레이어 티어

    void Start()
    {
        // 버튼 클릭 이벤트
        enhanceBtn.onClick.AddListener(() => TabClick(enhancePage, true));  // 강화 버튼을 누르면 플레이어 경험치가 뜸
        growthBtn.onClick.AddListener(() => TabClick(growthPage, true));    // 성장 버튼을 누르면 플레이어 경험치가 뜸
        upgradeBtn.onClick.AddListener(() => TabClick(upgradePage, false)); // 승급 버튼을 누르면 플레이어 티어가 뜸

        // 기본 페이지는 강화 페이지임
        TabClick(enhancePage, true);
    }

    // 강화 혹은 성장 혹은 승급 버튼을 누르면
    void TabClick(GameObject activePage, bool isExp)
    {
        // 모든 페이지 비활성화 후 선택된 페이지만 활성화하라.
        enhancePage.SetActive(activePage == enhancePage);
        growthPage.SetActive(activePage == growthPage);
        upgradePage.SetActive(activePage == upgradePage);

        // 버튼에 따라 플레이어 경험치 혹은 플레이어의 티어를 보여라.
        playerExp.SetActive(isExp);
        playerTier.SetActive(!isExp);
    }
}