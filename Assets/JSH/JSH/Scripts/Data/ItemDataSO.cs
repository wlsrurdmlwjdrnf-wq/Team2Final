using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "GameData/Item/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public string Name;
    public EDataType Type;
    public ElementType Element;
    public GradeType Grade;
    public int Tier;
    public int Level;
    //장착효과
    public StatType EquipStat;
    public float EquipValue;
    //보유효과
    public StatType PassiveStat;
    public float PassiveValue;

    public float CriticalDMG;
    public float CriticalRate;
    public float GoldPer;

    public float EquipATKbyLv;
    public float PassiveATKbyLv;
    public float CriticalDMGbyLv;
    public float GoldPerbyLv;

    public DataSOType DataSO;
}
