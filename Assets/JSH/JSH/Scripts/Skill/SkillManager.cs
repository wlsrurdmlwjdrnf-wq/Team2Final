using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    private List<SkillInstance> _equippedSkills = new List<SkillInstance>();
    [SerializeField] private GameEventChannelSO _eventChannel;
    
    [SerializeField] private PlayerHpMp _playerHpMp;
    [SerializeField] private GameObject _player;

    private List<IDamageable> _enemies = new List<IDamageable>();

    private bool _IsPlayerReady = true;

    private void OnEnable()
    {
        _eventChannel.OnEventRaised += HandleEvent;
        Player.OnAttack += OnNormalAttack;
        Player.OnAttack += PlayerReady;
        Player.OnDead += PlayerDead;
        Player.OnKnockBack += PlayerDead;
        if (StageManager.Instance != null) StageManager.Instance.OnStageChanged += GetNewEnemy;
    }
    private void OnDisable()
    {
        _eventChannel.OnEventRaised -= HandleEvent;
        Player.OnAttack -= OnNormalAttack;
        Player.OnAttack -= PlayerReady;
        Player.OnDead -= PlayerDead;
        Player.OnKnockBack -= PlayerDead;
        if (StageManager.Instance != null) StageManager.Instance.OnStageChanged -= GetNewEnemy;
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
            if (skill.baseData.TriggerCount <= 0 && _IsPlayerReady) 
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
            if (skill.baseData.TriggerCount > 0 && _IsPlayerReady) 
            {
                skill.OnNormalAttack();
                if (skill.CanCast(_playerHpMp)) skill.Cast(); 
            }
        }
    }

    public void GetNewEnemy() 
    {
        Debug.Log("NewEnemy");
        _enemies.Clear();
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (var monster in monsters) 
        {
            if (monster.TryGetComponent(out IDamageable damageable)) { _enemies.Add(damageable); }
        }
    }

    public List<IDamageable> CheckEnemy(float range) 
    {
        List<IDamageable> enemiesInRange = new List<IDamageable>();
        Vector2 playerPos = _player.transform.position;
        Vector2 boxCenter = playerPos + Vector2.right * (range/2);
        Vector2 boxSize = new Vector2(range, 2f);

        foreach (var enemy in _enemies) 
        {
            if (enemy is MonoBehaviour mono && mono.gameObject.activeSelf)
            {
                Vector2 enemyPos = mono.transform.position;

                //AABB°ø½Ä |posA.x - posB.x| <= sizeA.x/2 + sizeB.x/2, |posA.y - posB.y| <= sizeA.y/2 + sizeB.y/2
                bool overlapX = Mathf.Abs(boxCenter.x - enemyPos.x) <= boxSize.x / 2f;
                bool overlapY = Mathf.Abs(boxCenter.y - enemyPos.y) <= boxSize.y / 2f;

                if (overlapX && overlapY) { enemiesInRange.Add(enemy); }
            }
        }
        return enemiesInRange;
    }

    private void OnDrawGizmos()
    {
        Vector2 playerPos = _player.transform.position;
        Vector2 boxCenter = playerPos + Vector2.right * (PublicConst.SkillDetectRange / 2);
        Vector2 boxSize = new Vector2(PublicConst.SkillDetectRange, 2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boxCenter, boxSize);

        List<IDamageable> damageables = CheckEnemy(PublicConst.SkillDetectRange);
        if (damageables.Count > 0) 
        {
            foreach (var enemy in damageables)
            {
                Gizmos.DrawWireSphere((enemy as MonoBehaviour).transform.position, PublicConst.NormalEnemyRadius);
            }
        }
    }

    public GameObject GetClosestEnemy(List<IDamageable> enemies) 
    {
        if (_player == null || !_player.activeSelf) return null;
        float closestDist = float.MaxValue;
        GameObject closestEnemy = null;

        foreach (var enemy in enemies)
        {
            if (enemy is MonoBehaviour mono && mono.gameObject.activeSelf) 
            {
                float dist = (_player.transform.position - mono.transform.position).sqrMagnitude;
                if (dist < closestDist) 
                {
                    closestDist = dist;
                    closestEnemy = mono.gameObject;
                }
            }
        }
        return closestEnemy;
    }

    public GameObject GetPlayer() { return _player; }
}
