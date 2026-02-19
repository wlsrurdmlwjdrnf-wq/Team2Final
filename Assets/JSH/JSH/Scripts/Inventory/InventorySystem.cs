using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : Singleton<InventorySystem>
{
    private List<InventorySlot> _weaponInventory = new List<InventorySlot>();
    private List<InventorySlot> _accessoriesInventory = new List<InventorySlot>();
    private List<InventorySlot> _skillInventory = new List<InventorySlot>();
    private InventorySlot _equippedWeapon = null;
    private InventorySlot _equippedAccessory = null;
    private List<InventorySlot> _equippedSkills = new List<InventorySlot>();
    private int _maxSkillSlot = 8;
    private int _currSkillSlot = 4;
    [SerializeField] private GameEventChannelSO _eventChannel;
    private Dictionary<StatType,float> _totalStatDict = new Dictionary<StatType,float>();
    private List<StatModifier> _InventoryModifiers = new List<StatModifier>();
    private void OnEnable()
    {
        _eventChannel.OnEventRaised += HandleEvent;
    }
    private void OnDisable()
    {
        _eventChannel.OnEventRaised -= HandleEvent;
    }
    private void HandleEvent(EGameEventType type, object payload) 
    {
        switch (type)
        {
            case EGameEventType.SortInventory:
                if (payload is EDataType sortType) SortInventory(sortType);
                break;
            case EGameEventType.CombineSlot:
                if (payload is InventorySlot combineSlot) CombineSlot(combineSlot);
                break;
            case EGameEventType.UpgradeRequest:
                if (payload is InventorySlot upgradeSlot) UpgradeSlot(upgradeSlot);
                break;
            case EGameEventType.EquipRequest:
                if (payload is InventorySlot equipSlot) Equip(equipSlot);
                break;
            case EGameEventType.UnEquipRequest:
                if (payload is InventorySlot unEquipSlot) UnEquip(unEquipSlot);
                break;
            case EGameEventType.GachaPull:
                if (payload is ItemCard card)
                {
                    switch (card.Type)
                    {
                        case EDataType.Weapon:
                        case EDataType.Accessories:
                            var item = ItemSkillDataManager.Instance.GetItemData(card);
                            if (item != null) AddItem(item);
                            else Debug.LogWarning("GetItemDataFail");
                            break;
                        case EDataType.Skill:
                            var skillData = ItemSkillDataManager.Instance.GetSkillData(card);
                            if (skillData != null) AddSkill(skillData);
                            break;
                    }
                }
                break;
            case EGameEventType.AutoCombine:
                if (payload is EDataType combineType) AutoCombine(combineType);
                break;
        }
    }
    public void Initialize()
    {
        foreach (var item in ItemSkillDataManager.Instance.WeaponDatabase.items) 
        {
            _weaponInventory.Add(new InventorySlot(item, 0, false));
        }
        foreach (var item in ItemSkillDataManager.Instance.AccessoriesDatabase.items)
        {
            _accessoriesInventory.Add(new InventorySlot(item, 0, false));
        }
        foreach (var skill in ItemSkillDataManager.Instance.SkillDatabase.skills)
        {
            _skillInventory.Add(new InventorySlot(skill, 0, false));
        }
        Debug.Log($"Weapon:{_weaponInventory.Count}, Accessory:{_accessoriesInventory.Count}, Skill:{_skillInventory.Count}");
    }
    #region 정렬
    public void SortInventory(EDataType type)
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        targetInventory.Sort((a, b) =>
        {
            object aData = a.BaseData;
            object bData = b.BaseData;

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
        Debug.Log($"{itemDataSO.Name}");
        if (itemDataSO == null) Debug.LogWarning($"ItemDataNull!!");
        AddToInventory(itemDataSO, itemDataSO.Type);
    }
    public void AddSkill(SkillDataSO skillDataSO)
    {
        AddToInventory(skillDataSO, skillDataSO.Type);
    }
    private void AddToInventory(ScriptableObject data, EDataType type)
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        if (targetInventory == null) return;

        int index = targetInventory.FindIndex(slot => slot.Matches(data));
        Debug.Log(index);
        if (index >= 0)
        {
            var slot = targetInventory[index];
            if (!slot.Unlocked)
            {
                Debug.Log("SlotUnlocked!!");
                slot.Unlocked = true;
                slot.Stack = 0;
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, slot);
            }
            else 
            {
                slot.Stack++;
                if (slot.GetDataType() == EDataType.Weapon && _equippedWeapon == null) Equip(slot);
                else if (slot.GetDataType() == EDataType.Accessories && _equippedAccessory == null) Equip(slot);
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, slot);
            }
        }
        else
        {
            Debug.Log("!UnlockFailed!");
            var newSlot = new InventorySlot(data, 0, true);
            targetInventory.Add(newSlot);
            _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, newSlot);
        }
    }
    //public void AddCard(ItemCard card)
    //{
    //    var targetInventory = GetInventory(card.Type);
    //    if (targetInventory == null) return;

    //    //기존 슬롯 찾기
    //    int index = targetInventory.FindIndex(slot =>
    //    {
    //        if (slot.BaseData is ItemDataSO item)
    //            return item.Type == card.Type && item.Grade == card.Grade && item.Tier == card.Tier;
    //        if (slot.BaseData is SkillDataSO skill)
    //            return skill.Type == card.Type && skill.Grade == card.Grade;
    //        return false;
    //    });

    //    if (index >= 0)
    //    {
    //        //기존 슬롯 스택 증가
    //        var slot = targetInventory[index];
    //        slot.Stack++;
    //        Debug.Log($"Stack increased: {slot.BaseData.name}, new stack = {slot.Stack}");
    //        _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, slot);
    //    }
    //}

    #endregion
    #region 합성
    public void AutoCombine(EDataType type)
    {
        SortInventory(type);
        var targetInventory = GetInventory(type);

        for (int i = 0; i < targetInventory.Count; i++)
        {
            var slot = targetInventory[i];
            if (slot.Stack >= PublicConst.UpgradeStack) CombineSlot(slot);           
        }
    }
    public void CombineSlot(InventorySlot slot) 
    {
        List<InventorySlot> targetInventory = GetInventory(slot.GetDataType());
        while (slot.TryCombine(PublicConst.UpgradeStack, out InventorySlot newSlot))
        {
            int existingIndex = targetInventory.FindIndex(s => s.BaseData == newSlot.BaseData);
            if (existingIndex >= 0)
            {
                targetInventory[existingIndex].Stack++;
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, targetInventory[existingIndex]);
            }
            else
            {
                targetInventory.Add(newSlot);
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, newSlot);
            }
        }
        //스택감소 반영
        _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, slot);
    }
    #endregion
    #region 강화
    public void UpgradeSlot(InventorySlot slot) 
    {
        BigNumber amount = new BigNumber(slot.GetUpgradeCost());
        if (slot.GetDataType() == EDataType.Skill)
        {
            if (slot.Stack >= PublicConst.UpgradeStack && slot.Unlocked) 
            {
                //여기서 골드 소모 & 소모 가능여부 체크 둘 다 해줌 + 다른 재화 써야하면 나중에 수정
                if (!PlayerResourceManager.Instance.SpendResource(ResourceType.Gold, amount)) return;
                slot.Stack -= PublicConst.UpgradeStack;
                slot.Upgrade();
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, slot);
            }
        }
        else
        {
            if (slot.Unlocked)
            {
                if (!PlayerResourceManager.Instance.SpendResource(ResourceType.Gold, amount)) return;
                slot.Upgrade();
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, slot);
            }
            else
            {
                //강화실패
                Debug.Log("NotEnoughCost or NotUnlocked");
            }
        }
    }
    #endregion
    #region 장착
    public void Equip(InventorySlot slot)
    {
        //해금 안된 경우
        if (!slot.Unlocked)
        {
            Debug.Log("NotUnlocked");
            return;
        }
        switch (slot.GetDataType())
        {
            case EDataType.Weapon:
                if (_equippedWeapon != null) { _equippedWeapon = null; }
                _equippedWeapon = slot;
                //Debug.Log($"EquippedWeapon:{((ItemDataSO)slot.BaseData).Name}");
                break;
            case EDataType.Accessories:
                if (_equippedAccessory != null) { _equippedAccessory = null; }
                _equippedAccessory = slot;
                //Debug.Log($"EquippedAccessory:{((ItemDataSO)slot.BaseData).Name}");
                break;
            case EDataType.Skill:
                if (!_equippedSkills.Contains(slot) && _equippedSkills.Count < _currSkillSlot)
                {
                    _equippedSkills.Add(slot);
                    //Debug.Log($"EquippedSkill:{((SkillDataSO)slot.BaseData).Name}");
                }
                break;
        }
        slot.IsEquipped = true;
        _eventChannel.RaiseEvent(EGameEventType.EquipChanged, slot);
    }
    public void UnEquip(InventorySlot slot)
    {
        switch (slot.GetDataType())
        {
            case EDataType.Weapon: _equippedWeapon = null; break;
            case EDataType.Accessories: _equippedAccessory = null; break;
            case EDataType.Skill: _equippedSkills.Remove(slot); break;
        }
        slot.IsEquipped = false;
        _eventChannel.RaiseEvent(EGameEventType.EquipChanged, slot);
    }
    public void AddSkillSlot()
    {
        if (_currSkillSlot < _maxSkillSlot) _currSkillSlot++;
    }
    #endregion
    #region 스탯합산
    public void StatModifyToPlayer() 
    {
        if (_InventoryModifiers.Count > 0)
        {
            foreach (var modifier in _InventoryModifiers) 
            {
                PlayerStatManager.Instance.RemoveModifier(modifier);
            }
            _InventoryModifiers.Clear();
        }
        foreach (StatType stat in Enum.GetValues(typeof(StatType)))
        {
            StatModifier newModifier = new StatModifier(stat, Operation.Add, _totalStatDict[stat]);
            _InventoryModifiers.Add(newModifier);
            PlayerStatManager.Instance.AddModifier(newModifier);
        }
    }
    public Dictionary<StatType, float> CalculateStats() 
    {
        CleanStatDict();
        AddPassiveStats(EDataType.Weapon);
        AddPassiveStats(EDataType.Accessories);
        AddPassiveStats(EDataType.Skill);
        AddEquipStats(_equippedWeapon);
        AddEquipStats(_equippedAccessory);
        foreach (var skillSlot in _equippedSkills) { AddEquipStats(skillSlot); }
        return _totalStatDict;
    }
    private void AddPassiveStats(EDataType type) 
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        foreach (var slot in targetInventory) 
        {
            if (!slot.Unlocked) continue;
            if (slot.BaseData is ItemDataSO item)
            {
                AddStat(item.PassiveStat, item.PassiveValue);
                AddStat(StatType.CritRate, item.CriticalRate);
                AddStat(StatType.CritDamage, item.CriticalDMG);
                AddStat(StatType.GoldMultiplier, item.GoldPer);
            }
            else if (slot.BaseData is SkillDataSO skill) 
            {
                //스킬데이터의 크리 항목이 스킬 보정치면 이거 빼야함
                AddStat(StatType.CritRate, skill.CriticalRate);
                AddStat(StatType.CritDamage, skill.CriticalDMG);
            }
        }
    }
    private void AddEquipStats(InventorySlot slot) 
    {
        if ( slot == null || !slot.Unlocked) return;

        if (slot.BaseData is ItemDataSO item)
        {
            AddStat(item.PassiveStat, item.EquipValue);
        }
        else if (slot.BaseData is SkillDataSO skill) 
        {
            AddStat(skill.Stat, skill.ModifyAmount);
        }
    }
    private void AddStat(StatType stat, float value) 
    {
        if(_totalStatDict.ContainsKey(stat)) _totalStatDict[stat] += value;
    }
    #endregion
    #region 헬퍼
