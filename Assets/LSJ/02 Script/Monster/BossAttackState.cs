using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : IEntityState
{
    private readonly Boss _boss;

    public BossAttackState(Boss boss) => _boss = boss;

    public void OnEnter()
    {
        _boss.Animator.speed = _boss.AttackSpeed;
        _boss.Animator.SetBool("IsAttacking", true);
    }

    public void OnUpdate()
    {
        //if (_boss.CanAttack())
        //{
        //    Collider2D hit = Physics2D.OverlapCircle(
        //        _boss.AttackPoint.position,
        //        _boss.AttackRange,
        //        _boss.PlayerLayer
        //    );

        //    if (hit == null)
        //        _boss.ChangeState(_boss.IdleState);
        //    else
        //        _boss.ChangeState(_boss.AttackState); // 재진입
        //}

        // 애니메이션이벤트로 공격애니메이션 끝날때 이벤트 추가해서 강제로 Idle상태로 복귀

    }

    public void OnFixedUpdate() { }

    public void OnExit()
    {
        _boss.Animator.speed = 1f;
    }

}