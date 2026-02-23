using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Monster/BaseStats", fileName = "MonsterBaseStats")]
public class MonsterBaseStatsSO : ScriptableObject
{
    public string monsterName;

    [Header("기본 스탯")]
    public float baseAttackPower;   // 공격력
    public float baseMaxHP;         // 최대 체력
    public float baseDefensivePower; // 방어력
    public float baseAttackSpeed;   // 공격 속도 (보스몬스터만 유효)

    public ElementType elementType; // 속성 (모험 모드에서만 유효)
}
