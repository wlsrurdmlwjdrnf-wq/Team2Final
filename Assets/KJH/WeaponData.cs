using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[System.Serializable]
public class WeaponData : BaseData
{    
    public string name;
    public EDataType type;
    public ElementType element;
    public GradeType grade;
    public int level;
    public float equipATK;
    public float passiveATK;
    public float criticalDMG;
    public float criticalRate;
    public float goldPer;
    public string iconKey;
    public string soundKey;
    public string effectKey;

    [PrimaryKey]
    public int ID { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public EDataType Type { get => type; set => type = value; }
    public ElementType Element { get => element; set => element = value; }
    public GradeType Grade { get => grade; set => grade = value; }
    public int Level { get => level; set => level = value; }
    public float EquipATK { get => equipATK; set => equipATK = value; }
    public float PassiveATK { get => passiveATK; set => passiveATK = value; }
    public float CriticalDMG { get => criticalDMG; set => criticalDMG = value; }
    public float CriticalRate { get => criticalRate; set => criticalRate = value; }
    public float GoldPer { get => goldPer; set => goldPer = value; }
    public string Icon {  get => iconKey; set => iconKey = value; }
    public string Sound { get => soundKey; set => soundKey = value; }
    public string Effect { get => effectKey; set => effectKey = value; }
}
