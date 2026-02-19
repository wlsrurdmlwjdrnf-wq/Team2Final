using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemSkillDataManager : MonoBehaviour
{
    public static ItemSkillDataManager Instance { get; private set; }

    public ItemDatabaseSO ItemDatabaseSO;
    public ItemDatabaseSO WeaponDatabase;
    public ItemDatabaseSO AccessoriesDatabase;
    public ItemDatabaseSO ArtifactsDatabase;
    public SkillDatabaseSO SkillDatabase;
    public SkillVFXDatabaseSO SkillVFXDatabase;

    private void Awake()
    {
        Instance = this;

        ItemDatabaseSO = FindSOByType<ItemDatabaseSO>();
        ConvertData();
    }

    private void Start()
    {
        foreach (var entry in SkillVFXDatabase.entries) 
        {
            if (entry.vfxPrefab != null) 
            {
                PoolManager.Instance.CreatePool(entry.vfxPrefab, 3, null);
            }
        }
        InventorySystem.Instance.Initialize();
        TestUIManager.Instance.Initialize();
        GachaSystem.Instance.Initialize();
    }

    private TSO FindSOByType<TSO>() where TSO : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(TSO).Name);
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.StartsWith("Assets/AssetIgnore")) continue;
            return AssetDatabase.LoadAssetAtPath<TSO>(path);
        }
        return null;
    }

    private void ConvertData()
    {
        if (ItemDatabaseSO == null) return;

        ItemDatabaseSO.items.Clear();

        if (WeaponDatabase != null) WeaponDatabase.items.Clear();
        else WeaponDatabase = ScriptableObject.CreateInstance<ItemDatabaseSO>();
        //무기 데이터
        foreach (var weapon in ItemDatabaseSO.weapons)
            {
                ItemDataSO itemSO = ScriptableObject.CreateInstance<ItemDataSO>();
                itemSO.Name = weapon.Name;
                itemSO.Type = EDataType.Weapon;
                itemSO.Grade = weapon.Grade;
                itemSO.Tier = weapon.Tier;
                itemSO.Level = weapon.Level;
                itemSO.EquipStat = StatType.AttackPower;
                itemSO.EquipValue = weapon.equipATK;
                itemSO.PassiveStat = StatType.AttackPower;
                itemSO.PassiveValue = weapon.passiveATK;
                itemSO.CriticalDMG = weapon.CriticalDMG;
                itemSO.CriticalRate = 0;
                itemSO.GoldPer = weapon.GoldPer;
                itemSO.EquipATKbyLv = weapon.EquipATKByLv;
                itemSO.PassiveATKbyLv = weapon.passiveATKByLv;
                itemSO.GoldPerbyLv = weapon.goldPerByLv;
                itemSO.DataSO = DataSOType.Resource;

                WeaponDatabase.items.Add(itemSO);
                Debug.Log($"weapon : { itemSO.Tier}  {weapon.Tier}");
            }
        EditorUtility.SetDirty(WeaponDatabase);

        if (AccessoriesDatabase != null) AccessoriesDatabase.items.Clear();
        else AccessoriesDatabase = ScriptableObject.CreateInstance<ItemDatabaseSO>();
        //악세서리 데이터
        foreach (var accessory in ItemDatabaseSO.accessories)
            {
                ItemDataSO itemSO = ScriptableObject.CreateInstance<ItemDataSO>();
                itemSO.Name = accessory.Name;
                itemSO.Type = EDataType.Accessories;
                itemSO.Grade = accessory.grade;
                itemSO.Tier = accessory.Tier;
                itemSO.Level = accessory.Level;
                itemSO.EquipStat = StatType.MaxHP;
                itemSO.EquipValue = accessory.EquipHPPer;
                itemSO.PassiveStat = StatType.MaxHP;
                itemSO.PassiveValue = accessory.PassiveHPPer;
                itemSO.DataSO = DataSOType.Resource;
                itemSO.EquipATKbyLv = accessory.EquipHPPerByLv;
                itemSO.PassiveATKbyLv = accessory.passiveHPPerByLv;
                itemSO.CriticalDMG = accessory.MPPer;
                itemSO.CriticalDMGbyLv = accessory.MPPerByLv;
                itemSO.GoldPer = accessory.EXPPer;
                itemSO.GoldPerbyLv = accessory.EXPPerByLv;

                AccessoriesDatabase.items.Add(itemSO);
                Debug.Log($"accessory : {itemSO.Tier}  {accessory.Tier}");
        }
        EditorUtility.SetDirty(AccessoriesDatabase);

        if (ArtifactsDatabase != null) ArtifactsDatabase.items.Clear();
        else ArtifactsDatabase = ScriptableObject.CreateInstance<ItemDatabaseSO>();
        //유물 데이터
        foreach (var artifact in ItemDatabaseSO.artifacts)
            {
                ItemDataSO itemSO = ScriptableObject.CreateInstance<ItemDataSO>();
                itemSO.Name = artifact.Name;
                itemSO.Type = EDataType.Accessories;//나중에 고치기
                itemSO.Element = artifact.Element;
                itemSO.Grade = artifact.Grade;
                itemSO.Level = artifact.Level;
                itemSO.DataSO = DataSOType.Resource;

                ArtifactsDatabase.items.Add(itemSO);
            }
        EditorUtility.SetDirty(ArtifactsDatabase);

        if (SkillDatabase != null) SkillDatabase.skills.Clear();
        else SkillDatabase = ScriptableObject.CreateInstance<SkillDatabaseSO>();
            foreach (var skill in ItemDatabaseSO.skills)
            {
                SkillDataSO skillSO = ScriptableObject.CreateInstance<SkillDataSO>();
                skillSO.Name = skill.Name;
                skillSO.Type = EDataType.Skill;
                skillSO.Grade = skill.Grade;
                skillSO.Level = skill.Level;
                skillSO.DataSO = DataSOType.Resource;

                SkillDatabase.skills.Add(skillSO);
            }
        EditorUtility.SetDirty(SkillDatabase);

        EditorUtility.SetDirty(ItemDatabaseSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public ItemDataSO GetItemData(ItemCard card) 
    {
        ItemDatabaseSO targetDB = null;

        switch (card.Type) 
        {
            case EDataType.Weapon:
                targetDB = WeaponDatabase;
                break;
            case EDataType.Accessories:
                targetDB = AccessoriesDatabase;
                break;
        }

        if ( targetDB == null ) return null;
        foreach (var item in targetDB.items)
        {
            if (item.Type == card.Type && item.Grade == card.Grade)
            {
                if (card.Tier == 0 || item.Tier == card.Tier)
                {
                    Debug.Log($"GetItemData: Found {item.Name}");
                    return item;
                }
            }
        }

        Debug.LogWarning($"GetItemData: No match for {card.Type}, Grade={card.Grade}, Tier={card.Tier}");
        return null;

    }

    //이쪽은 더 보강할 필요가 있음
    public SkillDataSO GetSkillData(ItemCard card) 
    {
        List<SkillDataSO> dataLists = new List<SkillDataSO>();
        foreach (var skill in SkillDatabase.skills) 
        {
            if (skill.Type == card.Type && skill.Grade == card.Grade) 
            {
                dataLists.Add(skill);
            }
        }
        if (dataLists.Count > 0) 
        {
            int index = Random.Range(0, dataLists.Count);
            return dataLists[index];
        }
        return null;
    }
}
