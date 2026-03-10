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
    private bool _IsAuto = true;

    private void OnEnable()
    {
        Debug.Log("[SkillManager] OnEnable 호출됨");

        _eventChannel.OnEventRaised += HandleEvent;
        Player.OnAttack += OnNormalAttack;
        Player.OnAttack += PlayerReady;
        Player.OnDead += PlayerDead;
        Player.OnKnockBack += PlayerDead;
        if (StageManager.Instance != null) 
        { 
            StageManager.Instance.OnStageChanged += GetNewEnemy;
            Debug.Log("[SkillManager] StageManager 연결 성공");
        }
    }
    private void OnDisable()
    {
        Debug.Log("[SkillManager] OnDisable 호출됨");

        _eventChannel.OnEventRaised -= HandleEvent;
        Player.OnAttack -= OnNormalAttack;
        Player.OnAttack -= PlayerReady;
        Player.OnDead -= PlayerDead;
        Player.OnKnockBack -= PlayerDead;
        if (StageManager.Instance != null) StageManager.Instance.OnStageChanged -= GetNewEnemy;       
    }
    private void HandleEvent(EGameEventType eventType, IGameEventPayload payload) 
    {
        Debug.Log($"[SkillManager] HandleEvent 호출됨: {eventType}");

        switch (eventType) 
        {
            case EGameEventType.SlotUpdated:
            case EGameEventType.EquipChanged:
            case EGameEventType.EquipRequest:
                if (payload is SlotPayload slot && slot.Slot.BaseData is SkillDataSO)
                {
                    Debug.Log("[SkillManager] RefreshSlots 실행");
                    RefreshSlots();
                }

                break;
            case EGameEventType.RequestSkillUse:
                if (payload is SlotPayload useSlot && useSlot.Slot.BaseData is SkillDataSO)
                {
                    var instance = GetSkillInstance(useSlot.Slot);
                    Debug.Log($"[SkillManager] RequestSkillUse: Slot={useSlot.Slot.BaseData.name}, Instance={(instance != null)}");

                    if (instance != null && instance.CanCast(_playerHpMp))
                    {
                        Debug.Log("[SkillManager] 스킬 캐스트 실행");
                        instance.Cast();
                    }
                    else
                    {
                        Debug.LogWarning("[SkillManager] 스킬 캐스트 실패 (Instance null 또는 CanCast=false)");
                    }
                }

                break;
        }
    }
    public void RefreshSlots() 
    {
        Debug.Log("[SkillManager] RefreshSlots 시작");


        List<InventorySlot> equippedSlots = InventorySystem.Instance.GetEquippedSkills();
        Debug.Log($"[SkillManager] 현재 장착된 슬롯 개수: {equippedSlots.Count}");


        //새로 장착된 슬롯 추가
        foreach (InventorySlot slot in equippedSlots)
        {
            bool exists = false;
            foreach (SkillInstance skill in _equippedSkills)
            {
                if (skill.baseData == slot.BaseData)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                SkillInstance instance = SkillFactory.CreateInstance(slot);
                _equippedSkills.Add(instance);
                Debug.Log($"[SkillManager] 새 스킬 추가: {slot.BaseData.name}");

            }
        }
        //해제된 슬롯 제거
        for (int i = _equippedSkills.Count - 1; i >= 0; i--)
        {
            SkillInstance skill = _equippedSkills[i];
            bool stillEquipped = false;
            foreach (InventorySlot slot in equippedSlots)
            {
                if (slot.BaseData == skill.baseData)
                {
                    stillEquipped = true;
                    break;
                }
            }

            if (!stillEquipped)
            {
                Debug.Log($"[SkillManager] 스킬 제거: {skill.baseData.Name}");

                _equippedSkills.RemoveAt(i);
            }
        }
        Debug.Log($"[SkillManager] RefreshSlots 완료. 현재 스킬 개수: {_equippedSkills.Count}");

    }
    private void Update()
    {
        if (_IsAuto && _IsPlayerReady) CheckCast();    
    }
    private void PlayerDead() { _IsPlayerReady = false; }
    private void PlayerReady() { _IsPlayerReady = true; }
    public void OnNormalAttack() 
    {
        foreach (var skill in _equippedSkills) skill.OnNormalAttack();
        if (_IsAuto && _IsPlayerReady) CheckCast();
    }

    public void CheckCast() 
    {
        foreach (var skill in _equippedSkills)
        {
            if (skill.CanCast(_playerHpMp) && _IsAuto) skill.Cast();
        }
    }

    public void GetNewEnemy() 
    {
        Debug.Log("[SkillManager] GetNewEnemy 호출됨 (레이어 기반)");

        _enemies.Clear();

        // Monster 레이어 번호 가져오기
        int monsterLayer = LayerMask.NameToLayer("Monster");

        // 모든 활성 오브젝트 중 Monster 레이어만 필터링
        foreach (var mono in GameObject.FindObjectsOfType<MonoBehaviour>())
        {
            if (mono.gameObject.layer == monsterLayer && mono.TryGetComponent(out IDamageable damageable))
            {
                _enemies.Add(damageable);
                Debug.Log($"[SkillManager] 몬스터 발견: {mono.name}, Pos={mono.transform.position}");
            }
        }

        Debug.Log($"[SkillManager] 적 리스트 갱신 완료: {_enemies.Count}개");
    }
    public List<IDamageable> CheckEnemy(float range) 
    {
        List<IDamageable> enemiesInRange = new List<IDamageable>();
        Vector2 playerPos = _player.transform.position;
        Vector2 boxCenter = playerPos + Vector2.right * (range/2);
        Vector2 boxSize = new Vector2(range, 2f);
        Debug.Log($"[SkillManager] CheckEnemy 호출됨, Range={range}, PlayerPos={playerPos}");
        foreach (var enemy in _enemies)
        {
            Debug.Log($"[SkillManager] Enemy={enemy}, Active={(enemy as MonoBehaviour).gameObject.activeSelf}, Pos={(enemy as MonoBehaviour).transform.position}");
        }

        foreach (var enemy in _enemies) 
        {
            if (enemy is MonoBehaviour mono && mono.gameObject.activeSelf)
            {
                Vector2 enemyPos = mono.transform.position;

                //AABB공식 |posA.x - posB.x| <= sizeA.x/2 + sizeB.x/2, |posA.y - posB.y| <= sizeA.y/2 + sizeB.y/2
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
    public SkillInstance GetSkillInstance(InventorySlot slot)
    {
        if (slot.BaseData is SkillDataSO skillData)
        {
            foreach (var skill in _equippedSkills)
            {
                if (skill.baseData == skillData) return skill;
            }
        }
        return null;
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
    public void ToggleAuto() { _IsAuto = !_IsAuto; }
    public GameObject GetPlayer() { return _player; }
}
