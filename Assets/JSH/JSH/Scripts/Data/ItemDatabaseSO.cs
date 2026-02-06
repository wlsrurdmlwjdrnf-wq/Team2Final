using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabaseSO", menuName = "Database/ItemDatabaseSO")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemDataSO> items;
}

[CreateAssetMenu(fileName = "SkillDatabaseSO", menuName = "Database/SkillDatabaseSO")]
public class SkillDatabaseSO : ScriptableObject
{
    public List<SkillDataSO> skills;
}
