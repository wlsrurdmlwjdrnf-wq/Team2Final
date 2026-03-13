using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabaseSO", menuName = "Database/ItemDatabaseSO")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemDataSO> items = new List<ItemDataSO>();
    public List<WeaponData> weapons = new List<WeaponData>();
    public List<AccessoryData> accessories = new List<AccessoryData>();
    public List<ArtifactData> artifacts = new List<ArtifactData>();
    public List<SkillData> skills = new List<SkillData>();
    public List<PlayerInitData> playerInits = new List<PlayerInitData>();
    public List<StageData> stages = new List<StageData>();
}