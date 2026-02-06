using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSkillDataManager : MonoBehaviour
{
    public static ItemSkillDataManager Instance { get; private set; }

    public ItemDatabaseSO WeaponDatabase;
    public ItemDatabaseSO AccessoriesDatabase;
    public SkillDatabaseSO SkillDatabase;
    public SkillVFXDatabaseSO SkillVFXDatabase;

    private void Awake()
    {
        Instance = this;
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
            if (item.Type == card.Type &&
                item.Grade == card.Grade &&
                item.Tier == card.Tier) 
            {
                return item;
            }
        }
        return null;
    }

    //이쪽은 더 보강할 필요가 있음
    public SkillDataSO GetSkillData(ItemCard card) 
    {
        foreach (var skill in SkillDatabase.skills) 
        {
            if (skill.Type == card.Type &&
                skill.Grade == card.Grade) 
            {
                return skill;
            }
        }
        return null;
    }
}
