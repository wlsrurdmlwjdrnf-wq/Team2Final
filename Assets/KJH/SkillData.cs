using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[System.Serializable]
public class SkillData : BaseData
{    
    public string name;
    public EDataType type;
    public ElementType element;
    public GradeType grade;
    public int level;
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
    public string Icon {  get => iconKey; set => iconKey = value; }
    public string Sound { get => soundKey; set => soundKey = value; }
    public string Effect { get => effectKey; set => effectKey = value; }
}