#if UNITY_EDITOR
    public void PrintInventory(EDataType type)
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        Debug.Log($"{type} Inventory");
        for (int i = 0; i < targetInventory.Count; i++)
        {
            var slot = targetInventory[i];
            string name = "";
            if (slot.BaseData is ItemDataSO item) { name = item.Name; }
            else if (slot.BaseData is SkillDataSO skill) { name = skill.Name; }
            Debug.Log($"Index:{i}, Name:{name}, Grade:{GetGrade(slot)}, Tier:{GetTier(slot)}");
        }
    }
#endif
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
        if (slot.BaseData is ItemDataSO item) return item.Grade;
        if (slot.BaseData is SkillDataSO skill) return skill.Grade;
        return GradeType.Normal;
    }
    private int GetTier(InventorySlot slot) 
    {
        if (slot.BaseData is ItemDataSO item) return item.Tier;
        if (slot.BaseData is SkillDataSO skill) return 0;
        return 0;
    }
    private void CleanStatDict() 
    {
        foreach (StatType stat in Enum.GetValues(typeof(StatType)))
        {
            _totalStatDict[stat] = 0f;
        }
    }
    public List<InventorySlot> GetInventory(EDataType type) 
    {
        switch (type)
        {
            case EDataType.Weapon: return _weaponInventory;
            case EDataType.Accessories: return _accessoriesInventory;       
            case EDataType.Skill: return _skillInventory;
            default: return null;
        }
    }
    public List<InventorySlot> GetEquippedSkills() { return _equippedSkills; }
    public InventorySlot GetEquippedWeapon() { return _equippedWeapon; }
    public InventorySlot GetEquippedAccessory() { return _equippedAccessory; }
    #endregion
}