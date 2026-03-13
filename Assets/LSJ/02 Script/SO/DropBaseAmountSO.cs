using UnityEngine;

// 1-1 기준 몬스터 기본 드롭 재화량
[CreateAssetMenu(menuName = "GameData/Drop/AmountData",fileName = "DropBaseAmountData")]
public class DropBaseAmountSO : ScriptableObject
{
    public float baseExpAmount = 10f;   // 경험치
    public float baseGoldAmount = 10f;  // 골드
    public float baseEnhancementCubeAmount = 10f;   // 강화큐브
    public float baseElementalStone = 1f;   // 속성석
}
