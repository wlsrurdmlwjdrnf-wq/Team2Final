using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    [SerializeField] private ItemDatabaseSO _database;

    private Dictionary<int, WeaponData> _weaponData = new Dictionary<int, WeaponData>();
    private Dictionary<int, AccessoryData> _accessoryData = new Dictionary<int, AccessoryData>();
    private Dictionary<int, ArtifactData> _artifactData = new Dictionary<int, ArtifactData>();
    private Dictionary<int, SkillData> _skillData = new Dictionary<int, SkillData>();

    private Dictionary<string, Sprite> _icon = new Dictionary<string, Sprite>();
    private Dictionary<string, AudioClip> _sound = new Dictionary<string, AudioClip>();
    private Dictionary<string, GameObject> _effect = new Dictionary<string, GameObject>();

    protected override void Init()
    {
        base.Init();
        //InitData();
    }
    /*
    private void InitData()
    {
        _weaponData = _database.weapons.ToDictionary(x => x.ID);
        _accessoryData = _database.accessories.ToDictionary(x => x.ID);
        _artifactData = _database.artifacts.ToDictionary(x => x.ID);
        _skillData = _database.skills.ToDictionary(x => x.ID);
    }
    */

}
