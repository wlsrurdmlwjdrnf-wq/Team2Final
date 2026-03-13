#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class ItemDatabaseConverter
{
    private static TSO FindSOByType<TSO>() where TSO : ScriptableObject
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
    public static void Convert(ItemSkillDataManager manager) 
    {
        if (manager.ItemDatabaseSO == null) manager.ItemDatabaseSO = FindSOByType<ItemDatabaseSO>();

        if (manager.WeaponDatabase != null) manager.WeaponDatabase.items.Clear();
        else manager.WeaponDatabase = ScriptableObject.CreateInstance<ItemDatabaseSO>();
        //무기 데이터
        foreach (var weapon in manager.ItemDatabaseSO.weapons)
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
            itemSO.DataSO = DataSOType.Resource;
            itemSO.EquipATKbyLv = weapon.EquipATKByLv;
            itemSO.PassiveATKbyLv = weapon.passiveATKByLv;

            itemSO.CriticalDMG = weapon.CriticalDMG;
            itemSO.CriticalDMGbyLv = weapon.CriticalDMGByLV;
            itemSO.CriticalRate = 0;
            itemSO.GoldPer = weapon.GoldPer;
            itemSO.GoldPerbyLv = weapon.goldPerByLv;

            itemSO.IconKey = weapon.Icon;
            itemSO.EffectKey = weapon.Effect;
            itemSO.SoundKey = weapon.Sound;

            manager.WeaponDatabase.items.Add(itemSO);
        }
        EditorUtility.SetDirty(manager.WeaponDatabase);

        if (manager.AccessoriesDatabase != null) manager.AccessoriesDatabase.items.Clear();
        else manager.AccessoriesDatabase = ScriptableObject.CreateInstance<ItemDatabaseSO>();
        //악세서리 데이터
        foreach (var accessory in manager.ItemDatabaseSO.accessories)
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

            itemSO.IconKey = accessory.Name;

            manager.AccessoriesDatabase.items.Add(itemSO);
        }
        EditorUtility.SetDirty(manager.AccessoriesDatabase);

        EditorUtility.SetDirty(manager.ItemDatabaseSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif