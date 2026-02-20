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

            manager.AccessoriesDatabase.items.Add(itemSO);
        }
        EditorUtility.SetDirty(manager.AccessoriesDatabase);

        if (manager.ArtifactsDatabase != null) manager.ArtifactsDatabase.items.Clear();
        else manager.ArtifactsDatabase = ScriptableObject.CreateInstance<ItemDatabaseSO>();
        //유물 데이터
        foreach (var artifact in manager.ItemDatabaseSO.artifacts)
        {
            ItemDataSO itemSO = ScriptableObject.CreateInstance<ItemDataSO>();
            itemSO.Name = artifact.Name;
            itemSO.Type = EDataType.Artifact;
            itemSO.Element = artifact.Element;
            itemSO.Grade = artifact.Grade;
            itemSO.Level = artifact.Level;
            itemSO.DataSO = DataSOType.Resource;

            manager.ArtifactsDatabase.items.Add(itemSO);
        }
        EditorUtility.SetDirty(manager.ArtifactsDatabase);

        //if (manager.SkillDatabase != null) manager.SkillDatabase.skills.Clear();
        //else manager.SkillDatabase = ScriptableObject.CreateInstance<SkillDatabaseSO>();
        //foreach (var skill in manager.ItemDatabaseSO.skills)
        //{
        //    SkillDataSO skillSO = ScriptableObject.CreateInstance<SkillDataSO>();
        //    skillSO.Name = skill.Name;
        //    skillSO.Type = EDataType.Skill;
        //    skillSO.Grade = skill.Grade;
        //    skillSO.Level = skill.Level;
        //    skillSO.DataSO = DataSOType.Resource;

        //    manager.SkillDatabase.skills.Add(skillSO);
        //}
        //EditorUtility.SetDirty(manager.SkillDatabase);

        EditorUtility.SetDirty(manager.ItemDatabaseSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif