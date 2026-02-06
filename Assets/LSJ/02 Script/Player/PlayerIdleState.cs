using UnityEngine;

public class PlayerIdleState : IEntityState
{
    private readonly Player _player;

    public PlayerIdleState(Player player) => _player = player;

    public void OnEnter()
    {
        _player.Animator.speed = 1f;
        _player.Animator.SetInteger("AttackIndex", 0);
        _player.Animator.SetBool("IsKnockBack", false);
        _player.Animator.SetBool("IsDead", false);
        _player.Animator.SetBool("IsSkilling", false);

        Debug.Log("IDLE");
    }

    public void OnUpdate()
    {
        // 공격 가능 + 적이 범위 안에 있으면 Attack으로 전이
        if (_player.CanAttack())
        {
            Collider2D hit = Physics2D.OverlapCircle(
                _player.AttackPoint.position,
                _player.AttackRange,
                _player.MonsterLayer
            );

            if (hit != null)
            {
                _player.ChangeState(_player.AttackState);
            }
        }
    }

    public void OnFixedUpdate() { }
    public void OnExit() { }
}
