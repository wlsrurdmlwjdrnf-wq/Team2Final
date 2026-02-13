using System.Collections;
using UnityEngine;

public class TestSkill_1 : ISkillEffect 
{
    private SkillVFX _vfx;
    private ESkillEffectType _skillEffectType;
    public TestSkill_1(SkillVFX vfx, ESkillEffectType effectType)
    {
        _vfx = vfx;
        _skillEffectType = effectType;
    }
    public void Apply(Collider2D[] enemies, float damageDuplicator) 
    {
        if (enemies == null || enemies.Length <= 0)
        {
            Debug.Log("EnemyNull");
            return;
        }
        var effect = PoolManager.Instance.GetFromPool(_vfx);
        effect.Setup(SkillManager.Instance.GetClosestEnemy(enemies), damageDuplicator);
        Debug.Log("TestSkill_1");
    }
}
public class TestSkill_2 : ISkillEffect
{
    private SkillVFX _vfx;
    private ESkillEffectType _skillEffectType;
    public TestSkill_2(SkillVFX vfx, ESkillEffectType effectType)
    {
        _vfx = vfx;
        _skillEffectType = effectType;
    }
    public void Apply(Collider2D[] enemies, float damageDuplicator)
    {
        if (enemies == null || enemies.Length <= 0)
        {
            Debug.Log("EnemyNull");
            return;
        }
        var effect = PoolManager.Instance.GetFromPool(_vfx);
        effect.Setup(SkillManager.Instance.GetClosestEnemy(enemies), damageDuplicator);
        Debug.Log("TestSkill_2");
    }
}
public class TestSkill_3 : ISkillEffect
{
    private SkillVFX _vfx;
    private ESkillEffectType _skillEffectType;
    public TestSkill_3(SkillVFX vfx, ESkillEffectType effectType)
    {
        _vfx = vfx;
        _skillEffectType = effectType;
    }
    public void Apply(Collider2D[] enemies, float damageDuplicator)
    {
        if (enemies == null || enemies.Length <= 0)
        {
            Debug.Log("EnemyNull");
            return;
        }
        var effect = PoolManager.Instance.GetFromPool(_vfx);
        effect.Setup(SkillManager.Instance.GetClosestEnemy(enemies), damageDuplicator);
        Debug.Log("TestSkill_3");
    }
}
public class TestSkill_4 : ISkillEffect
{
    private SkillVFX _vfx;
    private ESkillEffectType _skillEffectType;
    public TestSkill_4(SkillVFX vfx, ESkillEffectType effectType)
    {
        _vfx = vfx;
        _skillEffectType = effectType;
    }
    public void Apply(Collider2D[] enemies, float damageDuplicator)
    {
        if (enemies == null || enemies.Length <= 0) 
        {
            Debug.Log("EnemyNull");
            return;
        }
        var effect = PoolManager.Instance.GetFromPool(_vfx);
        effect.Setup(SkillManager.Instance.GetClosestEnemy(enemies), damageDuplicator);
        Debug.Log("TestSkill_4");
    }
}
public class EarthGrow : ISkillEffect
{
    private SkillVFX _vfx;
    private ESkillEffectType _skillEffectType;

    public EarthGrow(SkillVFX vfx, ESkillEffectType effectType)
    {
        _vfx = vfx;
        _skillEffectType = effectType;
    }
    public void Apply(Collider2D[] enemies, float damageDuplicator)
    {
        if (enemies == null || enemies.Length <= 0)
        {
            Debug.Log("EnemyNull");
            return;
        }
        var effect = PoolManager.Instance.GetFromPool(_vfx);
        effect.Setup(SkillManager.Instance.GetClosestEnemy(enemies), damageDuplicator);
        Debug.Log("EarthGrow");
    }
}
public class Lightning : ISkillEffect
{
    private SkillVFX _vfx;
    private ESkillEffectType _skillEffectType;
    private WaitForSeconds _effectWaitSec = new WaitForSeconds(0.2f);
    private Vector3 _effectOffset = new Vector3(2.5f, 0f, 0f);

    public Lightning(SkillVFX vfx, ESkillEffectType effectType)
    {
        _vfx = vfx;
        _skillEffectType = effectType;
    }
    public void Apply(Collider2D[] enemies, float damageDuplicator)
    {
        if (enemies == null || enemies.Length <= 0)
        {
            Debug.Log("EnemyNull");
            return;
        }
        SkillManager.Instance.StartCoroutine(SpawnLightning(enemies, damageDuplicator));
        Debug.Log("Lightning");
    }
    private IEnumerator SpawnLightning(Collider2D[] enemies, float damageDuplicator) 
    {
        Vector3 spawnPos = SkillManager.Instance.GetClosestEnemy(enemies).transform.position;
        for (int i = 0; i < 5; i++)
        {
            var effect = PoolManager.Instance.GetFromPool(_vfx);
            effect.Setup(spawnPos, damageDuplicator);
            spawnPos += _effectOffset;
            yield return new WaitForSeconds(0.2f);
        }
    }
}