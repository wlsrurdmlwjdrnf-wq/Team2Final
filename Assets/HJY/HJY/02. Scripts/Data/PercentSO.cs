
using UnityEngine;

// 소환 레벨에 따른 무기 및 악세 확률
public enum EquipmentType
{
    Weapon,
    Accessory
}

[System.Serializable]
public class SummonRate
{
    public int level;        // 소환 레벨
    public float common;     // 일반
    public float uncommon;   // 고급
    public float rare;       // 레어
    public float epic;       // 영웅
    public float legendary;  // 전설
    public float mythic;     // 신화
}

[CreateAssetMenu(fileName = "PercentSO", menuName = "Gacha/PercentSO")]
public class PercentSO : ScriptableObject
{
    // 장비 종류
    public EquipmentType equipmentType;

    // 장비 소환 레벨
    public SummonRate[] rates;
}