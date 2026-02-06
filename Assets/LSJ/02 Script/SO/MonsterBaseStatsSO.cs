using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Monster/BaseStats", fileName = "MonsterBaseStats")]
public class MonsterBaseStatsSO : ScriptableObject
{
    public string monsterName;

    [Header("±‚∫ª Ω∫≈»")]
    public float baseAttackPower;
    public float baseMaxHP;
    public float baseDefensivePower;
    public float baseAttackSpeed;
}
