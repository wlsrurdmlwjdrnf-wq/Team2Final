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
    public float EquipATK;
    public float PassiveATK;
    public float CriticalDMG;
    public float CriticalRate;
    public float GoldPer;
    public DataSOType DataSO;
}
