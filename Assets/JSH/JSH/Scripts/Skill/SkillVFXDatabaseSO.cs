using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillVFXDatabaseSO", menuName = "Database/SkillVFXDatabaseSO")]
public class SkillVFXDatabaseSO : ScriptableObject
{
    [System.Serializable]
    public class VFXEntry 
    {
        public ESkillEffectType effectType;
        public ElementType element;
        public SkillVFX vfxPrefab;
    }

    public List<VFXEntry> entries;
    private Dictionary<ESkillEffectType, SkillVFX> _vfxDict;

    private void OnEnable()
    {
        _vfxDict = new Dictionary<ESkillEffectType, SkillVFX>();
        foreach (var entry in entries) 
        {
            if (!_vfxDict.ContainsKey(entry.effectType)) 
            {
                _vfxDict.Add(entry.effectType, entry.vfxPrefab);
            }
        }
    }

    public SkillVFX GetVFX(ESkillEffectType effectType) 
    {
        if (_vfxDict.TryGetValue(effectType, out var prefab)) { return prefab; }
        return null;
    }
}
