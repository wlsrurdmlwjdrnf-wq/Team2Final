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

        // 랜덤 공격애니메이션
        int rand = Random.Range(1, 4);
        while (_attackIndex == rand) rand = Random.Range(1, 4);
        _attackIndex = rand;

        _player.Animator.SetInteger("AttackIndex", _attackIndex);
    }

    public void OnUpdate()
    {
        if (_player.CanAttack())
        {
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
        Player.TriggerNoAttack();
        _player.Animator.speed = 1f;
    }
    
}