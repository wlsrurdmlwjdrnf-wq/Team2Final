using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    private List<SkillInstance> _equippedSkills = new List<SkillInstance>();
    [SerializeField] private GameEventChannelSO _eventChannel;
    //나중에 수정 필요
    [SerializeField] private PlayerHpMp _playerHpMp;
    [SerializeField] private GameObject _player;

    private bool _IsPlayerReady = true;
    //죽음 이벤트 받아서 죽어도 스킬 시전되지 않게 하기
    //넉백시에도 스킬 시전 금지
    //속성 데미지
    private void OnEnable()
    {
        _eventChannel.OnEventRaised += HandleEvent;
        Player.OnAttack += OnNormalAttack;
        Player.OnAttack += PlayerReady;
        Player.OnDead += PlayerDead;
        Player.OnKnockBack += PlayerDead;
    }
    private void OnDisable()
    {
        _eventChannel.OnEventRaised -= HandleEvent;
        Player.OnAttack -= OnNormalAttack;
        Player.OnAttack -= PlayerReady;
        Player.OnDead -= PlayerDead;
        Player.OnKnockBack -= PlayerDead;
    }
    private void HandleEvent(EGameEventType eventType, object payload) 
    {
        if (eventType == EGameEventType.SlotUpdated || 
            eventType == EGameEventType.EquipRequest ||
            eventType == EGameEventType.EquipChanged) 
        {
            if(payload is InventorySlot slot && slot.BaseData is SkillDataSO)
                RefreshSlots();
        }
    }
    public void RefreshSlots() 
    {
        _equippedSkills.Clear();
        List<InventorySlot> equippedSlots = InventorySystem.Instance.GetEquippedSkills();

        foreach (var slot in equippedSlots) 
        {
            if (slot.BaseData is SkillDataSO skillData) 
            {
                SkillInstance instance = SkillFactory.CreateInstance(slot);
                _equippedSkills.Add(instance);
            }
        }
    }
    private void Update()
    {
        foreach (var skill in _equippedSkills) 
        {
            if (skill.baseData.TriggerCount <= 0) 
            {
                if (skill.CanCast(_playerHpMp)) skill.Cast();                
            }
        }
    }

    private void PlayerDead() { _IsPlayerReady = false; }
    private void PlayerReady() { _IsPlayerReady = true; }

    public void OnNormalAttack() 
    {
        foreach (var skill in _equippedSkills) 
        {
            if (skill.baseData.TriggerCount > 0) 
            {
                skill.OnNormalAttack();
                if (skill.CanCast(_playerHpMp)) skill.Cast(); 
            }
        }
    }

    public Collider2D[] CheckEnemy(float range) 
    {
        Vector2 playerPos = _player.transform.position;
        Vector2 boxCenter = playerPos + Vector2.right * (range/2);
        Vector2 boxSize = new Vector2(range, 2f);
        Collider2D[] hit = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f, LayerMask.GetMask("Monster"));
        if (hit != null) return hit;
        else return null;
    }

    public GameObject GetClosestEnemy(Collider2D[] enemies) 
    {
        if (_player == null || !_player.activeSelf) return null;
        float closestDist = float.MaxValue;
        Collider2D closestEnemy = null;
        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(_player.transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestEnemy = enemy;
            }
        }
        return closestEnemy.gameObject;
    }
}
