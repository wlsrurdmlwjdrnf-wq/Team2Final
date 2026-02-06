using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;


[System.Serializable]
public class PlayerInitData : BaseData
{    
    public string name;
    public int level;
    public Tier tier;
    public float atkPower;
    public float maxHP;
    public float hpRegenPerSec;
    public float maxMP;
    public float criticalRate;
    public float criticaldamage;
    public float mpRegenPerSec;
    public float goldMultiplier;
    public float curGold;
    public float expMultiplier;
    public float atkSpeed;
    public float moveSpeed;

    [PrimaryKey]
    public int ID { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public int Level { get => level; set => level = value; }
    public Tier Tier { get => tier; set => tier = value; }
    public float ATKPower { get => atkPower; set => atkPower = value; }
    public float MaxHP { get => maxHP; set => maxHP = value; }
    public float HPRegenPerSec { get => hpRegenPerSec; set => hpRegenPerSec = value; }
    public float MaxMP { get => maxMP; set => maxMP = value; }
    public float CriticalRate { get => criticalRate; set => criticalRate = value; }
    public float CriticalDamage { get => criticaldamage; set => criticaldamage = value; }    
    public float MPRegenPerSec { get => mpRegenPerSec; set => mpRegenPerSec = value; }
    public float GoldMultiplier { get => goldMultiplier; set => goldMultiplier = value; }
    public float CurGold { get => curGold; set => curGold = value; }
    public float EXPMultiplier { get => expMultiplier; set => expMultiplier = value; }
    public float ATKSpeed { get => atkSpeed; set => atkSpeed = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
}
