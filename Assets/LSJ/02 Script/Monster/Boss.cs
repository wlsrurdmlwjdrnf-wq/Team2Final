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
        base.Awake();
        AttackState = new BossAttackState(this);
        _lastAttackTime = Time.time;
    }
    public bool CanAttack()
    {
        return Time.time >= _lastAttackTime + (1f / _baseStats.baseAttackSpeed);
    }

    // Animation Event가 호출할 함수
    public void OnBossAttackHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            AttackPoint.position,
            AttackRange,
            PlayerLayer
        );

        if (hits.Length == 0) return;

        PlayerHpMp target = hits[0].GetComponent<PlayerHpMp>();
        if (target != null)
        {
            BigNumber damage = CurrentAtk;

            target.TakeDamage(damage);

            TryKnockBackAttack();

            LastAttackTime = Time.time;
        }
    }
    private void TryKnockBackAttack()
    {
        int rand = Random.Range(0, 100);
        if (rand < 30)
        {
            Player.TriggerKnockBack();  // 보스가 플레이어를 넉백시키는 로직
        }
    }
    public void OnChangeIdle()
    {
        ChangeState(IdleState);
    }
}
