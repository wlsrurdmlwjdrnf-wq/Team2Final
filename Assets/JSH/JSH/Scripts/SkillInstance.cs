using System.Collections.Generic;
using UnityEngine;

public class SkillInstance : IUpgradable
{
    public SkillDataSO baseData;
    public int currentLevel;
    public ISkillEffect effect;
    private float _lastCastTime;
    private float _currAttackCount;
    private List<IDamageable> _enemyDamageables;
    public SkillInstance(InventorySlot slot, ISkillEffect effect)
    {
        baseData = slot.BaseData as SkillDataSO;
        currentLevel = slot.Level;
        this.effect = effect;
        _lastCastTime = Time.time;
        _currAttackCount = 0;
    }
    public int Level => currentLevel;
    public void Upgrade()
    {
        currentLevel++;
    }
    public void OnNormalAttack() 
    {
        _currAttackCount++;
    }
    public bool CanCast(PlayerHpMp playerHpMp)
    {
        if (baseData.TriggerCount > 0)
        {
            if (_currAttackCount < baseData.TriggerCount)
            {
                return false;
            }
        }
        else
        {
            if (Time.time < _lastCastTime + baseData.CoolTime)
            {
                return false;
            }
        }

        _enemyDamageables = SkillManager.Instance.CheckEnemy(PublicConst.SkillDetectRange);
        if (_enemyDamageables == null || _enemyDamageables.Count <= 0)
        {
            SkillManager.instance.GetNewEnemy();
            return false;
        }

        if (!playerHpMp.UseMana(baseData.ManaCost))
        {
            return false;
        }
        return true;
    }

    public void Cast()
    {
        _lastCastTime = Time.time;
        _currAttackCount = 0;
        effect.Apply(_enemyDamageables, baseData.Damage);
    }

    public float GetCooldownProgress() 
    {
        float progress;
        if (baseData.TriggerCount > 0)
        {
            progress = Mathf.Clamp01(_currAttackCount / (float)baseData.TriggerCount);
        }
        else
        {
            float elapsed = Time.time - _lastCastTime;
            progress = Mathf.Clamp01(elapsed / baseData.CoolTime);
        }
        return progress;
    }
}
