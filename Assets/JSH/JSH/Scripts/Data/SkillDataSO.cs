using UnityEngine;

[CreateAssetMenu(fileName = "SkillDataSO", menuName = "GameData/Skill/SkillDataSO")]
public class SkillDataSO : ScriptableObject
{
    public string Name;
    public EDataType Type;
    public ElementType Element;
    public GradeType Grade;
    public int Level;
    public DataSOType DataSO;
    //스킬타입은 스킬마다 고유하게 하나씩
    public ESkillEffectType SkillType;
    //액티브 > 쿨타임 혹은 평타 횟수 발동형
    public float CoolTime;
    public float ManaCost;
    public int TriggerCount;

    public float Range;
    public float Damage;
    public float CriticalRate;
    public float CriticalDMG;
    //패시브 > 스탯증가류
    public StatType Stat;
    public float ModifyAmount;
}
