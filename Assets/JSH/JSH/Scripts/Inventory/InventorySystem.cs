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

    [SerializeField] private GameEventChannelSO _eventChannel;
    private void Awake()
    {
        Instance = this;
    }
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
                if (payload is EDataType sortType)
                    SortInventory(sortType);
                break;
            case EGameEventType.CombineSlot:
                if (payload is InventorySlot combineSlot)
                    CombineSlot(combineSlot);
                break;
            case EGameEventType.UpgradeRequest:
                if (payload is InventorySlot upgradeSlot)
                    UpgradeSlot(upgradeSlot);
                break;
            case EGameEventType.EquipRequest:
                if (payload is InventorySlot equipSlot)
                {
                    if (equipSlot.BaseData is ItemDataSO item)
                        Equip(equipSlot);
                    else if (equipSlot.BaseData is SkillDataSO skill)
                        Equip(equipSlot);
                }
                break;
            case EGameEventType.UnEquipRequest:
                if (payload is InventorySlot unEquipSlot)
                {
                    if (unEquipSlot.BaseData is ItemDataSO item)
                        UnEquip(unEquipSlot);
                    else if (unEquipSlot.BaseData is SkillDataSO skill)
                        UnEquip(unEquipSlot);
                }
                break;
            case EGameEventType.GachaPull:
                if (payload is ItemCard card)
                {
                    switch (card.Type)
                    {
                        case EDataType.Weapon:
                            var weapon = ItemSkillDataManager.Instance.GetItemData(card);
                            if (weapon != null) AddItem(weapon);
                            break;
                        case EDataType.Accessories:
                            var accessory = ItemSkillDataManager.Instance.GetItemData(card);
                            if (accessory != null) AddItem(accessory);
                            break;
                        case EDataType.Skill:
                            var skillData = ItemSkillDataManager.Instance.GetSkillData(card);
                            if (skillData != null) AddSkill(skillData);
                            break;
                    }
                }
                break;
            case EGameEventType.AutoCombine:
                if (payload is EDataType combineType)
                    AutoCombine(combineType);
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
        TotalStats stat = CalculateStats();
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

        int index = targetInventory.FindIndex(slot => slot.BaseData == data);
        if (index >= 0)
        {
            var slot = targetInventory[index];
            if (!slot.Unlocked)
            {
                slot.Unlocked = true;
                slot.Stack = 0;
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, slot);
            }
            else 
            {
                slot.Stack++;
                Debug.Log($"{slot.BaseData}{type} : {slot.Stack}");
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, slot);
            }
        }
        else
        {
            var newSlot = new InventorySlot(data, 0, true);
            targetInventory.Add(newSlot);
            _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, newSlot);
        }

    }
    public void UnlockItem(EDataType type, int slotIndex) 
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        var slot = targetInventory[slotIndex];
        slot.Stack++;
        slot.Unlocked = true;
        targetInventory[slotIndex] = slot;
    }
    #endregion
    #region 합성
    public void AutoCombine(EDataType type)
    {
        SortInventory(type);
        var targetInventory = GetInventory(type);

        for (int i = 0; i < targetInventory.Count; i++)
        {
            var slot = targetInventory[i];
            if (slot.Stack >= PublicConst.UpgradeStack)
            {
                CombineSlot(slot);
            }
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
        int cost = slot.GetUpgradeCost();
        if (slot.GetDataType() == EDataType.Skill)
        {
            //골드 소모부분 나중에 고쳐야함
            if (slot.Stack >= PublicConst.UpgradeStack && testGold >= cost && slot.Unlocked) 
            {
                testGold -= cost;
                slot.Stack -= PublicConst.UpgradeStack;

                slot.Upgrade();
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, slot);
            }
        }
        else 
        {
            //코스트를 받아올수 있게 되면 판단 로직을 슬롯 내부로 옮기는게 더 좋을듯
            if (testGold >= cost && slot.Unlocked)
            {
                testGold -= cost;
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
                Debug.Log($"EquippedWeapon:{((ItemDataSO)slot.BaseData).Name}");
                break;
            case EDataType.Accessories:
                if (_equippedAccessory != null) { _equippedAccessory = null; }
                _equippedAccessory = slot;
                Debug.Log($"EquippedAccessory:{((ItemDataSO)slot.BaseData).Name}");
                break;
            case EDataType.Skill:
                if (!_equippedSkills.Contains(slot) && _equippedSkills.Count < _currSkillSlot)
                {
                    _equippedSkills.Add(slot);
                    Debug.Log($"EquippedSkill:{((SkillDataSO)slot.BaseData).Name}");
                }
                break;
        }
        _eventChannel.RaiseEvent(EGameEventType.EquipChanged, slot);
    }
    public void UnEquip(InventorySlot slot)
    {
        switch (slot.GetDataType())
        {
            case EDataType.Weapon:
                _equippedWeapon = null;
                break;
            case EDataType.Accessories:
                _equippedAccessory = null;
                break;
            case EDataType.Skill:
                _equippedSkills.Remove(slot);
                break;
        }
        _eventChannel.RaiseEvent(EGameEventType.EquipChanged, slot);
    }
    public void AddSkillSlot()
    {
        if (_currSkillSlot < _maxSkillSlot) _currSkillSlot++;
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
            if (!slot.Unlocked) continue;
            if (slot.BaseData is ItemDataSO item)
            {
                switch (type) 
                {
                    case EDataType.Weapon:
                        totalStats.ATK += item.PassiveATK;
                        break;
                    case EDataType.Accessories:
                        totalStats.HP += item.PassiveATK;
                        break;
                }
                totalStats.CriticalRate += item.CriticalRate;
                totalStats.CriticalDMG += item.CriticalDMG;
                totalStats.GoldPer += item.GoldPer;
            }
            else if (slot.BaseData is SkillDataSO skill) 
            {
                break;
            }
        }
    }
    private void AddEquipStats(InventorySlot slot, ref TotalStats totalStats) 
    {
        if ( slot == null || !slot.Unlocked) return;

        if (slot.BaseData is ItemDataSO item)
        {
            if (item.Type == EDataType.Weapon)
                totalStats.ATK += item.EquipATK;
            else if (item.Type == EDataType.Accessories)
                totalStats.HP += item.EquipATK;
        }
        else if (slot.BaseData is SkillDataSO skill) 
        {
            switch (skill.Stat) 
            {
                case StatType.AttackPower: 
                    totalStats.ATK += skill.ModifyAmount;
                    break;
                case StatType.AttackSpeed:
                    totalStats.AttackSpeed += skill.ModifyAmount;
                    break;
                case StatType.MoveSpeed:
                    totalStats.MoveSpeed += skill.ModifyAmount;
                    break;
            }
        }
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