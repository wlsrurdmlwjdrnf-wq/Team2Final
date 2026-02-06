using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleState : IEntityState
{
    private readonly MonsterBase _monster;

    public MonsterIdleState(MonsterBase monster) => _monster = monster;

    public void OnEnter()
    {
        _monster.Animator.SetBool("IsAttacking", false);
        _monster.Animator.SetBool("IsDead", false);
    }

    public void OnUpdate()
    {
        if (_monster is Boss boss && boss.CanAttack())
        {
            Collider2D hit = Physics2D.OverlapCircle(
                boss.AttackPoint.position,
                boss.AttackRange,
                boss.PlayerLayer
            );

            if (hit != null)
            {
                boss.ChangeState(boss.AttackState);
            }
        }
    }

    public void OnFixedUpdate() { }
    public void OnExit() { }
}
