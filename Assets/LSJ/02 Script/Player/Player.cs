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
    private Collider2D _col;

    private float _lastAttackTime;

    public PlayerIdleState IdleState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerSkillState SkillState { get; private set; }
    public PlayerKnockBackState KnockBackState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }

    public Animator Animator => _anim;
    public SpriteRenderer SpriteRenderer => _sr;
    public Collider2D Collider => _col;
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
    public static event Action OnSkill;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();

        // 상태 초기화
        IdleState = new PlayerIdleState(this);
        AttackState = new PlayerAttackState(this);
        SkillState = new PlayerSkillState(this);
        KnockBackState = new PlayerKnockBackState(this);
        DeadState = new PlayerDeadState(this);
    }
    private void OnEnable()
    {
        OnKnockBack += ChangeKnockBackState;
        OnSkill += ChangeSkillState;
        _col.enabled = true;
        _lastAttackTime = Time.time;
        ChangeState(IdleState);
    }
    private void OnDisable()
    {
        OnKnockBack -= ChangeKnockBackState;
        OnSkill -= ChangeSkillState;
    }
    public bool CanAttack()
    {
        return Time.time >= _lastAttackTime + (1f / PlayerStatManager.Instance.AttackSpeed);
    }
    private void ChangeKnockBackState()
    {
        ChangeState(KnockBackState);
    }
    private void ChangeSkillState()
    {
        ChangeState(SkillState);
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
    public static void TriggerSkill()
    {
        OnSkill?.Invoke();
    }

    // Animation Event가 부를 함수
    public void OnAttackHit()
    {
        IDamageable target = EnemyManager.Instance.GetClosestEnemy(transform.position);

        if (target != null)
        {
            BigNumber damage = PlayerStatManager.Instance.AttackPower;
            // 크리티컬
            if (UnityEngine.Random.value < PlayerStatManager.Instance.CritRate)
            {
                damage *= PlayerStatManager.Instance.CritDamage;
                target.TakeDamage(damage, true);
            }
            else target.TakeDamage(damage);

            // 이펙트나 사운드 넣으면 될 듯
        }
    }
    public void OnChangeIdle()
    {
        ChangeState(IdleState);
    }
}
