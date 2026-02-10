using System;
using UnityEngine;

public class InventorySlot
{
    public Guid Id { get; private set; }
    public ScriptableObject BaseData;
    public int Stack;
    public int Level;
    public bool Unlocked;
    public float ActiveEffectValue;
    public float PassiveEffectValue;

    public InventorySlot(ScriptableObject baseData, int stack, bool unlocked = false)
    {
        Id = Guid.NewGuid();
        this.BaseData = baseData;
        this.Stack = stack;
        this.Unlocked = unlocked;

        if (baseData is ItemDataSO item)
        {
            Level = item.Level;
            ActiveEffectValue = item.EquipATK;
            PassiveEffectValue = item.PassiveATK;
        }
        else if (baseData is SkillDataSO skill) 
        {
            Level = skill.Level;
            ActiveEffectValue = skill.Damage;
            PassiveEffectValue = skill.ModifyAmount;
        }
    }
    public int GetUpgradeCost() 
    {
        return Level * 100; //юс╫ц
    }
    public void Upgrade() 
    {
        Level++;
        ActiveEffectValue = CalculateEffect(0, Level);
        PassiveEffectValue = CalculateEffect(1, Level);
    }

    public float CalculateEffect(int type,int level) 
    {
        float effectValue = 0;
        if (BaseData is ItemDataSO item)
        {
            switch (type) 
            {
                case 0:
                    effectValue = item.EquipATK + level * 10f;
                    break;
                case 1:
                    effectValue = item.PassiveATK + level * 10f;
                    break;
            }
        }
        else if (BaseData is SkillDataSO skill)
        {
            switch (type)
            {
                case 0:
                    effectValue = skill.Damage + level * 10f;
                    break;
                case 1:
                    effectValue = skill.ModifyAmount + level * 10f;
                    break;
            }
        }
        return effectValue;
    }

    public bool TryCombine(int requireStack, out InventorySlot newSlot)
    {
        newSlot = null;
        if (Stack < requireStack) return false;
        if (BaseData is ItemDataSO itemData)
        {
            int tier = itemData.Tier;
            GradeType grade = itemData.Grade;

            if (tier > 1) { tier--; }
            else
            {
                grade = GetNextRarity(grade);
                tier = 4;
            }

            ItemCard card = new ItemCard
            {
                Type = itemData.Type,
                Grade = grade,
                Tier = tier
            };

            ItemDataSO newData = ItemSkillDataManager.Instance.GetItemData(card);
            if (newData != null)
            {
                newSlot = new InventorySlot(newData, 1, true);
            }
        }
        else if (BaseData is SkillDataSO skillData)
        {
            GradeType rarity = GetNextRarity(skillData.Grade);

            ItemCard card = new ItemCard
            {
                Type = skillData.Type,
                Grade = rarity,
                Tier = 4
            };

            SkillDataSO newSkill = ItemSkillDataManager.Instance.GetSkillData(card);
            if (newSkill != null)
            {
                newSlot = new InventorySlot(newSkill, 1, true);
            }
        }
        Stack -= requireStack;
        return true;
    }
    public EDataType GetDataType() 
    {
        if (BaseData is ItemDataSO item)
            return item.Type;
        else if (BaseData is SkillDataSO skill)
            return skill.Type;
        EDataType type = EDataType.Weapon;
        return type;
    }
    private GradeType GetNextRarity(GradeType rarity)
    {
        switch (rarity)
        {
            case GradeType.Normal: return GradeType.Advanced;
            case GradeType.Advanced: return GradeType.Rare;
            case GradeType.Rare: return GradeType.Heroic;
            case GradeType.Heroic: return GradeType.Legendary;
            case GradeType.Legendary: return GradeType.Mythical;
            default: return GradeType.Mythical;
        }
    }
}