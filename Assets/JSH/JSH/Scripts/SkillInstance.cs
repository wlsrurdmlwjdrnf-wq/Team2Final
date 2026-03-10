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
        Debug.Log($"[SkillInstance] CanCast 시작: Skill={baseData.name}, CurrAttackCount={_currAttackCount}, TriggerCount={baseData.TriggerCount}");

        if (baseData.TriggerCount > 0)
        {
            if (_currAttackCount < baseData.TriggerCount)
            {
                Debug.Log("[SkillInstance] 조건 불충족: 공격 횟수 부족");
                return false;
            }
        }
        else
        {
            if (Time.time < _lastCastTime + baseData.CoolTime)
            {
                Debug.Log("[SkillInstance] 조건 불충족: 쿨타임 미완료");
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
            Debug.Log("[SkillInstance] 조건 불충족: 마나 부족");
            return false;
        }

        Debug.Log("[SkillInstance] 모든 조건 충족 → 캐스트 가능");
        return true;
    }

    public void Cast()
    {
        Debug.Log($"[SkillInstance] Cast 실행: Skill={baseData.name}, Damage={baseData.Damage}, EnemyCount={_enemyDamageables.Count}");
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
        Debug.Log($"[SkillInstance] 쿨타임 진행도: {progress}");
        return progress;
    }
}
