using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[System.Serializable]
public class AccessoryData : BaseData
{    
    public string name;
    public EDataType type;
    public int tier;
    public GradeType grade;
    public int level;
    public float equipHPPer;
    public float passiveHPPer;
    public float mPPer;
    public float eXPPer;
    public float equipHPPerByLv;
    public float passiveHPPerByLv;
    public float mPPerByLv;
    public float eXPPerByLv;


    [PrimaryKey]
    public int ID { get => id; set => id = value; }
    public string Name { get => name; set => name = value;}
    public EDataType Type { get => type; set => type = value; }
    public GradeType Grade { get => grade; set => grade = value; }
    public int Level { get => level; set => level = value; }
    public float EquipHPPer { get => equipHPPer; set => equipHPPer = value; }
    public float PassiveHPPer { get => passiveHPPer; set => passiveHPPer = value; }
    public float MPPer { get => mPPer; set => mPPer = value; }
    public float EXPPer { get => eXPPer; set => eXPPer = value; }
    public float EquipHPPerByLv { get => equipHPPerByLv; set => equipHPPerByLv = value; }
    public float PassiveHPPerByLv { get => passiveHPPerByLv; set =>  passiveHPPerByLv = value; }
    public float MPPerByLv { get => mPPerByLv; set => mPPerByLv = value; }
    public float EXPPerByLv { get => eXPPerByLv; set => eXPPerByLv = value; }
}
