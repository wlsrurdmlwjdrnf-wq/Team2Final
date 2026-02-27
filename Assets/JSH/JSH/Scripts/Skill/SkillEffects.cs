using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExplosion : ISkillEffect
{
    private SkillVFX _vfx;
    private ESkillEffectType _skillEffectType;
    private ElementType _elementType;
    public FireExplosion(SkillVFX vfx, ESkillEffectType effectType)
    {
        _vfx = vfx;
        _skillEffectType = effectType;
    }
    public void Apply(List<IDamageable> enemies, float damageDuplicator)
    {
        if (enemies == null || enemies.Count <= 0)
        {
            Debug.Log("EnemyNull");
            return;
        }
        var effect = PoolManager.Instance.GetFromPool(_vfx);
        effect.Setup(_elementType, SkillManager.Instance.GetClosestEnemy(enemies), damageDuplicator);
    }
}
public class EarthGrow : ISkillEffect
{
    private SkillVFX _vfx;
    private ESkillEffectType _skillEffectType;
    private ElementType _elementType;
    public EarthGrow(SkillVFX vfx, ESkillEffectType effectType)
    {
        _vfx = vfx;
        _skillEffectType = effectType;
    }
    public void Apply(List<IDamageable> enemies, float damageDuplicator)
    {
        if (enemies == null || enemies.Count <= 0)
        {
            Debug.Log("EnemyNull");
            return;
        }
        var effect = PoolManager.Instance.GetFromPool(_vfx);
        effect.Setup(_elementType, SkillManager.Instance.GetClosestEnemy(enemies), damageDuplicator);
    }
}
public class WindGust : ISkillEffect
{
    private SkillVFX _vfx;
    private ESkillEffectType _skillEffectType;
    private ElementType _elementType;
    public WindGust(SkillVFX vfx, ESkillEffectType effectType)
    {
        _vfx = vfx;
        _skillEffectType = effectType;
    }
    public void Apply(List<IDamageable> enemies, float damageDuplicator)
    {
        if (enemies == null || enemies.Count <= 0)
        {
            Debug.Log("EnemyNull");
            return;
        }
        var effect = PoolManager.Instance.GetFromPool(_vfx);
        effect.Setup(_elementType, SkillManager.Instance.GetClosestEnemy(enemies), damageDuplicator);
    }
}
public class IceSlash : ISkillEffect
{
    private SkillVFX _vfx;
    private ESkillEffectType _skillEffectType;
    private ElementType _elementType;
    public IceSlash(SkillVFX vfx, ESkillEffectType effectType)
    {
        _vfx = vfx;
        _skillEffectType = effectType;
    }
    public void Apply(List<IDamageable> enemies, float damageDuplicator)
    {
        if (enemies == null || enemies.Count <= 0)
        {
            Debug.Log("EnemyNull");
            return;
        }
        var effect = PoolManager.Instance.GetFromPool(_vfx);
        effect.Setup(_elementType, SkillManager.Instance.GetClosestEnemy(enemies), damageDuplicator);
    }
}
public class Lightning : ISkillEffect
{
    private SkillVFX _vfx;
    private ESkillEffectType _skillEffectType;
    private ElementType _elementType;
    private WaitForSeconds _effectWaitSec = new WaitForSeconds(0.1f);
    private Vector3 _effectOffset = new Vector3(2.5f, 0f, 0f);

    public Lightning(SkillVFX vfx, ESkillEffectType effectType)
    {
        _vfx = vfx;
        _skillEffectType = effectType;
    }
    public void Apply(List<IDamageable> enemies, float damageDuplicator)
    {
        if (enemies == null || enemies.Count <= 0)
        {
            Debug.Log("EnemyNull");
            return;
        }
        SkillManager.Instance.StartCoroutine(SpawnLightning(enemies, damageDuplicator));
    }
    private IEnumerator SpawnLightning(List<IDamageable> enemies, float damageDuplicator) 
    {
        Vector3 spawnPos = SkillManager.Instance.GetClosestEnemy(enemies).transform.position;
        for (int i = 0; i < 5; i++)
        {
            var effect = PoolManager.Instance.GetFromPool(_vfx);
            effect.Setup(_elementType, spawnPos, damageDuplicator);
            //effect.OnAttackHit();
            spawnPos += _effectOffset;
            yield return _effectWaitSec;
        }
    }
}