using System.Linq;
using UnityEngine;

public class PlayerIdleState : IEntityState
{
    private readonly Player _player;
    private float _monsterDistance;
    public PlayerIdleState(Player player) => _player = player;

    public void OnEnter()
    {   
        _player.Animator.SetInteger("AttackIndex", 0);
        _player.Animator.SetBool("IsKnockBack", false);
        _player.Animator.SetBool("IsDead", false);
        _player.Animator.SetBool("IsSkilling", false);
    }

    public void OnUpdate()
    {
        //Collider2D hit = Physics2D.OverlapCircle(
        //    _player.AttackPoint.position,
        //    _player.AttackRange,
        //    _player.MonsterLayer
        //);
        if (EnemyManager.Instance.enemies == null || !EnemyManager.Instance.enemies.Any())
        {
            return;
        }

        _monsterDistance = (EnemyManager.Instance.GetClosestEnemy(
            _player.transform.position).Transform.position 
            - _player.transform.position).sqrMagnitude;

        if (_monsterDistance <= _player.AttackRange)
        {
            _player.ChangeState(_player.AttackState);
        }
    }

    public void OnFixedUpdate() { }
    public void OnExit() { }
}
