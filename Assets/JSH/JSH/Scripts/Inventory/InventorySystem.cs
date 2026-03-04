using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : Singleton<InventorySystem>
{
    private List<InventorySlot> _weaponInventory = new List<InventorySlot>();
    private List<InventorySlot> _accessoriesInventory = new List<InventorySlot>();
    private List<InventorySlot> _artifactInventory = new List<InventorySlot>();
    private List<InventorySlot> _skillInventory = new List<InventorySlot>();
    private InventorySlot _equippedWeapon = null;
    private InventorySlot _equippedAccessory = null;
    private List<InventorySlot> _equippedSkills = new List<InventorySlot>();
    private int _maxSkillSlot = 8;
    private int _currSkillSlot = 4;
    [SerializeField] private GameEventChannelSO _eventChannel;
    private Dictionary<StatType,float> _totalStatDict = new Dictionary<StatType,float>();
    private List<StatModifier> _InventoryModifiers = new List<StatModifier>();
    //스킬 바꾸는 기능
    public bool _OnSkillChange = false;
    private InventorySlot _EquipSkill = null;
    private InventorySlot _UnequipSkill = null;
    public int MaxSkillSlot { get => _maxSkillSlot; set => _maxSkillSlot = value; }
    public int CurrSkillSlot { get => _currSkillSlot; set => _currSkillSlot = value; }
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
                        case EDataType.Artifact:
                            var item = ItemSkillDataManager.Instance.GetItemData(card);
                            if (item != null) AddItem(item);
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
            case EGameEventType.RequestAddSkillSlot:
                AddSkillSlot();
                break;
            case EGameEventType.RequestChangeSkill:
                if (payload is InventorySlot slot)
                {
                    _UnequipSkill = slot;
                    ChangeSkill();
                }
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
        foreach (var item in ItemSkillDataManager.Instance.ArtifactsDatabase.items)
        {
            _artifactInventory.Add(new InventorySlot(item, 0, false));
        }
        foreach (var skill in ItemSkillDataManager.Instance.SkillDatabase.skills)
        {
            _skillInventory.Add(new InventorySlot(skill, 0, false));
        }
        foreach (StatType stat in Enum.GetValues(typeof(StatType))) 
        {
            if (!_totalStatDict.ContainsKey(stat)) _totalStatDict[stat] = 0;
        }
        SortInventory(EDataType.Weapon);
        SortInventory(EDataType.Accessories);
        SortInventory(EDataType.Skill);
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
        if (itemDataSO == null) return;
        AddToInventory(itemDataSO, itemDataSO.Type);
    }
    public void AddSkill(SkillDataSO skillDataSO)
    {
        if (skillDataSO == null) return;
        AddToInventory(skillDataSO, skillDataSO.Type);
    }
    private void AddToInventory(ScriptableObject data, EDataType type)
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        if (targetInventory == null) return;

        int index = targetInventory.FindIndex(slot => slot.Matches(data));
        if (index >= 0)
        {
            var slot = targetInventory[index];
            if (!slot.Unlocked)
            {
                slot.Unlocked = true;
                slot.Stack = 0;
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, new SlotPayload(slot));
            }
            else 
            {
                slot.Stack++;
                if (slot.GetDataType() == EDataType.Weapon && _equippedWeapon == null) Equip(slot);
                else if (slot.GetDataType() == EDataType.Accessories && _equippedAccessory == null) Equip(slot);
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, new SlotPayload(slot));
            }
        }
        else
        {
            var newSlot = new InventorySlot(data, 0, true);
            targetInventory.Add(newSlot);
            _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, new SlotPayload(newSlot));
        }
        StatModifyToPlayer();
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
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, new SlotPayload(targetInventory[existingIndex]));
            }
            else
            {
                targetInventory.Add(newSlot);
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, new SlotPayload(newSlot));
            }
        }
        StatModifyToPlayer();
        //스택감소 반영
        _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, new SlotPayload(slot));
    }
    public void CombineCount(int count, InventorySlot slot)
    {
        List<InventorySlot> targetInventory = GetInventory(slot.GetDataType());
        for (int i = 0; i < count; i++)
        {
            if (slot.TryCombine(PublicConst.UpgradeStack, out InventorySlot newSlot)) 
            {
                int existingIndex = targetInventory.FindIndex(s => s.BaseData == newSlot.BaseData);
                if (existingIndex >= 0)
                {
                    targetInventory[existingIndex].Stack++;
                    _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, new SlotPayload(targetInventory[existingIndex]));
                }
                else
                {
                    targetInventory.Add(newSlot);
                    _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, new SlotPayload(newSlot));
                }
            }
        }
        StatModifyToPlayer();
        //스택감소 반영
        _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, new SlotPayload(slot));
    }
    #endregion
    #region 강화  
    public void UpgradeSlot(InventorySlot slot) //유물강화도 추가해야함
    {
        BigNumber amount = new BigNumber(slot.GetUpgradeCost());
        if (slot.GetDataType() == EDataType.Skill)
        {
            if (slot.Stack >= PublicConst.UpgradeStack && slot.Unlocked) 
            {
                //여기서 강화재화 소모 & 소모 가능여부 체크 둘 다 해줌 + 다른 재화 써야하면 나중에 수정
                if (!PlayerResourceManager.Instance.SpendResource(ResourceType.Emerald, amount)) return;
                slot.Stack -= PublicConst.UpgradeStack;
                slot.Upgrade();
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, new SlotPayload(slot));
            }
        }
        else
        {
            if (slot.Unlocked)
            {
                if (!PlayerResourceManager.Instance.SpendResource(ResourceType.EnhancementCube, amount)) return;
                slot.Upgrade();
                _eventChannel.RaiseEvent(EGameEventType.SlotUpdated, new SlotPayload(slot));
            }
            else
            {
                //강화실패
            }
        }
        StatModifyToPlayer();
    }
    #endregion
    #region 장착
    public void Equip(InventorySlot slot)
    {
        //해금 안된 경우
        if (!slot.Unlocked)
        {
            return;
        }
        switch (slot.GetDataType())
        {
            case EDataType.Weapon:
                if (_equippedWeapon != null) { UnEquip(_equippedWeapon); }
                _equippedWeapon = slot;
                slot.IsEquipped = true;
                break;
            case EDataType.Accessories:
                if (_equippedAccessory != null) { UnEquip(_equippedAccessory); }
                _equippedAccessory = slot;
                slot.IsEquipped = true;
                break;
            case EDataType.Skill:
                if (!_equippedSkills.Contains(slot) && _equippedSkills.Count < _currSkillSlot)
                {
                    _equippedSkills.Add(slot);
                    slot.IsEquipped = true;
                }
                else if (!_equippedSkills.Contains(slot) && _equippedSkills.Count >= _currSkillSlot) 
                {
                    _eventChannel.RaiseEvent(EGameEventType.CloseUpgradeUI);
                    _EquipSkill = slot;
                    _OnSkillChange = true;
                }
                else { slot.IsEquipped = false; }
                break;
        }
        StatModifyToPlayer();
        _eventChannel.RaiseEvent(EGameEventType.EquipChanged, new SlotPayload(slot));
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
        StatModifyToPlayer();
        _eventChannel.RaiseEvent(EGameEventType.EquipChanged, new SlotPayload(slot));
    }
    public void AddSkillSlot()
    {
        BigNumber amount = new BigNumber(100);
        if (!PlayerResourceManager.Instance.SpendResource(ResourceType.EnhancementCube, amount)) return;
        if (_currSkillSlot < _maxSkillSlot) _currSkillSlot++;
    }
    public void ChangeSkill() 
    {
        if (!_OnSkillChange || _EquipSkill == null || _UnequipSkill == null) return;

        int index = _equippedSkills.IndexOf(_UnequipSkill);
        if (index >= 0)
        {
            _equippedSkills[index].IsEquipped = false;
            _equippedSkills[index] = _EquipSkill;
            _EquipSkill.IsEquipped = true;
        }

        _OnSkillChange = false;
        StatModifyToPlayer();
        _eventChannel.RaiseEvent(EGameEventType.EquipChanged, new SlotPayload(_EquipSkill));
    }
    #endregion
    #region 스탯합산
    public void StatModifyToPlayer() 
    {
        CalculateStats();
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
            float value;
            if (_totalStatDict.ContainsKey(stat)) value = _totalStatDict[stat];
            else value = 0;
            StatModifier newModifier = new StatModifier(stat, Operation.Add, value);
            _InventoryModifiers.Add(newModifier);
            PlayerStatManager.Instance.AddModifier(newModifier);
        }
    }
    public void CalculateStats() 
    {
        CleanStatDict();
        AddPassiveStats(EDataType.Weapon);
        AddPassiveStats(EDataType.Accessories); //유물도 나중에 추가해야함
        AddPassiveStats(EDataType.Skill);
        AddEquipStats(_equippedWeapon);
        AddEquipStats(_equippedAccessory);
        foreach (var skillSlot in _equippedSkills) { AddEquipStats(skillSlot); }
    }
    private void AddPassiveStats(EDataType type) 
    {
        List<InventorySlot> targetInventory = GetInventory(type);
        foreach (var slot in targetInventory) 
        {
            if (!slot.Unlocked) continue;
            if (slot.BaseData is ItemDataSO item)
            {
                AddStat(item.PassiveStat, slot.PassiveEffectValue);

                switch (item.Type) 
                {
                    case EDataType.Weapon:
                        AddStat(StatType.CritRate, item.CriticalRate);
                        AddStat(StatType.CritDamage, slot.CriticalDMG);
                        AddStat(StatType.GoldMultiplier, slot.GoldPer);
                        break;
                    case EDataType.Accessories:
                        AddStat(StatType.MaxMana, slot.CriticalDMG);
                        AddStat(StatType.ExpMultiplier, slot.GoldPer);
                        break;
                }
            }
            else if (slot.BaseData is SkillDataSO skill) 
            {
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
            AddStat(item.EquipStat, slot.ActiveEffectValue);
        }
        else if (slot.BaseData is SkillDataSO skill) 
        {
            AddStat(skill.Stat, slot.ActiveEffectValue);
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
    private void CleanStatDict() 
    {
        foreach (StatType stat in Enum.GetValues(typeof(StatType)))
        {
            _totalStatDict[stat] = 0f;
        }
    }
    public InventorySlot GetNextSlot(InventorySlot slot) 
    {
        var targetInventory = GetInventory(slot.GetDataType());
        return targetInventory[targetInventory.IndexOf(slot) + 1];
    }
    public List<InventorySlot> GetInventory(EDataType type) 
    {
        switch (type)
        {
            case EDataType.Weapon: return _weaponInventory;
            case EDataType.Accessories: return _accessoriesInventory;       
            case EDataType.Artifact: return _artifactInventory;
            case EDataType.Skill: return _skillInventory;
            default: return null;
        }
    }
    public ItemDataSO GetRandomArtifact() 
    {
        int rand = UnityEngine.Random.Range(0, _artifactInventory.Count);
        if (_artifactInventory != null && _artifactInventory.Count > 0)
        {
            return _artifactInventory[rand].BaseData as ItemDataSO;
        }
        else return null;
    }
    public List<InventorySlot> GetEquippedSkills() { return _equippedSkills; }
    public InventorySlot GetEquippedWeapon() { return _equippedWeapon; }
    public InventorySlot GetEquippedAccessory() { return _equippedAccessory; }
    #endregion
}