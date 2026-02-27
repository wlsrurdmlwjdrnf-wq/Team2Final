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
            if (_currAttackCount < baseData.TriggerCount) return false;
        }
        else 
        {
            if (Time.time < _lastCastTime + baseData.CoolTime) return false;
        }
        _enemyDamageables = SkillManager.Instance.CheckEnemy(PublicConst.SkillDetectRange);//스킬 시전 감지 사거리
        if (_enemyDamageables == null || _enemyDamageables.Count <= 0) return false;                       //범위내 적 체크
        if (!playerHpMp.UseMana(baseData.ManaCost)) return false;         //UseMana에서 마나 감소랑 마나 체크 둘 다 해줌
        return true;
    }
    public void Cast() 
    {
        Debug.Log($"CoolTime : {baseData.CoolTime}");
        Debug.Log($"TriggerCount : {baseData.TriggerCount}");
        Debug.Log($"AttackCount : {_currAttackCount}");
        _lastCastTime = Time.time;
        _currAttackCount = 0;
        effect.Apply(_enemyDamageables, baseData.Damage);
    }
}
