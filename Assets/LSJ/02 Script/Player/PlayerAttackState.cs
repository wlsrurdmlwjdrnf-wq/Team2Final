using UnityEngine;

public class PlayerAttackState : IEntityState
{
    private readonly Player _player;
    private int _attackIndex = 0;

    public PlayerAttackState(Player player) => _player = player;

    public void OnEnter()
    {
        Player.TriggerAttack();
        _player.Animator.speed = PlayerStatManager.Instance.AttackSpeed;
        
        int rand = Random.Range(1, 4);
        while (_attackIndex == rand) rand = Random.Range(1, 4);
        _attackIndex = rand;
        _player.Animator.SetInteger("AttackIndex", _attackIndex);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            _player.AttackPoint.position,
            _player.AttackRange,
            _player.MonsterLayer
        );

        if (hits.Length > 0)
        {
            IDamageable target = hits[0].GetComponent<IDamageable>();
            if (target != null)
            {
                BigNumber damage = PlayerStatManager.Instance.AttackPower;

                // 크리티컬
                if (Random.value < PlayerStatManager.Instance.CritRate)
                {
                    damage *= PlayerStatManager.Instance.CritDamage;
                }

                target.TakeDamage(damage);
                _player.LastAttackTime = Time.time;
            }
        }
    }

    public void OnUpdate()
    {
        if (_player.CanAttack())
        {
            // 적이 여전히 있으면 재공격, 없으면 Idle로
            Collider2D hit = Physics2D.OverlapCircle(
                _player.AttackPoint.position,
                _player.AttackRange,
                _player.MonsterLayer
            );

            if (hit == null)
                _player.ChangeState(_player.IdleState);
            else
                _player.ChangeState(_player.AttackState); // 재진입
        }
    }

    public void OnFixedUpdate() { }
    public void OnExit() 
    {
        Debug.Log("공격상태 exit");
        Player.TriggerNoAttack();
    }
}