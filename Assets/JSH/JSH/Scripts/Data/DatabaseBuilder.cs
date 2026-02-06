using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DatabaseBuilder
{
    [MenuItem("Tools/Generate/Weapon Database")]
    public static void GenerateWeaponDatabase()
    {
        // 저 경로에서 아이템데이터SO인 녀석들 guid 가져옴
        string[] guids = AssetDatabase.FindAssets("t:ItemDataSO", new[] { "Assets/JSH/SO/Weapon" });
        List<ItemDataSO> items = new List<ItemDataSO>();

        foreach (string guid in guids)
        {
            //guid > 에셋 고유 주소같은거
            string path = AssetDatabase.GUIDToAssetPath(guid);
            //주소 받아서 실제 데이터 받아옴
            ItemDataSO item = AssetDatabase.LoadAssetAtPath<ItemDataSO>(path);
            if (item != null && item.Type == EDataType.Weapon)
                items.Add(item);
        }
        //이 경로에 데이터베이스가 있으면 가져오고
        string dbPath = "Assets/JSH/SO/WeaponDatabase.asset";
        ItemDatabaseSO database = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(dbPath);

        //없으면 생성
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<ItemDatabaseSO>();
            AssetDatabase.CreateAsset(database, dbPath);
        }

        database.items = items;
        EditorUtility.SetDirty(database);  //변경된 에셋을 표시해줌
        AssetDatabase.SaveAssets();  //변경됨(SetDirty) 표시된 에셋을 실제로 디스크에 저장

        Debug.Log($"WeaponDatabaseSO : {items.Count}");
    }
    [MenuItem("Tools/Generate/Accessories Database")]
    public static void GenerateAccessoriesDatabase()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemDataSO", new[] { "Assets/JSH/SO/Accessories" });
        List<ItemDataSO> items = new List<ItemDataSO>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemDataSO item = AssetDatabase.LoadAssetAtPath<ItemDataSO>(path);
            if (item != null && item.Type == EDataType.Accessories)
                items.Add(item);
        }

        string dbPath = "Assets/JSH/SO/AccessoriesDatabase.asset";
        ItemDatabaseSO database = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(dbPath);

        if (database == null)
        {
            database = ScriptableObject.CreateInstance<ItemDatabaseSO>();
            AssetDatabase.CreateAsset(database, dbPath);
        }

        database.items = items;
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log($"AccessoriesDatabaseSO : {items.Count}");
    }
    [MenuItem("Tools/Generate/Skill Database")]
    public static void GenerateSkillDatabase()
    {
        string[] guids = AssetDatabase.FindAssets("t:SkillDataSO", new[] { "Assets/JSH/SO/Skills" });
        List<SkillDataSO> skills = new List<SkillDataSO>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SkillDataSO skill = AssetDatabase.LoadAssetAtPath<SkillDataSO>(path);
            if (skill != null && skill.Type == EDataType.Skill)
                skills.Add(skill);
        }

        string dbPath = "Assets/JSH/SO/SkillDatabase.asset";
        SkillDatabaseSO database = AssetDatabase.LoadAssetAtPath<SkillDatabaseSO>(dbPath);

        if (database == null)
        {
            database = ScriptableObject.CreateInstance<SkillDatabaseSO>();
            AssetDatabase.CreateAsset(database, dbPath);
        }

        database.skills = skills;
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log($"SkillDatabaseSO : {skills.Count}");
    }
}

