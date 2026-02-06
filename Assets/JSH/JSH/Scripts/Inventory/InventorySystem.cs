using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    private List<InventorySlot> _weaponInventory = new List<InventorySlot>();
    private List<InventorySlot> _accessoriesInventory = new List<InventorySlot>();
    private List<InventorySlot> _skillInventory = new List<InventorySlot>();

    private InventorySlot _equippedWeapon = null;
    private InventorySlot _equippedAccessory = null;
    private List<InventorySlot> _equippedSkills = new List<InventorySlot>();
    private int _maxSkillSlot = 8;
    private int _currSkillSlot = 4;

    private int testGold = 1000; //임시골드

    private void Awake()
    {
        Instance = this;
    }
    public void Initialize()
    {
        foreach (var item in ItemSkillDataManager.Instance.WeaponDatabase.items) 
        {
            _weaponInventory.Add(new InventorySlot(new ItemInstance(item), 0, false));
        }
        foreach (var item in ItemSkillDataManager.Instance.AccessoriesDatabase.items)
        {
            _accessoriesInventory.Add(new InventorySlot(new ItemInstance(item), 0, false));
        }
        foreach (var skill in ItemSkillDataManager.Instance.SkillDatabase.skills)
        {
            _skillInventory.Add(new InventorySlot(SkillFactory.CreateInstance(skill), 0, false));
        }
        Debug.Log($"Weapon:{_weaponInventory.Count}, Accessory:{_accessoriesInventory.Count}, Skill:{_skillInventory.Count}");
        TotalStats stat = CalculateStats();
    }
    #region 정렬
    public void SortInventory(EDataType type)
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        targetInventory.Sort((a, b) =>
        {
            object aData = null;
            if (a.instance is ItemInstance ai) { aData = ai.baseData; }
            else if (a.instance is SkillInstance asi) { aData = asi.baseData; }
            object bData = null;
            if (b.instance is ItemInstance bi) { bData = bi.baseData; }
            else if (b.instance is SkillInstance bsi) { bData = bsi.baseData; }
            GradeType aGrade;
            int aTier;
            if (aData is ItemDataSO aItem)
            {
                aGrade = aItem.Grade;
                aTier = aItem.Tier;
            }
            else if (aData is SkillDataSO aSkill)
            {
                aGrade = aSkill.Grade;
                aTier = 0;
            }
            else return 0;
            GradeType bGrade;
            int bTier;
            if (bData is ItemDataSO bItem)
            {
                bGrade = bItem.Grade;
                bTier = bItem.Tier;
            }
            else if (bData is SkillDataSO bSkill)
            {
                bGrade = bSkill.Grade;
                bTier = 0;
            }
            else return 0;
            //등급비교
            int gradeCompare = GetGradeOrder(aGrade).CompareTo(GetGradeOrder(bGrade));
            if (gradeCompare != 0) return gradeCompare;
            //티어비교
            return bTier.CompareTo(aTier);
        });
    }
    #endregion
    #region 획득
    public void AddItem(ItemDataSO itemDataSO) 
    {
        var newInstance = new ItemInstance(itemDataSO);
        AddToInventory(newInstance, itemDataSO.Type);
    }
    public void AddSkill(SkillDataSO skillDataSO)
    {
        var newInstance = SkillFactory.CreateInstance(skillDataSO);
        AddToInventory(newInstance, skillDataSO.Type);
    }
    private void AddToInventory(IUpgradable instance, EDataType type)
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        if (targetInventory == null) return;
        int index = targetInventory.FindIndex(slot => 
        (slot.instance is ItemInstance ii && instance is ItemInstance ni && ii.baseData == ni.baseData) ||
        (slot.instance is SkillInstance si && instance is SkillInstance nsi && si.baseData == nsi.baseData));
        if (index >= 0)
        {
            var slot = targetInventory[index];
            if (!slot.unlocked)
            {
                slot.unlocked = true;
                slot.stack = 0;
            }
            else 
            {
                slot.stack++;
            }
        }
        else
        {
            targetInventory.Add(new InventorySlot(instance, 0, true));
        }
    }
    public void UnlockItem(EDataType type, int slotIndex) 
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        var slot = targetInventory[slotIndex];
        slot.stack++;
        slot.unlocked = true;
        targetInventory[slotIndex] = slot;
    }
    #endregion
    #region 합성
    public void CombineSlot(EDataType type, int slotIndex, int requiredStack = 5) 
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        var slot = targetInventory[slotIndex];
        if (slot.TryCombine(requiredStack, out IUpgradable newInstance))
        {
            targetInventory.Add(new InventorySlot(newInstance, 1));
        }
        else 
        {
            //합성실패
            Debug.Log("NotEnoughStack");
        }
        //스택감소 반영
        targetInventory[slotIndex] = slot;
    }
    #endregion
    #region 강화
    public void UpgradeSlot(EDataType type, int slotIndex) 
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        var slot = targetInventory[slotIndex];
        int cost = CalculateUpgradeCost(slot.instance.Level);
        if (testGold >= cost)
        {
            testGold -= cost;
            slot.instance.Upgrade();
            targetInventory[slotIndex] = slot;
        }
        else 
        {
            //강화실패
            Debug.Log("NotEnoughCost");
        }
    }
    #endregion
    #region 장착
    public void Equip(EDataType type, int slotIndex)
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        var slot = targetInventory[slotIndex];
        Equip(type, slot);
    }
    public void Equip(EDataType type,InventorySlot slot)
    {
        //해금 안된 경우
        if (!slot.unlocked)
        {
            Debug.Log("NotUnlocked");
            return;
        }
        switch (type)
        {
            case EDataType.Weapon:
                if (_equippedWeapon != null) { _equippedWeapon = null; }
                _equippedWeapon = slot;
                Debug.Log($"EquippedWeapon:{((ItemInstance)slot.instance).baseData.Name}");
                break;
            case EDataType.Accessories:
                if (_equippedAccessory != null) { _equippedAccessory = null; }
                _equippedAccessory = slot;
                Debug.Log($"EquippedAccessory:{((ItemInstance)slot.instance).baseData.Name}");
                break;
            case EDataType.Skill:
                if (!_equippedSkills.Contains(slot) && _equippedSkills.Count < _currSkillSlot)
                {
                    _equippedSkills.Add(slot);
                    Debug.Log($"EquippedSkill:{((SkillInstance)slot.instance).baseData.Name}");
                }
                break;
        }
    }
    public void UnEquip(EDataType type, int slotIndex = -1)
    {
        switch (type)
        {
            case EDataType.Weapon:
                _equippedWeapon = null;
                break;
            case EDataType.Accessories:
                _equippedAccessory = null;
                break;
            case EDataType.Skill:
                if (slotIndex >= 0 && slotIndex < _equippedSkills.Count)
                { _equippedSkills.RemoveAt(slotIndex); }
                else
                { _equippedSkills.Clear(); }
                break;
        }
    }
    #endregion
    #region 스탯합산
    public TotalStats CalculateStats() 
    {
        TotalStats totalStats = new TotalStats();
        //ref로 해야 원본도 수정됨
        AddPassiveStats(EDataType.Weapon, ref totalStats);
        AddPassiveStats(EDataType.Accessories, ref totalStats);
        AddPassiveStats(EDataType.Skill, ref totalStats);
        AddEquipStats(_equippedWeapon, ref totalStats);
        AddEquipStats(_equippedAccessory, ref totalStats);
        foreach (var skillSlot in _equippedSkills) { AddEquipStats(skillSlot, ref totalStats); }
        return totalStats;
    }
    private void AddPassiveStats(EDataType type, ref TotalStats totalStats) 
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        foreach (var slot in targetInventory) 
        {
            if (!slot.unlocked) continue;
            if (slot.instance is ItemInstance item)
            {
                var data = item.baseData;
                switch (type) 
                {
                    case EDataType.Weapon:
                        totalStats.ATK += data.PassiveATK;
                        break;
                    case EDataType.Accessories:
                        totalStats.HP += data.PassiveATK;
                        break;
                }
                totalStats.CriticalRate += data.CriticalRate;
                totalStats.CriticalDMG += data.CriticalDMG;
                totalStats.GoldPer += data.GoldPer;
            }
            else if (slot.instance is SkillInstance skill) 
            {
                break;
            }
        }
    }
    private void AddEquipStats(InventorySlot slot, ref TotalStats totalStats) 
    {
        if ( slot == null || !slot.unlocked) return;

        if (slot.instance is ItemInstance item)
        {
            var data = item.baseData;
            if (data.Type == EDataType.Weapon)
                totalStats.ATK += data.EquipATK;
            else if (data.Type == EDataType.Accessories)
                totalStats.HP += data.EquipATK;
        }
        else if (slot.instance is SkillInstance skill) 
        {
            var data = skill.baseData;
            switch (data.Stat) 
            {
                case StatType.AttackPower: 
                    totalStats.ATK += data.ModifyAmount;
                    break;
                case StatType.AttackSpeed:
                    totalStats.AttackSpeed += data.ModifyAmount;
                    break;
                case StatType.MoveSpeed:
                    totalStats.MoveSpeed += data.ModifyAmount;
                    break;
            }
        }
    }
    #endregion
    #region 헬퍼
    public void PrintInventory(EDataType type)
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        Debug.Log($"{type} Inventory");
        for (int i = 0; i < targetInventory.Count; i++)
        {
            var slot = targetInventory[i];
            string name = "";
            if (slot.instance is ItemInstance item) { name = item.baseData.Name; }
            else if (slot.instance is SkillInstance skill) { name = skill.baseData.Name; }
            Debug.Log($"Index:{i}, Name:{name}, Grade:{GetGrade(slot)}, Tier:{GetTier(slot)}");
        }
    }
    private int CalculateUpgradeCost(int currentLevel) 
    {
        return currentLevel * 100;  //임시
    }
    private void AddSkillSlot() 
    {
        if (_currSkillSlot < _maxSkillSlot) _currSkillSlot++;
    }
    private int GetGradeOrder(GradeType grade) 
    {
        switch (grade) 
        {
            case GradeType.Normal: return 0;
            case GradeType.Advanced: return 1;
            case GradeType.Rare: return 2;
            case GradeType.Heroic: return 3;
            case GradeType.Legendary: return 4;
            case GradeType.Mythical: return 5;
            default: return 99999;
        }
    }
    private GradeType GetGrade(InventorySlot slot) 
    {
        if (slot.instance is ItemInstance item) return item.baseData.Grade;
        if (slot.instance is SkillInstance skill) return skill.baseData.Grade;
        return GradeType.Normal;
    }
    private int GetTier(InventorySlot slot) 
    {
        if (slot.instance is ItemInstance item) return item.baseData.Tier;
        return 0;
    }
    public List<InventorySlot> GetInventory(EDataType type) 
    {
        List<InventorySlot> targetInventory = null;

        switch (type)
        {
            case EDataType.Weapon:
                targetInventory = _weaponInventory;
                break;
            case EDataType.Accessories:
                targetInventory = _accessoriesInventory;
                break;
            case EDataType.Skill:
                targetInventory = _skillInventory;
                break;
        }
        return targetInventory;
    }
    public List<InventorySlot> GetEquippedSkills() { return _equippedSkills; }
    public InventorySlot GetEquippedWeapon() { return _equippedWeapon; }
    public InventorySlot GetEquippedAccessory() { return _equippedAccessory; }
    #endregion
}
