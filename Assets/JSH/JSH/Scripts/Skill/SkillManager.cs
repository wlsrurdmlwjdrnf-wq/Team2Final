using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }
    private List<SkillInstance> _equippedSkills = new List<SkillInstance>();
    //나중에 수정 필요
    private PlayerStatManager _statManager;
    private void Awake()
    {
        Instance = this;
    }
    public void Initialize() 
    {
        //나중에 수정 필요
        _statManager = PlayerStatManager.Instance;
        _equippedSkills.Clear();
        List<InventorySlot> equippedSlots = InventorySystem.Instance.GetEquippedSkills();

        foreach (var data in equippedSlots) 
        {
            if (data.instance is SkillInstance skill) 
            {
                _equippedSkills.Add(skill);
                SkillInstance instance = SkillFactory.CreateInstance(skill.baseData);
            }
        }
    }
    private void Update()
    {
        foreach (var skill in _equippedSkills) 
        {
            //나중에 수정해야함
            if (skill.CanCast(_statManager.MaxMana)) 
            {
                skill.Cast(_statManager);
            }
        }
    }
    public void OnNormalAttack() 
    {
        foreach (var skill in _equippedSkills) { skill.OnNormalAttack(); }
    }
}
