using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Player/BaseStats", fileName = "PlayerBaseStats")]
public class PlayerBaseStatsSO : ScriptableObject
{
    [Header("기본 정보")]
    public int baseLevel = 1;
    public Tier baseTier = Tier.Stone;

    [Header("기본 스탯")]
    public float baseAttackPower = 10f;     // 공격력
    public float baseMaxHP = 100f;          // 최대 체력
    public float baseHPRegenPerSec = 1f;     // 초당 회복 ( % 아님)
    public float baseCritRate = 0.05f;       // 5%
    public float baseCritDamage = 1.5f;     // 150%
    public float baseMaxMana = 100f;        // 최대 마나
    public float baseManaRegenPerSec = 1f;    // 초당 회복 ( % 아님) 
    public float baseGoldMultiplier = 1f;     // 추가 골드 획득량 배율
    public float baseExpMultiplier = 1f;      // 추가 경험치 배율
    public float baseAttackSpeed = 1f;       // 공격속도 ( 1 / 공속 = 공격쿨타임)
    public float baseMoveSpeed = 4f;       // 이동속도 (플레이어가 이동하는 것이 아니므로 배경 스크롤과 몬스터 이동 속도에 영향)

}