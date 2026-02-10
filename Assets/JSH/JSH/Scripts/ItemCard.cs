
using System.Collections.Generic;
using UnityEngine;

//°¡Ã­°á°ú¹°
public struct ItemCard
{
    public EDataType Type;
    public GradeType Grade;
    public int Tier;
    public ItemCard(EDataType type, GradeType rarity, int grade = 0)
    {
        this.Type = type;
        this.Grade = rarity;
        this.Tier = grade;
    }
}