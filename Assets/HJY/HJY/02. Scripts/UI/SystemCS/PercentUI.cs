
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PercentUI : MonoBehaviour
{
    // 무기 및 악세서리용 ScriptableObject
    [Header("데이터 연결")]
    [SerializeField] private PercentSO weaponTable;
    [SerializeField] private PercentSO accessoryTable;

    [Header("패널 연결")]
    [SerializeField] private GameObject summonPanel; // 무기 및 악세 공용 패널
    [SerializeField] private GameObject skillPanel;  // 스킬 전용 패널

    [Header("소환 패널 요소")]
    [SerializeField] private TextMeshProUGUI equipmentType;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI common;
    [SerializeField] private TextMeshProUGUI uncommon;
    [SerializeField] private TextMeshProUGUI rare;
    [SerializeField] private TextMeshProUGUI epic;
    [SerializeField] private TextMeshProUGUI legendary;
    [SerializeField] private TextMeshProUGUI mythic;

    [Header("버튼 연결")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;


    private PercentSO currentTable;
    private int currentLevel = 1;


    void Start()
    {
        // 시작 시에는 패널을 모두 꺼둠
        summonPanel.SetActive(false);
        skillPanel.SetActive(false);
    }

    // 무기 버튼 이벤트
    public void ShowWeapon()
    {
        summonPanel.SetActive(true);
        skillPanel.SetActive(false);
        SetSummonType(weaponTable);
    }

    // 악세 버튼 이벤트
    public void ShowAccessory()
    {
        summonPanel.SetActive(true);
        skillPanel.SetActive(false);
        SetSummonType(accessoryTable);
    }

    // 스킬 버튼 이벤트 
    public void ShowSkill()
    {
        summonPanel.SetActive(false);
        skillPanel.SetActive(true);
    }

    // 닫기 버튼을 누르면 패널을 닫아라.
    public void ClosePanel()
    {
        summonPanel.SetActive(false);
        skillPanel.SetActive(false);
    }

    // 소환 타입 설정
    public void SetSummonType(PercentSO summon)
    {
        currentTable = summon;
        currentLevel = 1;
        UpdateUI();
    }

    // 무기 및 악세 UI 갱신
    public void UpdateUI()
    {
        var rate = currentTable.rates[currentLevel - 1];

        if (currentTable.equipmentType == EquipmentType.Weapon) equipmentType.text = "무기";
        else if (currentTable.equipmentType == EquipmentType.Accessory) equipmentType.text = "악세";

        level.text = $"소환 레벨 {rate.level}";
        common.text = $"{rate.common}%";
        uncommon.text = $"{rate.uncommon}%";
        rare.text = $"{rate.rare}%";
        epic.text = $"{rate.epic}%";
        legendary.text = $"{rate.legendary}%";
        mythic.text = $"{rate.mythic}%";

    
        // 오른쪽 버튼은 레벨 1~9에서만 보임
        rightButton.gameObject.SetActive(currentLevel < 10);

        // 왼쪽 버튼은 레벨 2~10에서만 보임
        leftButton.gameObject.SetActive(currentLevel > 1);

    }

    // 오른쪽 버튼 클릭
    public void OnClickRight()
    {
        if (currentLevel < 10)
        {
            currentLevel++;
            UpdateUI();
        }
    }

    // 왼쪽 버튼 클릭
    public void OnClickLeft()
    {
        if (currentLevel > 1)
        {
            currentLevel--;
            UpdateUI();
        }
    }
}