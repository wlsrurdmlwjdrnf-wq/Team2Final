using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : EntityStateMachine
{
    [Header("공격 관련 세팅")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private LayerMask _monsterLayer;
    [SerializeField] private float _attackRange = 2f;

    private Animator _anim;
    private SpriteRenderer _sr;

    private float _lastAttackTime;

    public PlayerIdleState IdleState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerSkillState SkillState { get; private set; }
    public PlayerKnockBackState KnockBackState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }

    public Animator Animator => _anim;
    public SpriteRenderer SpriteRenderer => _sr;
    public Transform AttackPoint => _attackPoint;
    public LayerMask MonsterLayer => _monsterLayer;
    public float AttackRange => _attackRange;
    public float LastAttackTime
    {
        get => _lastAttackTime;
        set => _lastAttackTime = value;
    }

    public static event Action OnKnockBack;
    public static event Action OnAttack;
    public static event Action OnNoAttack;
    public static event Action OnDead;

    private void Awake()
    {
        OnKnockBack += ChangeKnockBackState;

        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        _lastAttackTime = Time.time;

        // 상태 초기화
        IdleState = new PlayerIdleState(this);
        AttackState = new PlayerAttackState(this);
        SkillState = new PlayerSkillState(this);
        KnockBackState = new PlayerKnockBackState(this);
        DeadState = new PlayerDeadState(this);

        ChangeState(IdleState);
    }

    public bool CanAttack()
    {
        return Time.time >= _lastAttackTime + (1f / PlayerStatManager.Instance.AttackSpeed);
    }
    private void ChangeKnockBackState()
    {
        ChangeState(KnockBackState);
    }
    public static void TriggerKnockBack()
    {
        OnKnockBack?.Invoke();
    }
    public static void TriggerAttack()
    {
        OnAttack?.Invoke();
    }
    public static void TriggerNoAttack()
    {
        OnNoAttack?.Invoke();
    }
    public static void TriggerDead()
    {
        OnDead?.Invoke();
    }
    private void OnDisable()
    {
        OnKnockBack -= ChangeKnockBackState;
    }
}
