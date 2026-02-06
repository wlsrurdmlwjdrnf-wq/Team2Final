using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[System.Serializable]
public class AccessoryData : BaseData
{    
    public string name;
    public EDataType type;
    public ElementType elemnet;
    public GradeType grade;
    public int level;
    public float hp;
    public float hpPer;
    public float mpPer;
    public float goldPer;
    public string iconKey;


    [PrimaryKey]
    public int ID { get => id; set => id = value; }
    public string Name { get => name; set => name = value;}
    public EDataType Type { get => type; set => type = value; }
    public ElementType Element { get => elemnet; set => elemnet = value; }
    public GradeType Grade { get => grade; set => grade = value; }
    public int Level { get => level; set => level = value; }
    public float HP { get => hp; set => hp = value; }
    public float HPPer { get => hpPer; set => hpPer = value; }
    public float MPPer { get => mpPer; set => mpPer = value; }
    public float GoldPer { get => goldPer; set => goldPer = value; }
    public string Icon {  get => iconKey; set => iconKey = value; }
}
