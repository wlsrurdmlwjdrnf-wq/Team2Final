using UnityEngine;

public class SkillInstance : IUpgradable
{
    public SkillDataSO baseData;
    public int currentLevel;
    public ISkillEffect effect;
    private float _lastCastTime;
    private float _currAttackCount;
    public SkillInstance(SkillDataSO data, ISkillEffect skillEffect)
    {
        baseData = data;
        currentLevel = data.Level;
        effect = skillEffect;
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
    //나중에 매개변수 수정 필요할 수 있음
    public bool CanCast(float mana) 
    {
        if (Time.time < _lastCastTime + baseData.CoolTime) return false;  //쿨타임체크 
        if (mana < baseData.ManaCost) return false;  //마나체크
        if (_currAttackCount < baseData.TriggerCount) return false;  //평타횟수체크
        return true;
    }
    public void Cast(PlayerStatManager player) 
    {
        //현재마나로 수정필요
        if (!CanCast(player.MaxMana)) return;
        //마나감소
        _lastCastTime = Time.time;
        _currAttackCount = 0;
        effect.Apply();
    }
}
