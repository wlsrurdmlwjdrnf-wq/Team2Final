using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

// 스테이지 데이터
[System.Serializable]
public class StageData : BaseData
{    
    public string name;
    public ElementType element;
    public GradeType grade;
    public int level;    

    [PrimaryKey]
    public int ID { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public ElementType Element { get => element; set => element = value; }
    public GradeType Grade { get => grade; set => grade = value; }
    public int Level { get => level; set => level = value; }
}

