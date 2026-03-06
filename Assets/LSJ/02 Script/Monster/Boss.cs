using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonsterBase
{
    //[Header("공격 관련 세팅")]
    //[SerializeField] private Transform _attackPoint;
    //[SerializeField] private LayerMask _playerLayer;
    //[SerializeField] private float _attackRange = 1f;

    private float _lastAttackTime;
    private PlayerHpMp _target;
    public BossAttackState AttackState { get; private set; }
    //public Transform AttackPoint => _attackPoint;
    //public LayerMask PlayerLayer => _playerLayer;
    //public float AttackRange => _attackRange;
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
        _target = FindObjectOfType<PlayerHpMp>();
        

        Player.OnAttack += OnChangeAttack;
    }
    public bool CanAttack()
    {
        return Time.time >= _lastAttackTime + (1f / _baseStats.baseAttackSpeed);
    }

    // Animation Event가 호출할 함수
    public void OnBossAttackHit()
    {
        if (_target != null)
        {
            BigNumber damage = CurrentAtk;

            _target.TakeDamage(damage);

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
    public void OnChangeAttack()
    {
        if(CanAttack()) ChangeState(AttackState);
    }
    private void OnDestroy()
    {
        Player.OnAttack -= OnChangeAttack;
    }
}
