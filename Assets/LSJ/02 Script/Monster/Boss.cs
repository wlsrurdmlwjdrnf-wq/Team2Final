using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonsterBase
{
    [Header("공격 관련 세팅")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _attackRange = 1f;

    private float _lastAttackTime;
    public BossAttackState AttackState { get; private set; }
    public Transform AttackPoint => _attackPoint;
    public LayerMask PlayerLayer => _playerLayer;
    public float AttackRange => _attackRange;
    public float AttackSpeed => _baseStats.baseAttackSpeed;
    public float LastAttackTime
    {
        get => _lastAttackTime;
        set => _lastAttackTime = value;
    }
    protected override void Awake()
    {
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();

        Name = _baseStats.monsterName;

        IdleState = new MonsterIdleState(this);
        DeadState = new MonsterDeadState(this);
        AttackState = new BossAttackState(this);

        _lastAttackTime = Time.time;

        ChangeState(IdleState);
    }
    public bool CanAttack()
    {
        return Time.time >= _lastAttackTime + (1f / _baseStats.baseAttackSpeed);
    }
}
