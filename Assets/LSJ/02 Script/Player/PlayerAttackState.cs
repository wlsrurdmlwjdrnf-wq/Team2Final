using System.Linq;
using UnityEngine;

public class PlayerAttackState : IEntityState
{
    private readonly Player _player;
    private int _attackIndex = 0;
    private float _monsterDistance;

    public PlayerAttackState(Player player) => _player = player;

    public void OnEnter()
    {
        Player.TriggerAttack();

        _player.Animator.speed = PlayerStatManager.Instance.AttackSpeed;

        // 랜덤 공격애니메이션
        int rand = Random.Range(1, 4);
        while (_attackIndex == rand) rand = Random.Range(1, 4);
        _attackIndex = rand;

        if (_player.CanAttack())
        {
            _player.LastAttackTime = Time.time;
            _player.Animator.SetInteger("AttackIndex", _attackIndex);
        }
        else
            _player.Animator.Play("Stand");

        Debug.Log("ATTACK");
    }

    public void OnUpdate()
    {
        //Collider2D hit = Physics2D.OverlapCircle(
        //    _player.AttackPoint.position,
        //    _player.AttackRange,
        //    _player.MonsterLayer
        //);

        //if (hit == null)
        //    _player.ChangeState(_player.IdleState);
        //else if (_player.CanAttack())
        //    _player.ChangeState(_player.AttackState); // 재진입

        if (EnemyManager.Instance.GetClosestEnemy(_player.transform.position) != null)
        {
            _monsterDistance = (EnemyManager.Instance.GetClosestEnemy
                (_player.transform.position).Transform.position
                - _player.transform.position).sqrMagnitude;
        }

        if (_monsterDistance <= _player.AttackRange && _player.CanAttack())
        {
            _player.ChangeState(_player.AttackState);
        }
        else if (_monsterDistance > _player.AttackRange || !EnemyManager.Instance.enemies.Any())
        {
            _player.ChangeState(_player.IdleState);
        }
    }

    public void OnFixedUpdate() { }

    public void OnExit()
    {
        Player.TriggerNoAttack();
        _player.Animator.speed = 1f;
    }
    
}